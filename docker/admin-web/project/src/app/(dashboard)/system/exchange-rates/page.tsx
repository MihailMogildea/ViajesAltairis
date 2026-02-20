import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { Breadcrumb } from "@/components/breadcrumb";
import { ExchangeRatesTable } from "./exchange-rates-table";
import type { ExchangeRateDto, CurrencyDto } from "@/types/system";

export default async function ExchangeRatesPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let rates: ExchangeRateDto[] = [];
  let currencies: CurrencyDto[] = [];
  let error: string | null = null;

  try {
    [rates, currencies] = await Promise.all([
      apiFetch<ExchangeRateDto[]>("/api/ExchangeRates", { cache: "no-store" }),
      apiFetch<CurrencyDto[]>("/api/Currencies", { cache: "no-store" }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load exchange rates";
  }

  return (
    <div>
      <Breadcrumb
        items={[
          { label: t["admin.section.system"] ?? "System Config", href: "/system" },
          { label: t["admin.system.exchange_rates"] ?? "Exchange Rates" },
        ]}
      />
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">
          {t["admin.system.exchange_rates"] ?? "Exchange Rates"}
        </h2>
        <p className="mt-1 text-sm text-gray-500">
          {t["admin.exchange_rates.desc"] ?? "Manage currency exchange rates to EUR."}
        </p>
      </div>

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <ExchangeRatesTable rates={rates} currencies={currencies} t={t} />
      )}
    </div>
  );
}
