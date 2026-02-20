"use client";

import { useState } from "react";
import { TabBar } from "@/components/tab-bar";
import { SeasonalMarginsTab } from "./seasonal-margins-tab";
import { PromoCodesTab } from "./promo-codes-tab";
import type { SeasonalMarginDto, PromoCodeDto } from "@/types/pricing";
import type { AdministrativeDivisionDto, CurrencyDto } from "@/types/system";
import type { AccessLevel } from "@/lib/permissions";

const TABS = [
  { key: "seasonal-margins", label: "Seasonal Margins" },
  { key: "promo-codes", label: "Promo Codes" },
];

export function PricingTabs({
  seasonalMargins,
  promoCodes,
  divisions,
  currencies,
  access,
  t,
}: {
  seasonalMargins: SeasonalMarginDto[];
  promoCodes: PromoCodeDto[];
  divisions: AdministrativeDivisionDto[];
  currencies: CurrencyDto[];
  access: AccessLevel | null;
  t: Record<string, string>;
}) {
  const [active, setActive] = useState("seasonal-margins");

  const tabs = TABS.map((tab) => ({
    ...tab,
    label: tab.key === "seasonal-margins"
      ? t["admin.pricing.tab.seasonal_margins"] ?? tab.label
      : t["admin.pricing.tab.promo_codes"] ?? tab.label,
  }));

  return (
    <>
      <TabBar tabs={tabs} active={active} onChange={setActive} />

      {active === "seasonal-margins" && (
        <SeasonalMarginsTab
          seasonalMargins={seasonalMargins}
          divisions={divisions}
          access={access}
          t={t}
        />
      )}

      {active === "promo-codes" && (
        <PromoCodesTab
          promoCodes={promoCodes}
          currencies={currencies}
          access={access}
          t={t}
        />
      )}
    </>
  );
}
