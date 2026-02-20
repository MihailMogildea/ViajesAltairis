import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { Breadcrumb } from "@/components/breadcrumb";
import {
  TaxDto,
  TaxTypeDto,
  CountryDto,
  AdministrativeDivisionDto,
  CityDto,
} from "@/types/system";
import { TaxesTabs } from "./taxes-tabs";

export default async function TaxesPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let taxes: TaxDto[] = [];
  let taxTypes: TaxTypeDto[] = [];
  let countries: CountryDto[] = [];
  let divisions: AdministrativeDivisionDto[] = [];
  let cities: CityDto[] = [];
  let error: string | null = null;

  try {
    [taxes, taxTypes, countries, divisions, cities] = await Promise.all([
      apiFetch<TaxDto[]>("/api/Taxes", { cache: "no-store" }),
      apiFetch<TaxTypeDto[]>("/api/TaxTypes", { cache: "no-store" }),
      apiFetch<CountryDto[]>("/api/Countries", { cache: "no-store" }),
      apiFetch<AdministrativeDivisionDto[]>("/api/AdministrativeDivisions", { cache: "no-store" }),
      apiFetch<CityDto[]>("/api/Cities", { cache: "no-store" }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load tax data";
  }

  return (
    <div>
      <Breadcrumb
        items={[
          { label: t["admin.section.system"] ?? "System Config", href: "/system" },
          { label: t["admin.system.taxes"] ?? "Tax Configuration" },
        ]}
      />

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <TaxesTabs
          taxes={taxes}
          taxTypes={taxTypes}
          countries={countries}
          divisions={divisions}
          cities={cities}
          t={t}
        />
      )}
    </div>
  );
}
