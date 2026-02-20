import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { apiFetch } from "@/lib/api";
import { Breadcrumb } from "@/components/breadcrumb";
import { ReservationsTable } from "./reservations-table";
import type { ReservationAdminDto, ReservationStatusDto } from "@/types/reservation";

export default async function ReservationsPage() {
  const session = await getSession();
  const access = session ? getAccessLevel(session.role, "reservations") : null;
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let reservations: ReservationAdminDto[] = [];
  let statuses: ReservationStatusDto[] = [];
  let error: string | null = null;

  try {
    [reservations, statuses] = await Promise.all([
      apiFetch<ReservationAdminDto[]>("/api/Reservations", { cache: "no-store" }),
      apiFetch<ReservationStatusDto[]>("/api/ReservationStatuses", { cache: "no-store" }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load reservations";
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
          access={access}
          t={t}
        />
      )}
    </div>
  );
}
