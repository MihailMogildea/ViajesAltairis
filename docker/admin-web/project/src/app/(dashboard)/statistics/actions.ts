"use server";

import { apiFetch } from "@/lib/api";
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

function buildParams(from?: string, to?: string, groupBy?: string): string {
  const params = new URLSearchParams();
  if (from) params.set("from", from);
  if (to) params.set("to", to);
  if (groupBy) params.set("groupBy", groupBy);
  const qs = params.toString();
  return qs ? `?${qs}` : "";
}

export async function fetchRevenueByHotel(from?: string, to?: string): Promise<RevenueByHotelDto[]> {
  return apiFetch<RevenueByHotelDto[]>(`/api/statistics/revenue/by-hotel${buildParams(from, to)}`, { cache: "no-store" });
}

export async function fetchRevenueByProvider(from?: string, to?: string): Promise<RevenueByProviderDto[]> {
  return apiFetch<RevenueByProviderDto[]>(`/api/statistics/revenue/by-provider${buildParams(from, to)}`, { cache: "no-store" });
}

export async function fetchRevenueByPeriod(from?: string, to?: string, groupBy?: string): Promise<RevenuePeriodDto[]> {
  return apiFetch<RevenuePeriodDto[]>(`/api/statistics/revenue/by-period${buildParams(from, to, groupBy)}`, { cache: "no-store" });
}

export async function fetchBookingVolume(from?: string, to?: string, groupBy?: string): Promise<BookingVolumeDto[]> {
  return apiFetch<BookingVolumeDto[]>(`/api/statistics/bookings/volume${buildParams(from, to, groupBy)}`, { cache: "no-store" });
}

export async function fetchBookingsByStatus(from?: string, to?: string): Promise<BookingsByStatusDto[]> {
  return apiFetch<BookingsByStatusDto[]>(`/api/statistics/bookings/by-status${buildParams(from, to)}`, { cache: "no-store" });
}

export async function fetchBookingAverage(from?: string, to?: string): Promise<BookingAverageDto> {
  return apiFetch<BookingAverageDto>(`/api/statistics/bookings/average${buildParams(from, to)}`, { cache: "no-store" });
}

export async function fetchOccupancy(from?: string, to?: string): Promise<OccupancyDto[]> {
  return apiFetch<OccupancyDto[]>(`/api/statistics/occupancy${buildParams(from, to)}`, { cache: "no-store" });
}

export async function fetchUserGrowth(from?: string, to?: string, groupBy?: string): Promise<UserGrowthDto[]> {
  return apiFetch<UserGrowthDto[]>(`/api/statistics/users/growth${buildParams(from, to, groupBy)}`, { cache: "no-store" });
}

export async function fetchUsersByType(from?: string, to?: string): Promise<UsersByTypeDto[]> {
  return apiFetch<UsersByTypeDto[]>(`/api/statistics/users/by-type${buildParams(from, to)}`, { cache: "no-store" });
}

export async function fetchActiveSubscriptions(): Promise<ActiveSubscriptionsDto> {
  return apiFetch<ActiveSubscriptionsDto>("/api/statistics/users/subscriptions", { cache: "no-store" });
}

export async function fetchReviewStats(from?: string, to?: string): Promise<ReviewStatsDto> {
  return apiFetch<ReviewStatsDto>(`/api/statistics/reviews${buildParams(from, to)}`, { cache: "no-store" });
}

export async function fetchCancellationStats(from?: string, to?: string): Promise<CancellationStatsDto> {
  return apiFetch<CancellationStatsDto>(`/api/statistics/cancellations${buildParams(from, to)}`, { cache: "no-store" });
}

export async function fetchPromoCodeStats(from?: string, to?: string): Promise<PromoCodeStatsDto[]> {
  return apiFetch<PromoCodeStatsDto[]>(`/api/statistics/promo-codes${buildParams(from, to)}`, { cache: "no-store" });
}

export async function fetchSubscriptionMrr(): Promise<SubscriptionMrrDto[]> {
  return apiFetch<SubscriptionMrrDto[]>("/api/statistics/subscriptions/mrr", { cache: "no-store" });
}
