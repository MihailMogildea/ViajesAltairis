"use client";

import { BarChart } from "@/components/bar-chart";
import type { UserGrowthDto, UsersByTypeDto, ActiveSubscriptionsDto } from "@/types/statistics";

interface UsersTabProps {
  growth: UserGrowthDto[];
  byType: UsersByTypeDto[];
  subscriptions: ActiveSubscriptionsDto | null;
  t: Record<string, string>;
}

export function UsersTab({ growth, byType, subscriptions, t }: UsersTabProps) {
  return (
    <div className="space-y-8">
      <section>
        <h3 className="mb-3 text-lg font-semibold">{t["admin.statistics.growth"] ?? "User Growth"}</h3>
        <BarChart
          items={growth.map((g) => ({ label: g.period, value: g.newUsers }))}
          color="bg-violet-500"
        />
      </section>

      <section>
        <h3 className="mb-3 text-lg font-semibold">{t["admin.statistics.by_type"] ?? "Users by Type"}</h3>
        <BarChart
          items={byType.map((u) => ({ label: u.typeName, value: u.userCount }))}
          color="bg-sky-500"
        />
      </section>

      {subscriptions && (
        <section>
          <h3 className="mb-3 text-lg font-semibold">{t["admin.statistics.subscription_rate"] ?? "Subscription Rate"}</h3>
          <div className="grid gap-4 sm:grid-cols-3">
            <Card
              label={t["admin.statistics.active_subscribers"] ?? "Active Subscribers"}
              value={String(subscriptions.activeCount)}
            />
            <Card
              label={t["admin.statistics.total_users"] ?? "Total Users"}
              value={String(subscriptions.totalUsers)}
            />
            <Card
              label={t["admin.statistics.subscription_rate"] ?? "Subscription Rate"}
              value={`${subscriptions.subscriptionRate.toFixed(1)}%`}
            />
          </div>
        </section>
      )}
    </div>
  );
}

function Card({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-lg border border-gray-200 bg-white p-5">
      <p className="text-sm font-medium text-gray-500">{label}</p>
      <p className="mt-1 text-3xl font-semibold text-gray-900">{value}</p>
    </div>
  );
}
