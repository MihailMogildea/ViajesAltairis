import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { Breadcrumb } from "@/components/breadcrumb";
import { CurrenciesTable } from "./currencies-table";
import type { CurrencyDto } from "@/types/system";

export default async function CurrenciesPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let currencies: CurrencyDto[] = [];
  let error: string | null = null;

  try {
    currencies = await apiFetch<CurrencyDto[]>("/api/Currencies", {
      cache: "no-store",
    });
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load currencies";
  }

  return (
    <div>
      <Breadcrumb
        items={[
          { label: t["admin.section.system"] ?? "System Config", href: "/system" },
          { label: t["admin.system.currencies"] ?? "Currencies" },
        ]}
      />
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">
          {t["admin.system.currencies"] ?? "Currencies"}
        </h2>
        <p className="mt-1 text-sm text-gray-500">
          {t["admin.system.currencies.desc"] ?? "Manage supported currencies for the platform."}
        </p>
      </div>

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <CurrenciesTable currencies={currencies} t={t} />
      )}
    </div>
  );
}
