"use client";

import { use, useState, useEffect } from "react";
import Link from "next/link";
import { useSearchParams } from "next/navigation";
import { notFound } from "next/navigation";
import { getHotelById } from "@/data/hotels";
import { getRoomsForHotel } from "@/data/rooms";
import { getReviewsForHotel } from "@/data/reviews";
import { apiGetHotelDetail, apiGetRoomAvailability, apiGetHotelReviews, apiGetCancellationPolicy, apiGetHotelTaxes } from "@/lib/api";
import ImageCarousel from "@/components/ImageCarousel";
import StarRating from "@/components/StarRating";
import AmenityBadge from "@/components/AmenityBadge";
import RoomCard from "@/components/RoomCard";
import ReviewCard from "@/components/ReviewCard";
import BookingBasket from "@/components/BookingBasket";
import { getDefaultCheckIn, getDefaultCheckOut } from "@/lib/utils";
import { useLocale } from "@/context/LocaleContext";
import type { Hotel, RoomConfig, Review, HotelImage, RoomImage, Amenity, ApiHotelDetail, ApiRoomAvailability, BoardOption, ApiCancellationPolicy, BasketItemTax } from "@/types";

function mapApiDetailToHotel(api: ApiHotelDetail, fallback: Hotel | null): Hotel {
  const images: HotelImage[] = api.images.map((url, i) => ({
    id: i + 1,
    hotel_id: api.id,
    url,
    alt_text: `${api.name} - Image ${i + 1}`,
    sort_order: i + 1,
  }));

  const amenities: Amenity[] = api.amenities.map((name, i) => ({
    id: i + 1,
    name,
    category: "hotel" as const,
    icon: fallback?.amenities.find((a) => a.name.toLowerCase() === name.toLowerCase())?.icon || "circle",
  }));

  return {
    id: api.id,
    name: api.name,
    city_id: fallback?.city_id ?? 0,
    city_name: api.city,
    region_name: fallback?.region_name ?? "",
    country_name: api.country,
    stars: api.stars,
    latitude: api.latitude ?? fallback?.latitude ?? 0,
    longitude: api.longitude ?? fallback?.longitude ?? 0,
    check_in_time: fallback?.check_in_time ?? "14:00",
    check_out_time: fallback?.check_out_time ?? "12:00",
    phone: api.phone ?? "",
    email: api.email ?? "",
    website: api.website ?? "",
    margin: fallback?.margin ?? 0,
    enabled: true,
    summary: fallback?.summary ?? "",
    description: api.description,
    images,
    amenities,
    min_price: fallback?.min_price ?? 0,
    avg_rating: fallback?.avg_rating ?? 0,
    review_count: fallback?.review_count ?? 0,
  };
}

interface RoomTypeGroup {
  roomTypeDbId: number;
  roomTypeName: string;
  capacity: number;
  images: RoomImage[];
  offers: RoomOffer[];
}

interface RoomOffer {
  id: number;
  basePricePerNight: number;
  availableRooms: number;
  boardOptions: BoardOption[];
}

function groupApiRoomsByType(apiRooms: ApiRoomAvailability[], hotelId: number): RoomTypeGroup[] {
  const groups = new Map<number, RoomTypeGroup>();

  for (const r of apiRooms) {
    const dbId = r.roomTypeDbId ?? r.roomTypeId;
    const sorted = [...r.boardOptions].sort((a, b) => a.pricePerNight - b.pricePerNight);
    const offer: RoomOffer = {
      id: Number(r.roomTypeId),
      basePricePerNight: r.basePricePerNight,
      availableRooms: r.availableRooms,
      boardOptions: sorted.map((b): BoardOption => ({
        board_type_id: Number(b.boardTypeId),
        board_type_name: b.boardTypeName,
        board_type_code: b.boardTypeName.toLowerCase().replace(/\s+/g, "_"),
        price_supplement: b.pricePerNight,
      })),
    };

    const existing = groups.get(dbId);
    if (existing) {
      existing.offers.push(offer);
      // Merge images
      for (const url of r.images) {
        if (!existing.images.some((img) => img.url === url)) {
          existing.images.push({
            id: existing.images.length + 1,
            room_config_id: dbId,
            url,
            alt_text: `${r.roomTypeName} - Image ${existing.images.length + 1}`,
            sort_order: existing.images.length + 1,
          });
        }
      }
    } else {
      groups.set(dbId, {
        roomTypeDbId: dbId,
        roomTypeName: r.roomTypeName,
        capacity: r.maxGuests,
        images: r.images.map((url, i) => ({
          id: i + 1,
          room_config_id: dbId,
          url,
          alt_text: `${r.roomTypeName} - Image ${i + 1}`,
          sort_order: i + 1,
        })),
        offers: [offer],
      });
    }
  }

  // Sort offers by base price within each group
  for (const group of groups.values()) {
    group.offers.sort((a, b) => a.basePricePerNight - b.basePricePerNight);
  }

  return Array.from(groups.values());
}

