"use client";

import { DataTable, type Column } from "@/components/data-table";
import { BarChart } from "@/components/bar-chart";
import type {
  CancellationStatsDto,
  PromoCodeStatsDto,
  SubscriptionMrrDto,
  ReviewStatsDto,
} from "@/types/statistics";

interface FinancialTabProps {
  cancellations: CancellationStatsDto | null;
  promoCodes: PromoCodeStatsDto[];
  mrr: SubscriptionMrrDto[];
  reviews: ReviewStatsDto | null;
  t: Record<string, string>;
}

export function FinancialTab({ cancellations, promoCodes, mrr, reviews, t }: FinancialTabProps) {
  const fmtCurrency = (v: number) => `€${v.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;

  const promoColumns: Column<PromoCodeStatsDto>[] = [
    { key: "code", header: t["admin.statistics.code"] ?? "Code" },
    {
      key: "usageCount",
      header: t["admin.statistics.usage_count"] ?? "Usage Count",
      render: (item) => String(item.usageCount),
      className: "text-right",
    },
    {
      key: "totalDiscount",
      header: t["admin.statistics.total_discount"] ?? "Total Discount",
      render: (item) => `${fmtCurrency(item.totalDiscount)} ${item.currencyCode}`,
      className: "text-right",
    },
  ];

  const mrrColumns: Column<SubscriptionMrrDto>[] = [
    { key: "subscriptionName", header: t["admin.statistics.subscription"] ?? "Subscription" },
    {
      key: "activeCount",
      header: t["admin.statistics.active_count"] ?? "Active",
      render: (item) => String(item.activeCount),
      className: "text-right",
    },
    {
      key: "monthlyRevenue",
      header: t["admin.statistics.monthly_revenue"] ?? "Monthly Revenue",
      render: (item) => `${fmtCurrency(item.monthlyRevenue)} ${item.currencyCode}`,
      className: "text-right",
    },
  ];

  const totalMrr = mrr.reduce((sum, m) => sum + m.monthlyRevenue, 0);

  return (
    <div className="space-y-8">
      {cancellations && (
        <section>
          <h3 className="mb-3 text-lg font-semibold">{t["admin.statistics.cancellations"] ?? "Cancellations"}</h3>
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            <Card label={t["admin.statistics.count"] ?? "Count"} value={String(cancellations.cancellationCount)} />
            <Card label={t["admin.statistics.cancellation_rate"] ?? "Cancellation Rate"} value={`${cancellations.cancellationRate.toFixed(1)}%`} />
            <Card label={t["admin.statistics.total_penalty"] ?? "Total Penalty"} value={fmtCurrency(cancellations.totalPenalty)} />
            <Card label={t["admin.statistics.total_refund"] ?? "Total Refund"} value={fmtCurrency(cancellations.totalRefund)} />
          </div>
        </section>
      )}

      {reviews && (
        <section>
          <h3 className="mb-3 text-lg font-semibold">{t["admin.statistics.reviews"] ?? "Reviews"}</h3>
          <div className="mb-4 grid gap-4 sm:grid-cols-2">
            <Card label={t["admin.statistics.avg_rating"] ?? "Avg. Rating"} value={reviews.averageRating.toFixed(1)} />
            <Card label={t["admin.statistics.total_reviews"] ?? "Total Reviews"} value={String(reviews.totalReviews)} />
          </div>
          <BarChart
            items={[
              { label: "5 stars", value: reviews.rating5 },
              { label: "4 stars", value: reviews.rating4 },
              { label: "3 stars", value: reviews.rating3 },
              { label: "2 stars", value: reviews.rating2 },
              { label: "1 star", value: reviews.rating1 },
            ]}
            color="bg-amber-500"
          />
        </section>
      )}

      <section>
        <h3 className="mb-3 text-lg font-semibold">{t["admin.statistics.promo_codes"] ?? "Promo Codes"}</h3>
        <DataTable columns={promoColumns} data={promoCodes} keyField="promoCodeId" emptyMessage={t["admin.label.no_data"] ?? "No data."} />
      </section>

      <section>
        <h3 className="mb-3 text-lg font-semibold">{t["admin.statistics.mrr"] ?? "Monthly Recurring Revenue"}</h3>
        {mrr.length > 0 && (
          <div className="mb-4">
            <Card label={t["admin.statistics.total_mrr"] ?? "Total MRR"} value={fmtCurrency(totalMrr)} />
          </div>
        )}
        <DataTable columns={mrrColumns} data={mrr} keyField="subscriptionName" emptyMessage={t["admin.label.no_data"] ?? "No data."} />
      </section>
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
