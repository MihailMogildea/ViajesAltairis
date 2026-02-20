import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { SectionHeader } from "@/components/section-header";
import { SubscriptionsTabs } from "./subscriptions-tabs";
import type { SubscriptionTypeDto } from "@/types/subscription";
import type { UserSubscriptionDto } from "@/types/user";
import type { CurrencyDto } from "@/types/system";

export default async function SubscriptionsPage() {
  const session = await getSession();
  const access = session
    ? getAccessLevel(session.role, "subscriptions")
    : null;
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let types: SubscriptionTypeDto[] = [];
  let subscriptions: UserSubscriptionDto[] = [];
  let currencies: CurrencyDto[] = [];
  let error: string | null = null;

  try {
    [types, subscriptions, currencies] = await Promise.all([
      apiFetch<SubscriptionTypeDto[]>("/api/SubscriptionTypes", { cache: "no-store" }),
      apiFetch<UserSubscriptionDto[]>("/api/UserSubscriptions", { cache: "no-store" }),
      apiFetch<CurrencyDto[]>("/api/Currencies", { cache: "no-store" }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load subscriptions";
  }

  return (
    <div>
      <SectionHeader
        title={t["admin.section.subscriptions"] ?? "Subscriptions"}
        description={t["admin.section.subscriptions.desc"] ?? "Manage subscription types and user subscriptions."}
      />

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <SubscriptionsTabs
          types={types}
          subscriptions={subscriptions}
          currencies={currencies}
          access={access}
          t={t}
        />
      )}
    </div>
  );
}
