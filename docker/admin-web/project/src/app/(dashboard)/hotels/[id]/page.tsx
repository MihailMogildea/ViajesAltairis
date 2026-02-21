import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { Breadcrumb } from "@/components/breadcrumb";
import { HotelDetail } from "./hotel-detail";
import type {
  HotelDto,
  HotelImageDto,
  HotelAmenityDto,
  HotelProviderDto,
  HotelProviderRoomTypeDto,
  HotelProviderRoomTypeBoardDto,
} from "@/types/hotel";
import type { ProviderDto } from "@/types/provider";
import type {
  AmenityDto,
  AmenityCategoryDto,
  CityDto,
  RoomTypeDto,
  BoardTypeDto,
  CurrencyDto,
  ExchangeRateDto,
  TranslationDto,
  LanguageDto,
} from "@/types/system";

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
  let hotelProviders: HotelProviderDto[] = [];
  let hpRoomTypes: HotelProviderRoomTypeDto[] = [];
  let hpRoomTypeBoards: HotelProviderRoomTypeBoardDto[] = [];
  let providers: ProviderDto[] = [];
  let roomTypes: RoomTypeDto[] = [];
  let boardTypes: BoardTypeDto[] = [];
  let currencies: CurrencyDto[] = [];
  let exchangeRates: ExchangeRateDto[] = [];
  let translations: TranslationDto[] = [];
  let languages: LanguageDto[] = [];
  let error: string | null = null;

  try {
    [
      hotel,
      images,
      hotelAmenities,
      amenities,
      amenityCategories,
      cities,
      hotelProviders,
      hpRoomTypes,
      hpRoomTypeBoards,
      providers,
      roomTypes,
      boardTypes,
      currencies,
      exchangeRates,
      translations,
      languages,
    ] = await Promise.all([
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
      apiFetch<HotelProviderDto[]>("/api/HotelProviders", {
        cache: "no-store",
      }),
      apiFetch<HotelProviderRoomTypeDto[]>("/api/HotelProviderRoomTypes", {
        cache: "no-store",
      }),
      apiFetch<HotelProviderRoomTypeBoardDto[]>(
        "/api/HotelProviderRoomTypeBoards",
        { cache: "no-store" }
      ),
      apiFetch<ProviderDto[]>("/api/Providers", { cache: "no-store" }),
      apiFetch<RoomTypeDto[]>("/api/RoomTypes", { cache: "no-store" }),
      apiFetch<BoardTypeDto[]>("/api/BoardTypes", { cache: "no-store" }),
      apiFetch<CurrencyDto[]>("/api/Currencies", { cache: "no-store" }),
      apiFetch<ExchangeRateDto[]>("/api/ExchangeRates", { cache: "no-store" }),
      apiFetch<TranslationDto[]>("/api/Translations", { cache: "no-store" }),
      apiFetch<LanguageDto[]>("/api/Languages", { cache: "no-store" }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load hotel details";
  }

  const langId = languages.find((l) => l.isoCode === locale)?.id ?? 1;
  const rtNames: Record<number, string> = {};
  const btNames: Record<number, string> = {};
  for (const tr of translations) {
    if (tr.field === "name" && tr.languageId === langId) {
      if (tr.entityType === "room_type") {
        rtNames[tr.entityId] = tr.value;
      } else if (tr.entityType === "board_type") {
        btNames[tr.entityId] = tr.value;
      }
    }
  }

  const hotelImages = images.filter((img) => img.hotelId === Number(id));
  const filteredHotelProviders = hotelProviders.filter((hp) => hp.hotelId === Number(id));

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
          hotelProviders={filteredHotelProviders}
          hpRoomTypes={hpRoomTypes}
          hpRoomTypeBoards={hpRoomTypeBoards}
          providers={providers}
          roomTypes={roomTypes}
          boardTypes={boardTypes}
          currencies={currencies}
          exchangeRates={exchangeRates}
          rtNames={rtNames}
          btNames={btNames}
          t={t}
          access={access}
        />
      )}
    </div>
  );
}
