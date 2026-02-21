import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { SectionHeader } from "@/components/section-header";
import { StatisticsTabs } from "./statistics-tabs";
import type {
  RevenueByHotelDto,
  RevenueByProviderDto,
  RevenuePeriodDto,
  BookingVolumeDto,
  BookingsByStatusDto,
  BookingAverageDto,
  OccupancyDto,
  UserGrowthDto,
  UsersByTypeDto,
  ActiveSubscriptionsDto,
  ReviewStatsDto,
  CancellationStatsDto,
  PromoCodeStatsDto,
  SubscriptionMrrDto,
} from "@/types/statistics";

function defaultDateRange() {
  const to = new Date();
  const from = new Date();
  from.setFullYear(from.getFullYear() - 1);
  return {
    from: from.toISOString().split("T")[0],
    to: to.toISOString().split("T")[0],
  };
}

export default async function StatisticsPage() {
  const session = await getSession();
  const access = session ? getAccessLevel(session.role, "statistics") : null;
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  if (!access) {
    return (
      <div>
        <SectionHeader
          title={t["admin.section.statistics"] ?? "Statistics"}
          description={t["admin.label.no_access"] ?? "You do not have access to this section."}
        />
      </div>
    );
  }

  const { from, to } = defaultDateRange();
  const qs = `?from=${from}&to=${to}T23:59:59`;

  let revenueByHotel: RevenueByHotelDto[] = [];
  let revenueByProvider: RevenueByProviderDto[] = [];
  let revenueByPeriod: RevenuePeriodDto[] = [];
  let bookingVolume: BookingVolumeDto[] = [];
  let bookingsByStatus: BookingsByStatusDto[] = [];
  let bookingAverage: BookingAverageDto | null = null;
  let occupancy: OccupancyDto[] = [];
  let userGrowth: UserGrowthDto[] = [];
  let usersByType: UsersByTypeDto[] = [];
  let activeSubscriptions: ActiveSubscriptionsDto | null = null;
  let reviewStats: ReviewStatsDto | null = null;
  let cancellationStats: CancellationStatsDto | null = null;
  let promoCodeStats: PromoCodeStatsDto[] = [];
  let subscriptionMrr: SubscriptionMrrDto[] = [];
  let error: string | null = null;
  let apiErrors: string[] = [];

  function tracked<T>(promise: Promise<T>, fallback: T, label: string): Promise<T> {
    return promise.catch((e) => {
      apiErrors.push(`${label}: ${e instanceof Error ? e.message : "unknown error"}`);
      return fallback;
    });
  }

  try {
    [
      revenueByHotel,
      revenueByProvider,
      revenueByPeriod,
      bookingVolume,
      bookingsByStatus,
      bookingAverage,
      occupancy,
      userGrowth,
      usersByType,
      activeSubscriptions,
      reviewStats,
      cancellationStats,
      promoCodeStats,
      subscriptionMrr,
    ] = await Promise.all([
      tracked(apiFetch<RevenueByHotelDto[]>(`/api/statistics/revenue/by-hotel${qs}`, { cache: "no-store" }), [], "revenue/by-hotel"),
      tracked(apiFetch<RevenueByProviderDto[]>(`/api/statistics/revenue/by-provider${qs}`, { cache: "no-store" }), [], "revenue/by-provider"),
      tracked(apiFetch<RevenuePeriodDto[]>(`/api/statistics/revenue/by-period${qs}`, { cache: "no-store" }), [], "revenue/by-period"),
      tracked(apiFetch<BookingVolumeDto[]>(`/api/statistics/bookings/volume${qs}`, { cache: "no-store" }), [], "bookings/volume"),
      tracked(apiFetch<BookingsByStatusDto[]>(`/api/statistics/bookings/by-status${qs}`, { cache: "no-store" }), [], "bookings/by-status"),
      tracked(apiFetch<BookingAverageDto>(`/api/statistics/bookings/average${qs}`, { cache: "no-store" }), null, "bookings/average"),
      tracked(apiFetch<OccupancyDto[]>(`/api/statistics/occupancy${qs}`, { cache: "no-store" }), [], "occupancy"),
      tracked(apiFetch<UserGrowthDto[]>(`/api/statistics/users/growth${qs}`, { cache: "no-store" }), [], "users/growth"),
      tracked(apiFetch<UsersByTypeDto[]>(`/api/statistics/users/by-type${qs}`, { cache: "no-store" }), [], "users/by-type"),
      tracked(apiFetch<ActiveSubscriptionsDto>("/api/statistics/users/subscriptions", { cache: "no-store" }), null, "users/subscriptions"),
      tracked(apiFetch<ReviewStatsDto>(`/api/statistics/reviews${qs}`, { cache: "no-store" }), null, "reviews"),
      tracked(apiFetch<CancellationStatsDto>(`/api/statistics/cancellations${qs}`, { cache: "no-store" }), null, "cancellations"),
      tracked(apiFetch<PromoCodeStatsDto[]>(`/api/statistics/promo-codes${qs}`, { cache: "no-store" }), [], "promo-codes"),
      tracked(apiFetch<SubscriptionMrrDto[]>("/api/statistics/subscriptions/mrr", { cache: "no-store" }), [], "subscriptions/mrr"),
    ]);

    if (apiErrors.length > 0) {
      error = `Failed to load ${apiErrors.length} endpoint(s): ${apiErrors[0]}`;
    }
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load statistics";
  }

  return (
    <div>
      <SectionHeader
        title={t["admin.section.statistics"] ?? "Statistics"}
        description={t["admin.statistics.desc"] ?? "Revenue, bookings, occupancy, and user analytics."}
      />

      {error && (
        <p className="mb-4 rounded-lg border border-amber-200 bg-amber-50 px-4 py-2 text-sm text-amber-700">{error}</p>
      )}

      <StatisticsTabs
        initialData={{
          revenueByHotel,
          revenueByProvider,
          revenueByPeriod,
          bookingVolume,
          bookingsByStatus,
          bookingAverage,
          occupancy,
          userGrowth,
          usersByType,
          activeSubscriptions,
          reviewStats,
          cancellationStats,
          promoCodeStats,
          subscriptionMrr,
        }}
        initialFrom={from}
        initialTo={to}
        t={t}
      />
    </div>
  );
}
