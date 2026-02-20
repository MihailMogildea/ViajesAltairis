import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { Breadcrumb } from "@/components/breadcrumb";
import { CitiesTable } from "./cities-table";
import type { CityDto, AdministrativeDivisionDto } from "@/types/system";

export default async function CitiesPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let cities: CityDto[] = [];
  let divisions: AdministrativeDivisionDto[] = [];
  let error: string | null = null;

  try {
    [cities, divisions] = await Promise.all([
      apiFetch<CityDto[]>("/api/Cities", { cache: "no-store" }),
      apiFetch<AdministrativeDivisionDto[]>("/api/AdministrativeDivisions", {
        cache: "no-store",
      }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load cities";
  }

  return (
    <div>
      <Breadcrumb
        items={[
          { label: t["admin.section.system"] ?? "System Config", href: "/system" },
          { label: t["admin.system.cities"] ?? "Cities" },
        ]}
      />
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">
          {t["admin.system.cities"] ?? "Cities"}
        </h2>
        <p className="mt-1 text-sm text-gray-500">
          {t["admin.cities.desc"] ?? "Manage cities and their administrative division assignments."}
        </p>
      </div>

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <CitiesTable cities={cities} divisions={divisions} t={t} />
      )}
    </div>
  );
}
