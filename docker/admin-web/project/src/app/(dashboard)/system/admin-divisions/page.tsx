import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { Breadcrumb } from "@/components/breadcrumb";
import { AdminDivisionsTable } from "./admin-divisions-table";
import type {
  AdministrativeDivisionDto,
  AdministrativeDivisionTypeDto,
  CountryDto,
} from "@/types/system";

export default async function AdminDivisionsPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let divisions: AdministrativeDivisionDto[] = [];
  let countries: CountryDto[] = [];
  let divisionTypes: AdministrativeDivisionTypeDto[] = [];
  let error: string | null = null;

  try {
    [divisions, countries, divisionTypes] = await Promise.all([
      apiFetch<AdministrativeDivisionDto[]>("/api/AdministrativeDivisions", {
        cache: "no-store",
      }),
      apiFetch<CountryDto[]>("/api/Countries", { cache: "no-store" }),
      apiFetch<AdministrativeDivisionTypeDto[]>("/api/AdministrativeDivisionTypes", {
        cache: "no-store",
      }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load divisions";
  }

  return (
    <div>
      <Breadcrumb
        items={[
          { label: t["admin.section.system"] ?? "System Config", href: "/system" },
          { label: t["admin.system.admin_divisions"] ?? "Admin Divisions" },
        ]}
      />
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">
          {t["admin.system.admin_divisions"] ?? "Administrative Divisions"}
        </h2>
        <p className="mt-1 text-sm text-gray-500">
          {t["admin.admin_divisions.desc"] ?? "Manage administrative divisions (states, provinces, regions)."}
        </p>
      </div>

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <AdminDivisionsTable
          divisions={divisions}
          countries={countries}
          divisionTypes={divisionTypes}
          t={t}
        />
      )}
    </div>
  );
}