// Keep for fallback data compatibility
function mapApiRoomsToRoomConfigs(apiRooms: ApiRoomAvailability[], hotelId: number): RoomConfig[] {
  return apiRooms.map((r) => {
    const basePrice = r.basePricePerNight;
    const sorted = [...r.boardOptions].sort((a, b) => a.pricePerNight - b.pricePerNight);
    return {
      id: Number(r.roomTypeId),
      hotel_id: hotelId,
      room_type_id: Number(r.roomTypeId),
      room_type_name: r.roomTypeName,
      capacity: r.maxGuests,
      quantity: r.availableRooms,
      base_price_per_night: basePrice,
      currency_code: "EUR",
      images: r.images.map((url, i) => ({
        id: Number(r.roomTypeId) * 10 + i,
        room_config_id: Number(r.roomTypeId),
        url,
        alt_text: `${r.roomTypeName} - Image ${i + 1}`,
        sort_order: i + 1,
      })),
      amenities: [],
      board_options: sorted.map((b): BoardOption => ({
        board_type_id: Number(b.boardTypeId),
        board_type_name: b.boardTypeName,
        board_type_code: b.boardTypeName.toLowerCase().replace(/\s+/g, "_"),
        price_supplement: b.pricePerNight,
      })),
    };
  });
}

