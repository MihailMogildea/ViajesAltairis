"use client";

import { useState } from "react";
import { TabBar } from "@/components/tab-bar";
import { SubscriptionTypesTab } from "./subscription-types-tab";
import { UserSubscriptionsTab } from "./user-subscriptions-tab";
import type { SubscriptionTypeDto } from "@/types/subscription";
import type { UserSubscriptionDto } from "@/types/user";
import type { CurrencyDto } from "@/types/system";

const TABS = [
  { key: "types", label: "Subscription Types" },
  { key: "user-subscriptions", label: "User Subscriptions" },
];

export function SubscriptionsTabs({
  types,
  subscriptions,
  currencies,
  access,
  t,
}: {
  types: SubscriptionTypeDto[];
  subscriptions: UserSubscriptionDto[];
  currencies: CurrencyDto[];
  access: string | null;
  t: Record<string, string>;
}) {
  const [activeTab, setActiveTab] = useState("types");

  const tabs = TABS.map((tab) => ({
    key: tab.key,
    label: t[`admin.subscriptions.tab.${tab.key}`] ?? tab.label,
  }));

  return (
    <>
      <TabBar tabs={tabs} active={activeTab} onChange={setActiveTab} />

      {activeTab === "types" && (
        <SubscriptionTypesTab
          types={types}
          currencies={currencies}
          access={access}
          t={t}
        />
      )}

      {activeTab === "user-subscriptions" && (
        <UserSubscriptionsTab
          subscriptions={subscriptions}
          types={types}
          access={access}
          t={t}
        />
      )}
    </>
  );
}
