"use client";

import { useState, useMemo, useEffect, useCallback } from "react";
import { useSearchParams } from "next/navigation";
import { Suspense } from "react";
import { hotels } from "@/data/hotels";
import { regions } from "@/data/areas";
import { filterHotels, sortHotels, paginate } from "@/lib/utils";
import { apiSearchHotels } from "@/lib/api";
import SearchBar from "@/components/SearchBar";
import FilterSidebar from "@/components/FilterSidebar";
import HotelCard from "@/components/HotelCard";
import Pagination from "@/components/Pagination";
import { useLocale } from "@/context/LocaleContext";
import type { Hotel, ApiHotelSummary } from "@/types";

function mapApiHotelToHotel(h: ApiHotelSummary): Hotel {
  return {
    id: h.id,
    name: h.name,
    city_id: 0,
    city_name: h.city,
    region_name: "",
    country_name: h.country,
    stars: h.stars,
    latitude: 0,
    longitude: 0,
    check_in_time: "14:00",
    check_out_time: "12:00",
    phone: "",
    email: "",
    website: "",
    margin: 0,
    enabled: true,
    summary: "",
    description: "",
    images: h.mainImageUrl
      ? [{ id: 1, hotel_id: h.id, url: h.mainImageUrl, alt_text: h.name, sort_order: 1 }]
      : [],
    amenities: [],
    min_price: h.priceFrom ?? 0,
    avg_rating: 0,
    review_count: 0,
  };
}

/** Try to resolve a destination string to a cityId or countryId for the API */
function resolveDestination(dest: string): { cityId?: number; countryId?: number } {
  if (!dest) return {};
  const lower = dest.toLowerCase();

  // Check exact city match first
  for (const region of regions) {
    for (const city of region.cities) {
      if (city.name.toLowerCase() === lower) {
        return { cityId: city.id };
      }
    }
  }

  // Check region names — resolve to country level (region spans multiple cities)
  for (const region of regions) {
    if (region.name.toLowerCase() === lower) {
      const countryMap: Record<string, number> = { spain: 1, france: 2 };
      const countryId = countryMap[region.country.toLowerCase()];
      if (countryId) return { countryId };
      // Fallback: first city in region
      return region.cities[0] ? { cityId: region.cities[0].id } : {};
    }
  }

  // Check partial city match
  for (const region of regions) {
    for (const city of region.cities) {
      if (city.name.toLowerCase().includes(lower) || lower.includes(city.name.toLowerCase())) {
        return { cityId: city.id };
      }
    }
  }

  // Check country
  const countryMap: Record<string, number> = { spain: 1, españa: 1, france: 2, francia: 2 };
  if (countryMap[lower]) return { countryId: countryMap[lower] };

  return {};
}

