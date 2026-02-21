import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { apiFetch } from "@/lib/api";
import { Breadcrumb } from "@/components/breadcrumb";
import { ReservationDetail } from "./reservation-detail";
import type { ReservationAdminDto, ReservationStatusDto, ReservationLineAdminDto, ReservationGuestAdminDto } from "@/types/reservation";
import type { HotelOption, RoomConfigOption, BoardOption, PaymentMethodOption, ProviderOption, HotelProviderOption, RoomTypeOption, BoardTypeOption } from "../actions";
import type { TranslationDto, LanguageDto } from "@/types/system";

export default async function ReservationDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const session = await getSession();
  const access = session ? getAccessLevel(session.role, "reservations") : null;
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let reservation: ReservationAdminDto | null = null;
  let statuses: ReservationStatusDto[] = [];
  let lines: ReservationLineAdminDto[] = [];
  let guests: ReservationGuestAdminDto[] = [];
  let hotels: HotelOption[] = [];
  let roomConfigs: RoomConfigOption[] = [];
  let boardOptions: BoardOption[] = [];
  let paymentMethods: PaymentMethodOption[] = [];
  let providers: ProviderOption[] = [];
  let hotelProviders: HotelProviderOption[] = [];
  let roomTypes: RoomTypeOption[] = [];
  let boardTypes: BoardTypeOption[] = [];
  let translations: TranslationDto[] = [];
  let languages: LanguageDto[] = [];
  let error: string | null = null;

  try {
    [reservation, statuses, lines, guests, hotels, roomConfigs, boardOptions, paymentMethods, providers, hotelProviders, roomTypes, boardTypes, translations, languages] = await Promise.all([
      apiFetch<ReservationAdminDto>(`/api/Reservations/${id}`, { cache: "no-store" }),
      apiFetch<ReservationStatusDto[]>("/api/ReservationStatuses", { cache: "no-store" }),
      apiFetch<ReservationLineAdminDto[]>(`/api/Reservations/${id}/lines`, { cache: "no-store" }),
      apiFetch<ReservationGuestAdminDto[]>(`/api/Reservations/${id}/guests`, { cache: "no-store" }),
      apiFetch<HotelOption[]>("/api/Hotels", { cache: "no-store" }),
      apiFetch<RoomConfigOption[]>("/api/HotelProviderRoomTypes", { cache: "no-store" }),
      apiFetch<BoardOption[]>("/api/HotelProviderRoomTypeBoards", { cache: "no-store" }),
      apiFetch<PaymentMethodOption[]>("/api/PaymentMethods", { cache: "no-store" }),
      apiFetch<ProviderOption[]>("/api/Providers", { cache: "no-store" }),
      apiFetch<HotelProviderOption[]>("/api/HotelProviders", { cache: "no-store" }),
      apiFetch<RoomTypeOption[]>("/api/RoomTypes", { cache: "no-store" }),
      apiFetch<BoardTypeOption[]>("/api/BoardTypes", { cache: "no-store" }),
      apiFetch<TranslationDto[]>("/api/Translations", { cache: "no-store" }),
      apiFetch<LanguageDto[]>("/api/Languages", { cache: "no-store" }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load reservation";
  }

  const langId = languages.find((l) => l.isoCode === locale)?.id ?? 1;
  const pmNames: Record<number, string> = {};
  const rsNames: Record<number, string> = {};
  const rtNames: Record<number, string> = {};
  const btNames: Record<number, string> = {};
  for (const tr of translations) {
    if (tr.field === "name" && tr.languageId === langId) {
      if (tr.entityType === "payment_method") {
        pmNames[tr.entityId] = tr.value;
      } else if (tr.entityType === "reservation_status") {
        rsNames[tr.entityId] = tr.value;
      } else if (tr.entityType === "room_type") {
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
          { label: t["admin.section.reservations"] ?? "Reservations", href: "/reservations" },
          { label: reservation?.reservationCode ?? `#${id}` },
        ]}
      />

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : reservation ? (
        <ReservationDetail
          reservation={reservation}
          statuses={statuses}
          initialLines={lines}
          initialGuests={guests}
          hotels={hotels}
          roomConfigs={roomConfigs}
          boardOptions={boardOptions}
          paymentMethods={paymentMethods}
          providers={providers}
          hotelProviders={hotelProviders}
          roomTypes={roomTypes}
          boardTypes={boardTypes}
          pmNames={pmNames}
          rsNames={rsNames}
          rtNames={rtNames}
          btNames={btNames}
          access={access}
          t={t}
        />
      ) : (
        <p className="text-sm text-gray-500">
          {t["admin.reservations.not_found"] ?? "Reservation not found."}
        </p>
      )}
    </div>
  );
}
