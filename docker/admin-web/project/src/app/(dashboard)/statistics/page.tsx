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
  from.setDate(from.getDate() - 30);
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
  const qs = `?from=${from}&to=${to}`;

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
      apiFetch<RevenueByHotelDto[]>(`/api/statistics/revenue/by-hotel${qs}`, { cache: "no-store" }).catch(() => [] as RevenueByHotelDto[]),
      apiFetch<RevenueByProviderDto[]>(`/api/statistics/revenue/by-provider${qs}`, { cache: "no-store" }).catch(() => [] as RevenueByProviderDto[]),
      apiFetch<RevenuePeriodDto[]>(`/api/statistics/revenue/by-period${qs}`, { cache: "no-store" }).catch(() => [] as RevenuePeriodDto[]),
      apiFetch<BookingVolumeDto[]>(`/api/statistics/bookings/volume${qs}`, { cache: "no-store" }).catch(() => [] as BookingVolumeDto[]),
      apiFetch<BookingsByStatusDto[]>(`/api/statistics/bookings/by-status${qs}`, { cache: "no-store" }).catch(() => [] as BookingsByStatusDto[]),
      apiFetch<BookingAverageDto>(`/api/statistics/bookings/average${qs}`, { cache: "no-store" }).catch(() => null),
      apiFetch<OccupancyDto[]>(`/api/statistics/occupancy${qs}`, { cache: "no-store" }).catch(() => [] as OccupancyDto[]),
      apiFetch<UserGrowthDto[]>(`/api/statistics/users/growth${qs}`, { cache: "no-store" }).catch(() => [] as UserGrowthDto[]),
      apiFetch<UsersByTypeDto[]>(`/api/statistics/users/by-type${qs}`, { cache: "no-store" }).catch(() => [] as UsersByTypeDto[]),
      apiFetch<ActiveSubscriptionsDto>("/api/statistics/users/subscriptions", { cache: "no-store" }).catch(() => null),
      apiFetch<ReviewStatsDto>(`/api/statistics/reviews${qs}`, { cache: "no-store" }).catch(() => null),
      apiFetch<CancellationStatsDto>(`/api/statistics/cancellations${qs}`, { cache: "no-store" }).catch(() => null),
      apiFetch<PromoCodeStatsDto[]>(`/api/statistics/promo-codes${qs}`, { cache: "no-store" }).catch(() => [] as PromoCodeStatsDto[]),
      apiFetch<SubscriptionMrrDto[]>("/api/statistics/subscriptions/mrr", { cache: "no-store" }).catch(() => [] as SubscriptionMrrDto[]),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load statistics";
  }

  return (
    <div>
      <SectionHeader
        title={t["admin.section.statistics"] ?? "Statistics"}
        description={t["admin.statistics.desc"] ?? "Revenue, bookings, occupancy, and user analytics."}
      />

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
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
      )}
    </div>
  );
}
