import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { apiFetch } from "@/lib/api";
import { Breadcrumb } from "@/components/breadcrumb";
import { ReservationsTable } from "./reservations-table";
import type { ReservationAdminDto, ReservationStatusDto } from "@/types/reservation";
import type { UserOption } from "./actions";
import type { TranslationDto, LanguageDto } from "@/types/system";

export default async function ReservationsPage() {
  const session = await getSession();
  const access = session ? getAccessLevel(session.role, "reservations") : null;
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let reservations: ReservationAdminDto[] = [];
  let statuses: ReservationStatusDto[] = [];
  let users: UserOption[] = [];
  let translations: TranslationDto[] = [];
  let languages: LanguageDto[] = [];
  let error: string | null = null;

  try {
    [reservations, statuses, translations, languages] = await Promise.all([
      apiFetch<ReservationAdminDto[]>("/api/Reservations", { cache: "no-store" }),
      apiFetch<ReservationStatusDto[]>("/api/ReservationStatuses", { cache: "no-store" }),
      apiFetch<TranslationDto[]>("/api/Translations", { cache: "no-store" }),
      apiFetch<LanguageDto[]>("/api/Languages", { cache: "no-store" }),
    ]);
    // Users list is used for filtering — B2B agents can't access /api/Users, so fetch separately
    try {
      users = await apiFetch<UserOption[]>("/api/Users", { cache: "no-store" });
    } catch {
      // B2B agents get an empty user list (no user filter needed)
    }
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load reservations";
  }

  const langId = languages.find((l) => l.isoCode === locale)?.id ?? 1;
  const rsNames: Record<number, string> = {};
  for (const tr of translations) {
    if (tr.entityType === "reservation_status" && tr.field === "name" && tr.languageId === langId) {
      rsNames[tr.entityId] = tr.value;
    }
  }

  return (
    <div>
      <Breadcrumb
        items={[
          { label: t["admin.section.reservations"] ?? "Reservations" },
        ]}
      />
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">
          {t["admin.section.reservations"] ?? "Reservations"}
        </h2>
        <p className="mt-1 text-sm text-gray-500">
          {access === "own"
            ? t["admin.label.showing_own"] ?? "Showing records scoped to your access."
            : t["admin.label.showing_all"] ?? "Showing all records."}
        </p>
      </div>

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <ReservationsTable
          reservations={reservations}
          statuses={statuses}
          users={users}
          rsNames={rsNames}
          access={access}
          t={t}
        />
      )}
    </div>
  );
}
