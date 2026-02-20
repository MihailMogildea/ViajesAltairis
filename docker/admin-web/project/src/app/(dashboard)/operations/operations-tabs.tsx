"use client";

import { useState } from "react";
import { TabBar } from "@/components/tab-bar";
import { BlackoutsTab } from "./blackouts-tab";
import { CancellationsTab } from "./cancellations-tab";
import { CancellationPoliciesTab } from "./cancellation-policies-tab";
import type { HotelDto, HotelBlackoutDto } from "@/types/hotel";
import type { CancellationDto, CancellationPolicyDto } from "@/types/reservation";
import type { AccessLevel } from "@/lib/permissions";

interface OperationsTabsProps {
  blackouts: HotelBlackoutDto[];
  cancellations: CancellationDto[];
  policies: CancellationPolicyDto[];
  hotels: HotelDto[];
  access: AccessLevel;
  t: Record<string, string>;
}

const TABS = [
  { key: "blackouts", label: "Blackouts" },
  { key: "cancellations", label: "Cancellations" },
  { key: "policies", label: "Cancellation Policies" },
];

export function OperationsTabs({
  blackouts,
  cancellations,
  policies,
  hotels,
  access,
  t,
}: OperationsTabsProps) {
  const [active, setActive] = useState("blackouts");

  const tabs = TABS.map((tab) => ({
    ...tab,
    label: t[`admin.operations.tab.${tab.key}`] ?? tab.label,
  }));

  return (
    <>
      <TabBar tabs={tabs} active={active} onChange={setActive} />

      {active === "blackouts" && (
        <BlackoutsTab
          blackouts={blackouts}
          hotels={hotels}
          access={access}
          t={t}
        />
      )}

      {active === "cancellations" && (
        <CancellationsTab
          cancellations={cancellations}
          t={t}
        />
      )}

      {active === "policies" && (
        <CancellationPoliciesTab
          policies={policies}
          hotels={hotels}
          access={access}
          t={t}
        />
      )}
    </>
  );
}