export default function HotelDetailPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = use(params);
  const hotelId = Number(id);
  const fallbackHotel = getHotelById(hotelId) ?? null;
  const { locale, t } = useLocale();
  const searchParams = useSearchParams();

  const [hotel, setHotel] = useState<Hotel | null>(fallbackHotel);
  const [roomGroups, setRoomGroups] = useState<RoomTypeGroup[]>([]);
  const [rooms, setRooms] = useState<RoomConfig[]>(fallbackHotel ? getRoomsForHotel(hotelId) : []);
  const [reviews, setReviews] = useState<Review[]>(fallbackHotel ? getReviewsForHotel(hotelId) : []);
  const [avgRating, setAvgRating] = useState(0);

  const [checkIn, setCheckIn] = useState(searchParams.get("checkIn") || getDefaultCheckIn());
  const [checkOut, setCheckOut] = useState(searchParams.get("checkOut") || getDefaultCheckOut());
  const [guests, setGuests] = useState(Number(searchParams.get("guests")) || 2);
  const [cancellationPolicy, setCancellationPolicy] = useState<ApiCancellationPolicy[]>([]);
  const [policyOpen, setPolicyOpen] = useState(false);
  const [taxes, setTaxes] = useState<BasketItemTax[]>([]);

  // Fetch hotel detail from API
  useEffect(() => {
    apiGetHotelDetail(hotelId)
      .then((detail) => setHotel(mapApiDetailToHotel(detail, fallbackHotel)))
      .catch(() => { /* keep fallback */ });
    apiGetCancellationPolicy(hotelId)
      .then(setCancellationPolicy)
      .catch(() => { /* no policy data */ });
    apiGetHotelTaxes(hotelId)
      .then((t) => setTaxes(t.map((tx) => ({ taxTypeName: tx.taxTypeName, rate: tx.rate, isPercentage: tx.isPercentage }))))
      .catch(() => { /* no tax data */ });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [hotelId, locale]);

  // Fetch reviews from API
  useEffect(() => {
    apiGetHotelReviews(hotelId)
      .then((res) => {
        setAvgRating(res.averageRating);
        setReviews(
          res.reviews.map((r) => ({
            id: r.id,
            hotel_id: hotelId,
            user_name: r.userName,
            rating: r.rating,
            title: "",
            comment: r.comment ?? "",
            created_at: r.createdAt,
            response: r.responseComment
              ? { id: 0, review_id: r.id, responder_name: "Hotel Management", comment: r.responseComment, created_at: r.responseDate ?? "" }
              : undefined,
          }))
        );
      })
      .catch(() => {
        // Keep fallback reviews
        const fallbackReviews = getReviewsForHotel(hotelId);
        setReviews(fallbackReviews);
        setAvgRating(fallbackReviews.length > 0 ? fallbackReviews.reduce((s, r) => s + r.rating, 0) / fallbackReviews.length : 0);
      });
  }, [hotelId, locale]);

  // Fetch room availability when dates or guests change
  useEffect(() => {
    if (!checkIn || !checkOut) return;
    apiGetRoomAvailability(hotelId, checkIn, checkOut, guests)
      .then((apiRooms) => {
        setRoomGroups(groupApiRoomsByType(apiRooms, hotelId));
        setRooms(mapApiRoomsToRoomConfigs(apiRooms, hotelId));
      })
      .catch(() => {
        setRoomGroups([]);
        setRooms(getRoomsForHotel(hotelId));
      });
  }, [hotelId, checkIn, checkOut, guests, locale]);

  if (!hotel) notFound();

  // Compute avg from local reviews if not set from API
  const displayRating = avgRating || (reviews.length > 0 ? reviews.reduce((s, r) => s + r.rating, 0) / reviews.length : 0);

  return (
    <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6">
      {/* Breadcrumb */}
      <nav className="mb-4 text-sm text-gray-400">
        <Link href="/hotels" className="hover:text-gray-600">{t("client.nav.hotels")}</Link>
        <span className="mx-2">/</span>
        <span className="text-gray-600">{hotel.name}</span>
      </nav>

      <div className="flex flex-col gap-6 lg:flex-row">
        {/* Main content */}
        <div className="flex-1">
          {/* Carousel */}
          <ImageCarousel images={hotel.images} aspectRatio="aspect-[16/9]" />

          {/* Hotel info */}
          <div className="mt-6">
            <div className="flex flex-wrap items-start justify-between gap-3">
              <div>
                <h1 className="text-2xl font-bold text-gray-900 sm:text-3xl">{hotel.name}</h1>
                <p className="mt-1 text-gray-500">{hotel.city_name}, {hotel.region_name ? `${hotel.region_name}, ` : ""}{hotel.country_name}</p>
              </div>
              <StarRating stars={hotel.stars} size="lg" />
            </div>

            {/* Rating summary */}
            {displayRating > 0 && (
              <div className="mt-3 flex items-center gap-2">
                <span className="inline-flex items-center rounded-lg bg-blue-600 px-3 py-1 text-sm font-bold text-white">
                  {displayRating.toFixed(1)}
                </span>
                <span className="text-sm text-gray-500">
                  {reviews.length} {reviews.length === 1 ? t("client.hotel_detail.review") : t("client.hotel_detail.reviews_plural")}
                </span>
              </div>
            )}

            <p className="mt-4 text-gray-600 leading-relaxed">{hotel.description}</p>

            {/* Info grid */}
            <div className="mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
              <div className="rounded-lg bg-gray-50 p-3">
                <p className="text-xs font-medium text-gray-400">{t("client.hotel_detail.check_in")}</p>
                <p className="text-sm font-medium text-gray-900">{hotel.check_in_time}</p>
              </div>
              <div className="rounded-lg bg-gray-50 p-3">
                <p className="text-xs font-medium text-gray-400">{t("client.hotel_detail.check_out")}</p>
                <p className="text-sm font-medium text-gray-900">{hotel.check_out_time}</p>
              </div>
              <div className="rounded-lg bg-gray-50 p-3">
                <p className="text-xs font-medium text-gray-400">{t("client.hotel_detail.contact")}</p>
                <p className="text-sm font-medium text-gray-900">{hotel.phone}</p>
              </div>
            </div>

            {/* Amenities */}
            {hotel.amenities.length > 0 && (
              <div className="mt-6">
                <h2 className="mb-3 text-lg font-semibold text-gray-900">{t("client.hotel_detail.amenities")}</h2>
                <div className="flex flex-wrap gap-2">
                  {hotel.amenities.map((a) => (
                    <AmenityBadge key={a.id} amenity={a} />
                  ))}
                </div>
              </div>
            )}

            {/* Cancellation policy */}
            {cancellationPolicy.length > 0 && (
              <div className="mt-6">
                <button
                  onClick={() => setPolicyOpen(!policyOpen)}
                  className="flex w-full items-center justify-between text-lg font-semibold text-gray-900"
                >
                  <span>{t("client.hotel_detail.cancellation_policy")}</span>
                  <svg className={`h-5 w-5 text-gray-400 transition-transform ${policyOpen ? "rotate-180" : ""}`} fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                    <path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7" />
                  </svg>
                </button>
                {policyOpen && (
                  <div className="mt-3 space-y-2">
                    {cancellationPolicy.map((p, i) => (
                      <div key={i} className="flex items-center gap-2 rounded-lg bg-gray-50 px-3 py-2 text-sm">
                        {p.penaltyPercentage === 0 ? (
                          <span className="text-green-600">{t("client.hotel_detail.free_cancellation")}</span>
                        ) : (
                          <span className="text-gray-700">
                            {p.hoursBeforeCheckIn}h {t("client.hotel_detail.before_checkin")} &rarr; {p.penaltyPercentage}% {t("client.hotel_detail.penalty")}
                          </span>
                        )}
                      </div>
                    ))}
                  </div>
                )}
              </div>
            )}

            {/* Date selection for rooms */}
            <div className="mt-8">
              <h2 className="mb-4 text-lg font-semibold text-gray-900">{t("client.hotel_detail.available_rooms")}</h2>
              <div className="mb-4 flex flex-wrap gap-3">
                <div>
                  <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.search.check_in")}</label>
                  <input
                    type="date"
                    value={checkIn}
                    onChange={(e) => setCheckIn(e.target.value)}
                    className="rounded-lg border border-gray-300 px-3 py-2 text-sm"
                  />
                </div>
                <div>
                  <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.search.check_out")}</label>
                  <input
                    type="date"
                    value={checkOut}
                    onChange={(e) => setCheckOut(e.target.value)}
                    className="rounded-lg border border-gray-300 px-3 py-2 text-sm"
                  />
                </div>
                <div>
                  <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.search.guests")}</label>
                  <select
                    value={guests}
                    onChange={(e) => setGuests(Number(e.target.value))}
                    className="rounded-lg border border-gray-300 px-3 py-2 text-sm"
                  >
                    {[1, 2, 3, 4, 5, 6].map((n) => (
                      <option key={n} value={n}>{n} {n === 1 ? t("client.search.guest") : t("client.search.guests_plural")}</option>
                    ))}
                  </select>
                </div>
              </div>
              <div className="space-y-4">
                {roomGroups.length > 0
                  ? roomGroups.map((group) => (
                      <RoomCard
                        key={group.roomTypeDbId}
                        group={group}
                        hotelId={hotel.id}
                        hotelName={hotel.name}
                        taxes={taxes}
                        checkIn={checkIn}
                        checkOut={checkOut}
                      />
                    ))
                  : rooms.map((room) => (
                      <RoomCard
                        key={room.id}
                        room={room}
                        hotelId={hotel.id}
                        hotelName={hotel.name}
                        taxes={taxes}
                        checkIn={checkIn}
                        checkOut={checkOut}
                      />
                    ))}
              </div>
            </div>

            {/* Map placeholder */}
            <div className="mt-8">
              <h2 className="mb-3 text-lg font-semibold text-gray-900">{t("client.hotel_detail.location")}</h2>
              <div className="flex h-48 items-center justify-center rounded-xl bg-gray-100 text-sm text-gray-400">
                Map placeholder — {hotel.latitude.toFixed(4)}°N, {hotel.longitude.toFixed(4)}°E
              </div>
            </div>

            {/* Reviews */}
            {reviews.length > 0 && (
              <div className="mt-8">
                <h2 className="mb-4 text-lg font-semibold text-gray-900">
                  {t("client.hotel_detail.reviews")} ({reviews.length})
                </h2>
                <div className="space-y-4">
                  {reviews.map((review) => (
                    <ReviewCard key={review.id} review={review} />
                  ))}
                </div>
              </div>
            )}
          </div>
        </div>

        {/* Sidebar basket */}
        <div className="w-full shrink-0 lg:w-72">
          <div className="sticky top-20">
            <BookingBasket />
          </div>
        </div>
      </div>
    </div>
  );
}
