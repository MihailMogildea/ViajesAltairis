import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { Breadcrumb } from "@/components/breadcrumb";
import { CountriesTable } from "./countries-table";
import type { CountryDto, CurrencyDto } from "@/types/system";

export default async function CountriesPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let countries: CountryDto[] = [];
  let currencies: CurrencyDto[] = [];
  let error: string | null = null;

  try {
    [countries, currencies] = await Promise.all([
      apiFetch<CountryDto[]>("/api/Countries", { cache: "no-store" }),
      apiFetch<CurrencyDto[]>("/api/Currencies", { cache: "no-store" }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load data";
  }

  return (
    <div>
      <Breadcrumb
        items={[
          { label: t["admin.section.system"] ?? "System Config", href: "/system" },
          { label: t["admin.system.countries"] ?? "Countries" },
        ]}
      />
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">
          {t["admin.system.countries"] ?? "Countries"}
        </h2>
        <p className="mt-1 text-sm text-gray-500">
          {t["admin.system.countries.desc"] ?? "Manage countries and their currency assignments."}
        </p>
      </div>

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <CountriesTable countries={countries} currencies={currencies} t={t} />
      )}
    </div>
  );
}
