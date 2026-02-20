import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { Breadcrumb } from "@/components/breadcrumb";
import { AdminDivisionTypesTable } from "./admin-division-types-table";
import type { AdministrativeDivisionTypeDto } from "@/types/system";

export default async function AdminDivisionTypesPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let types: AdministrativeDivisionTypeDto[] = [];
  let error: string | null = null;

  try {
    types = await apiFetch<AdministrativeDivisionTypeDto[]>(
      "/api/AdministrativeDivisionTypes",
      { cache: "no-store" }
    );
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load division types";
  }

  return (
    <div>
      <Breadcrumb
        items={[
          { label: t["admin.section.system"] ?? "System Config", href: "/system" },
          { label: t["admin.system.admin_division_types"] ?? "Division Types" },
        ]}
      />
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">
          {t["admin.system.admin_division_types"] ?? "Administrative Division Types"}
        </h2>
        <p className="mt-1 text-sm text-gray-500">
          {t["admin.admin_division_types.desc"] ?? "Manage types of administrative divisions (state, province, region, etc.)."}
        </p>
      </div>

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <AdminDivisionTypesTable types={types} t={t} />
      )}
    </div>
  );
}
