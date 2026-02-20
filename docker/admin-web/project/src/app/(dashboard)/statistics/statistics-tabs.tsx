"use client";

import { useState, useCallback } from "react";
import { TabBar } from "@/components/tab-bar";
import { DateRangePicker } from "@/components/date-range-picker";
import { RevenueTab } from "./revenue-tab";
import { BookingsTab } from "./bookings-tab";
import { OccupancyTab } from "./occupancy-tab";
import { UsersTab } from "./users-tab";
import { FinancialTab } from "./financial-tab";
import {
  fetchRevenueByHotel,
  fetchRevenueByProvider,
  fetchRevenueByPeriod,
  fetchBookingVolume,
  fetchBookingsByStatus,
  fetchBookingAverage,
  fetchOccupancy,
  fetchUserGrowth,
  fetchUsersByType,
  fetchActiveSubscriptions,
  fetchReviewStats,
  fetchCancellationStats,
  fetchPromoCodeStats,
  fetchSubscriptionMrr,
} from "./actions";
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

interface StatisticsData {
  revenueByHotel: RevenueByHotelDto[];
  revenueByProvider: RevenueByProviderDto[];
  revenueByPeriod: RevenuePeriodDto[];
  bookingVolume: BookingVolumeDto[];
  bookingsByStatus: BookingsByStatusDto[];
  bookingAverage: BookingAverageDto | null;
  occupancy: OccupancyDto[];
  userGrowth: UserGrowthDto[];
  usersByType: UsersByTypeDto[];
  activeSubscriptions: ActiveSubscriptionsDto | null;
  reviewStats: ReviewStatsDto | null;
  cancellationStats: CancellationStatsDto | null;
  promoCodeStats: PromoCodeStatsDto[];
  subscriptionMrr: SubscriptionMrrDto[];
}

interface StatisticsTabsProps {
  initialData: StatisticsData;
  initialFrom: string;
  initialTo: string;
  t: Record<string, string>;
}

const TABS = [
  { key: "revenue", label: "Revenue" },
  { key: "bookings", label: "Bookings" },
  { key: "occupancy", label: "Occupancy" },
  { key: "users", label: "Users" },
  { key: "financial", label: "Financial" },
];

export function StatisticsTabs({ initialData, initialFrom, initialTo, t }: StatisticsTabsProps) {
  const [activeTab, setActiveTab] = useState("revenue");
  const [from, setFrom] = useState(initialFrom);
  const [to, setTo] = useState(initialTo);
  const [data, setData] = useState<StatisticsData>(initialData);
  const [loading, setLoading] = useState(false);

  const tabs = TABS.map((tab) => ({
    ...tab,
    label: t[`admin.statistics.tab.${tab.key}`] ?? tab.label,
  }));

  const refresh = useCallback(async (newFrom: string, newTo: string) => {
    setFrom(newFrom);
    setTo(newTo);
    setLoading(true);
    try {
      const [
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
        fetchRevenueByHotel(newFrom, newTo).catch(() => [] as RevenueByHotelDto[]),
        fetchRevenueByProvider(newFrom, newTo).catch(() => [] as RevenueByProviderDto[]),
        fetchRevenueByPeriod(newFrom, newTo).catch(() => [] as RevenuePeriodDto[]),
        fetchBookingVolume(newFrom, newTo).catch(() => [] as BookingVolumeDto[]),
        fetchBookingsByStatus(newFrom, newTo).catch(() => [] as BookingsByStatusDto[]),
        fetchBookingAverage(newFrom, newTo).catch(() => null),
        fetchOccupancy(newFrom, newTo).catch(() => [] as OccupancyDto[]),
        fetchUserGrowth(newFrom, newTo).catch(() => [] as UserGrowthDto[]),
        fetchUsersByType(newFrom, newTo).catch(() => [] as UsersByTypeDto[]),
        fetchActiveSubscriptions().catch(() => null),
        fetchReviewStats(newFrom, newTo).catch(() => null),
        fetchCancellationStats(newFrom, newTo).catch(() => null),
        fetchPromoCodeStats(newFrom, newTo).catch(() => [] as PromoCodeStatsDto[]),
        fetchSubscriptionMrr().catch(() => [] as SubscriptionMrrDto[]),
      ]);
      setData({
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
      });
    } finally {
      setLoading(false);
    }
  }, []);

  return (
    <>
      <DateRangePicker from={from} to={to} onChange={refresh} t={t} />

      {loading && (
        <p className="mb-4 text-sm text-gray-500">{t["admin.label.loading"] ?? "Loading..."}</p>
      )}

      <TabBar tabs={tabs} active={activeTab} onChange={setActiveTab} />

      {activeTab === "revenue" && (
        <RevenueTab
          byHotel={data.revenueByHotel}
          byProvider={data.revenueByProvider}
          byPeriod={data.revenueByPeriod}
          t={t}
        />
      )}

      {activeTab === "bookings" && (
        <BookingsTab
          volume={data.bookingVolume}
          byStatus={data.bookingsByStatus}
          average={data.bookingAverage}
          t={t}
        />
      )}

      {activeTab === "occupancy" && (
        <OccupancyTab occupancy={data.occupancy} t={t} />
      )}

      {activeTab === "users" && (
        <UsersTab
          growth={data.userGrowth}
          byType={data.usersByType}
          subscriptions={data.activeSubscriptions}
          t={t}
        />
      )}

      {activeTab === "financial" && (
        <FinancialTab
          cancellations={data.cancellationStats}
          promoCodes={data.promoCodeStats}
          mrr={data.subscriptionMrr}
          reviews={data.reviewStats}
          t={t}
        />
      )}
    </>
  );
}
