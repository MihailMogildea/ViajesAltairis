"use client";

import { useState } from "react";
import {
  TaxDto,
  TaxTypeDto,
  CountryDto,
  AdministrativeDivisionDto,
  CityDto,
} from "@/types/system";
import { TabBar } from "@/components/tab-bar";
import { TaxesTab } from "./taxes-tab";
import { TaxTypesTab } from "./tax-types-tab";

interface TaxesTabsProps {
  taxes: TaxDto[];
  taxTypes: TaxTypeDto[];
  countries: CountryDto[];
  divisions: AdministrativeDivisionDto[];
  cities: CityDto[];
  t: Record<string, string>;
}

export function TaxesTabs({
  taxes,
  taxTypes,
  countries,
  divisions,
  cities,
  t,
}: TaxesTabsProps) {
  const [activeTab, setActiveTab] = useState("taxes");

  const tabs = [
    { key: "taxes", label: t["admin.taxes.tab_taxes"] ?? "Taxes" },
    { key: "tax-types", label: t["admin.taxes.tab_types"] ?? "Tax Types" },
  ];

  return (
    <>
      <TabBar tabs={tabs} active={activeTab} onChange={setActiveTab} />

      {activeTab === "taxes" && (
        <TaxesTab
          taxes={taxes}
          taxTypes={taxTypes}
          countries={countries}
          divisions={divisions}
          cities={cities}
          t={t}
        />
      )}

      {activeTab === "tax-types" && (
        <TaxTypesTab taxTypes={taxTypes} t={t} />
      )}
    </>
  );
}
