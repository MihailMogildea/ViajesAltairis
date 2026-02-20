import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { HotelsTable } from "./hotels-table";
import type { HotelDto } from "@/types/hotel";
import type { CityDto } from "@/types/system";

export default async function HotelsPage() {
  const session = await getSession();
  const access = session ? getAccessLevel(session.role, "hotels") : null;
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let hotels: HotelDto[] = [];
  let cities: CityDto[] = [];
  let error: string | null = null;

  try {
    [hotels, cities] = await Promise.all([
      apiFetch<HotelDto[]>("/api/Hotels", { cache: "no-store" }),
      apiFetch<CityDto[]>("/api/Cities", { cache: "no-store" }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load hotels";
  }

  return (
    <div>
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">
          {t["admin.section.hotels"] ?? "Hotels"}
        </h2>
        <p className="mt-1 text-sm text-gray-500">
          {t["admin.hotels.desc"] ?? "Manage hotels, images, amenities, and room configurations."}
        </p>
      </div>

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <HotelsTable hotels={hotels} cities={cities} t={t} access={access} />
      )}
    </div>
  );
}
