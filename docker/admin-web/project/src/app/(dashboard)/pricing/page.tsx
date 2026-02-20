import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { PricingTabs } from "./pricing-tabs";
import type { SeasonalMarginDto, PromoCodeDto } from "@/types/pricing";
import type { AdministrativeDivisionDto, CurrencyDto } from "@/types/system";

export default async function PricingPage() {
  const session = await getSession();
  const access = session ? getAccessLevel(session.role, "pricing") : null;
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let seasonalMargins: SeasonalMarginDto[] = [];
  let promoCodes: PromoCodeDto[] = [];
  let divisions: AdministrativeDivisionDto[] = [];
  let currencies: CurrencyDto[] = [];
  let error: string | null = null;

  try {
    [seasonalMargins, promoCodes, divisions, currencies] = await Promise.all([
      apiFetch<SeasonalMarginDto[]>("/api/SeasonalMargins", { cache: "no-store" }),
      apiFetch<PromoCodeDto[]>("/api/PromoCodes", { cache: "no-store" }),
      apiFetch<AdministrativeDivisionDto[]>("/api/AdministrativeDivisions", { cache: "no-store" }),
      apiFetch<CurrencyDto[]>("/api/Currencies", { cache: "no-store" }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load pricing data";
  }

  return (
    <div>
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">
          {t["admin.section.pricing"] ?? "Pricing & Margins"}
        </h2>
        <p className="mt-1 text-sm text-gray-500">
          {t["admin.pricing.desc"] ?? "Manage seasonal margins and promo codes."}
        </p>
      </div>

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <PricingTabs
          seasonalMargins={seasonalMargins}
          promoCodes={promoCodes}
          divisions={divisions}
          currencies={currencies}
          access={access}
          t={t}
        />
      )}
    </div>
  );
}
