import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { Breadcrumb } from "@/components/breadcrumb";
import { HotelDetail } from "./hotel-detail";
import type { HotelDto, HotelImageDto, HotelAmenityDto } from "@/types/hotel";
import type { AmenityDto, AmenityCategoryDto, CityDto } from "@/types/system";

interface PageProps {
  params: Promise<{ id: string }>;
}

export default async function HotelDetailPage({ params }: PageProps) {
  const { id } = await params;
  const session = await getSession();
  const access = session ? getAccessLevel(session.role, "hotels") : null;
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let hotel: HotelDto | null = null;
  let images: HotelImageDto[] = [];
  let hotelAmenities: HotelAmenityDto[] = [];
  let amenities: AmenityDto[] = [];
  let amenityCategories: AmenityCategoryDto[] = [];
  let cities: CityDto[] = [];
  let error: string | null = null;

  try {
    [hotel, images, hotelAmenities, amenities, amenityCategories, cities] =
      await Promise.all([
        apiFetch<HotelDto>(`/api/Hotels/${id}`, { cache: "no-store" }),
        apiFetch<HotelImageDto[]>("/api/HotelImages", { cache: "no-store" }),
        apiFetch<HotelAmenityDto[]>(`/api/HotelAmenities?hotelId=${id}`, {
          cache: "no-store",
        }),
        apiFetch<AmenityDto[]>("/api/Amenities", { cache: "no-store" }),
        apiFetch<AmenityCategoryDto[]>("/api/AmenityCategories", {
          cache: "no-store",
        }),
        apiFetch<CityDto[]>("/api/Cities", { cache: "no-store" }),
      ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load hotel details";
  }

  const hotelImages = images.filter((img) => img.hotelId === Number(id));

  return (
    <div>
      <Breadcrumb
        items={[
          { label: t["admin.section.hotels"] ?? "Hotels", href: "/hotels" },
          { label: hotel?.name ?? `Hotel #${id}` },
        ]}
      />

      {error || !hotel ? (
        <p className="text-sm text-red-600">
          {error ?? "Hotel not found."}
        </p>
      ) : (
        <HotelDetail
          hotel={hotel}
          images={hotelImages}
          hotelAmenities={hotelAmenities}
          amenities={amenities}
          amenityCategories={amenityCategories}
          cities={cities}
          t={t}
          access={access}
        />
      )}
    </div>
  );
}
