export interface RevenueByHotelDto {
  hotelId: number;
  hotelName: string;
  currencyCode: string;
  totalRevenue: number;
  reservationCount: number;
}

export interface RevenueByProviderDto {
  providerId: number;
  providerName: string;
  currencyCode: string;
  totalRevenue: number;
  reservationCount: number;
}

export interface RevenuePeriodDto {
  period: string;
  currencyCode: string;
  totalRevenue: number;
  reservationCount: number;
}

export interface BookingVolumeDto {
  period: string;
  bookingCount: number;
}

export interface BookingsByStatusDto {
  statusName: string;
  bookingCount: number;
}

export interface BookingAverageDto {
  averageValue: number;
  averageNights: number;
  totalBookings: number;
}

export interface OccupancyDto {
  hotelId: number;
  hotelName: string;
  bookedRoomNights: number;
  totalRoomNights: number;
  occupancyRate: number;
}

export interface UserGrowthDto {
  period: string;
  newUsers: number;
}

export interface UsersByTypeDto {
  typeName: string;
  userCount: number;
}

export interface ActiveSubscriptionsDto {
  activeCount: number;
  totalUsers: number;
  subscriptionRate: number;
}

export interface SubscriptionMrrDto {
  subscriptionName: string;
  currencyCode: string;
  activeCount: number;
  monthlyRevenue: number;
}

export interface CancellationStatsDto {
  cancellationCount: number;
  totalReservations: number;
  cancellationRate: number;
  totalPenalty: number;
  totalRefund: number;
}

export interface ReviewStatsDto {
  averageRating: number;
  totalReviews: number;
  rating1: number;
  rating2: number;
  rating3: number;
  rating4: number;
  rating5: number;
}

export interface PromoCodeStatsDto {
  promoCodeId: number;
  code: string;
  usageCount: number;
  totalDiscount: number;
  currencyCode: string;
}