function HotelsContent() {
  const searchParams = useSearchParams();
  const { t } = useLocale();

  const [destination, setDestination] = useState(searchParams.get("destination") || "");
  const [checkIn, setCheckIn] = useState(searchParams.get("checkIn") || "");
  const [checkOut, setCheckOut] = useState(searchParams.get("checkOut") || "");
  const [guests, setGuests] = useState(Number(searchParams.get("guests")) || 2);

  const [stars, setStars] = useState<number[]>([]);
  const [minPrice, setMinPrice] = useState(0);
  const [maxPrice, setMaxPrice] = useState(0);
  const [selectedAmenities, setSelectedAmenities] = useState<number[]>([]);
  const [sortBy, setSortBy] = useState("rating");
  const [page, setPage] = useState(1);
  const [showFilters, setShowFilters] = useState(false);

  const [apiHotels, setApiHotels] = useState<Hotel[] | null>(null);
  const [apiTotal, setApiTotal] = useState(0);

  const perPage = 6;

  // Try API search whenever search params change
  const fetchFromApi = useCallback(async () => {
    try {
      const resolved = resolveDestination(destination);
      const res = await apiSearchHotels({
        ...resolved,
        checkIn: checkIn || undefined,
        checkOut: checkOut || undefined,
        guests: guests || undefined,
        stars: stars.length > 0 ? stars : undefined,
        page,
        pageSize: perPage,
      });
      setApiHotels(res.hotels.map(mapApiHotelToHotel));
      setApiTotal(res.totalCount);
    } catch {
      // API unavailable — will fall back to mock data
      setApiHotels(null);
    }
  }, [destination, checkIn, checkOut, guests, stars, page]);

  useEffect(() => {
    fetchFromApi();
  }, [fetchFromApi]);

  // Mock data fallback (used when API is unavailable)
  const filtered = useMemo(() => {
    const f = filterHotels(hotels, {
      destination,
      stars: stars.length > 0 ? stars : undefined,
      minPrice: minPrice || undefined,
      maxPrice: maxPrice || undefined,
      amenities: selectedAmenities.length > 0 ? selectedAmenities : undefined,
    });
    return sortHotels(f, sortBy);
  }, [destination, stars, minPrice, maxPrice, selectedAmenities, sortBy]);

  // Use API data if available, otherwise fall back to mock
  const useApi = apiHotels !== null;
  const displayHotels = useApi ? apiHotels : paginate(filtered, page, perPage).items;
  const total = useApi ? apiTotal : filtered.length;
  const totalPages = Math.ceil(total / perPage);

  function handleSearch(dest: string, ci: string, co: string, g: number) {
    setDestination(dest);
    setCheckIn(ci);
    setCheckOut(co);
    setGuests(g);
    setPage(1);
  }

  function resetFilters() {
    setStars([]);
    setMinPrice(0);
    setMaxPrice(0);
    setSelectedAmenities([]);
    setPage(1);
  }

  return (
    <div className="mx-auto max-w-7xl px-4 py-6 sm:px-6">
      {/* Search bar */}
      <div className="mb-6 rounded-xl border border-gray-200 bg-white p-4">
        <SearchBar
          initialDestination={destination}
          initialCheckIn={checkIn}
          initialCheckOut={checkOut}
          initialGuests={guests}
          onSearch={handleSearch}
          compact
        />
      </div>

      <div className="flex items-center justify-between mb-4">
        <p className="text-sm text-gray-500">
          {total} {total === 1 ? t("client.hotels.hotel") : t("client.hotels.hotels")} {t("client.hotels.found")}
          {destination && <> {t("client.hotels.for")} <span className="font-medium text-gray-700">&ldquo;{destination}&rdquo;</span></>}
        </p>
        <div className="flex items-center gap-3">
          <button
            onClick={() => setShowFilters(!showFilters)}
            className="rounded-lg border border-gray-300 px-3 py-1.5 text-sm font-medium text-gray-700 hover:bg-gray-50 lg:hidden"
          >
            {t("client.hotels.filters")}
          </button>
          <select
            value={sortBy}
            onChange={(e) => { setSortBy(e.target.value); setPage(1); }}
            className="rounded-lg border border-gray-300 px-3 py-1.5 text-sm"
          >
            <option value="rating">{t("client.hotels.top_rated")}</option>
            <option value="price_asc">{t("client.hotels.price_low")}</option>
            <option value="price_desc">{t("client.hotels.price_high")}</option>
            <option value="stars">{t("client.hotels.star_rating")}</option>
          </select>
        </div>
      </div>

      <div className="flex gap-6">
        {/* Sidebar - desktop */}
        <div className="hidden w-56 shrink-0 lg:block">
          <FilterSidebar
            stars={stars} onStarsChange={(s) => { setStars(s); setPage(1); }}
            minPrice={minPrice} maxPrice={maxPrice}
            onMinPriceChange={(v) => { setMinPrice(v); setPage(1); }}
            onMaxPriceChange={(v) => { setMaxPrice(v); setPage(1); }}
            selectedAmenities={selectedAmenities}
            onAmenitiesChange={(a) => { setSelectedAmenities(a); setPage(1); }}
            onReset={resetFilters}
          />
        </div>

        {/* Mobile filter drawer */}
        {showFilters && (
          <div className="fixed inset-0 z-40 flex lg:hidden">
            <div className="fixed inset-0 bg-black/30" onClick={() => setShowFilters(false)} />
            <div className="relative z-50 ml-auto w-72 overflow-y-auto bg-white p-6 shadow-xl">
              <div className="mb-4 flex items-center justify-between">
                <h3 className="font-semibold">{t("client.hotels.filters")}</h3>
                <button onClick={() => setShowFilters(false)} className="text-gray-400 hover:text-gray-600">
                  <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                    <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              </div>
              <FilterSidebar
                stars={stars} onStarsChange={(s) => { setStars(s); setPage(1); }}
                minPrice={minPrice} maxPrice={maxPrice}
                onMinPriceChange={(v) => { setMinPrice(v); setPage(1); }}
                onMaxPriceChange={(v) => { setMaxPrice(v); setPage(1); }}
                selectedAmenities={selectedAmenities}
                onAmenitiesChange={(a) => { setSelectedAmenities(a); setPage(1); }}
                onReset={resetFilters}
              />
            </div>
          </div>
        )}

        {/* Hotel grid */}
        <div className="flex-1">
          {displayHotels.length === 0 ? (
            <div className="rounded-xl border border-gray-200 bg-gray-50 py-16 text-center">
              <p className="text-lg font-medium text-gray-600">{t("client.hotels.no_match")}</p>
              <p className="mt-1 text-sm text-gray-400">{t("client.hotels.try_adjusting")}</p>
              <button onClick={resetFilters} className="mt-4 text-sm text-blue-600 hover:underline">{t("client.hotels.reset_all")}</button>
            </div>
          ) : (
            <div className="grid gap-6 sm:grid-cols-2 xl:grid-cols-3">
              {displayHotels.map((hotel) => (
                <HotelCard key={hotel.id} hotel={hotel} />
              ))}
            </div>
          )}
          <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </div>
      </div>
    </div>
  );
}

export default function HotelsPage() {
  return (
    <Suspense fallback={<div className="mx-auto max-w-7xl px-4 py-12 text-center text-gray-400">{/* Loading */}</div>}>
      <HotelsContent />
    </Suspense>
  );
}
