import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { Breadcrumb } from "@/components/breadcrumb";
import { ProviderDetail } from "./provider-detail";
import type { ProviderDto, ProviderTypeDto } from "@/types/provider";
import type {
  HotelDto,
  HotelProviderDto,
  HotelProviderRoomTypeDto,
  HotelProviderRoomTypeBoardDto,
} from "@/types/hotel";
import type { CurrencyDto, RoomTypeDto, BoardTypeDto, ExchangeRateDto, TranslationDto, LanguageDto } from "@/types/system";

interface Props {
  params: Promise<{ id: string }>;
}

export default async function ProviderDetailPage({ params }: Props) {
  const { id } = await params;
  const providerId = Number(id);
  const session = await getSession();
  const access = session
    ? getAccessLevel(session.role, "providers")
    : null;
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let provider: ProviderDto | null = null;
  let providerTypes: ProviderTypeDto[] = [];
  let currencies: CurrencyDto[] = [];
  let hotels: HotelDto[] = [];
  let hotelProviders: HotelProviderDto[] = [];
  let roomTypes: RoomTypeDto[] = [];
  let boardTypes: BoardTypeDto[] = [];
  let hpRoomTypes: HotelProviderRoomTypeDto[] = [];
  let hpRoomTypeBoards: HotelProviderRoomTypeBoardDto[] = [];
  let exchangeRates: ExchangeRateDto[] = [];
  let translations: TranslationDto[] = [];
  let languages: LanguageDto[] = [];
  let error: string | null = null;

  try {
    [
      provider,
      providerTypes,
      currencies,
      hotels,
      hotelProviders,
      roomTypes,
      boardTypes,
      hpRoomTypes,
      hpRoomTypeBoards,
      exchangeRates,
      translations,
      languages,
    ] = await Promise.all([
      apiFetch<ProviderDto>(`/api/Providers/${providerId}`, { cache: "no-store" }),
      apiFetch<ProviderTypeDto[]>("/api/ProviderTypes", { cache: "no-store" }),
      apiFetch<CurrencyDto[]>("/api/Currencies", { cache: "no-store" }),
      apiFetch<HotelDto[]>("/api/Hotels", { cache: "no-store" }),
      apiFetch<HotelProviderDto[]>("/api/HotelProviders", { cache: "no-store" }),
      apiFetch<RoomTypeDto[]>("/api/RoomTypes", { cache: "no-store" }),
      apiFetch<BoardTypeDto[]>("/api/BoardTypes", { cache: "no-store" }),
      apiFetch<HotelProviderRoomTypeDto[]>("/api/HotelProviderRoomTypes", { cache: "no-store" }),
      apiFetch<HotelProviderRoomTypeBoardDto[]>("/api/HotelProviderRoomTypeBoards", { cache: "no-store" }),
      apiFetch<ExchangeRateDto[]>("/api/ExchangeRates", { cache: "no-store" }),
      apiFetch<TranslationDto[]>("/api/Translations", { cache: "no-store" }),
      apiFetch<LanguageDto[]>("/api/Languages", { cache: "no-store" }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load provider details";
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

  return (
    <div>
      <Breadcrumb
        items={[
          { label: t["admin.section.providers"] ?? "Providers", href: "/providers" },
          { label: provider?.name ?? `Provider #${providerId}` },
        ]}
      />

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : provider ? (
        <ProviderDetail
          provider={provider}
          providerTypes={providerTypes}
          currencies={currencies}
          hotels={hotels}
          hotelProviders={hotelProviders.filter((hp) => hp.providerId === providerId)}
          roomTypes={roomTypes}
          boardTypes={boardTypes}
          hpRoomTypes={hpRoomTypes}
          hpRoomTypeBoards={hpRoomTypeBoards}
          exchangeRates={exchangeRates}
          rtNames={rtNames}
          btNames={btNames}
          t={t}
          access={access}
        />
      ) : (
        <p className="text-sm text-red-600">
          {t["admin.providers.not_found"] ?? "Provider not found."}
        </p>
      )}
    </div>
  );
}
