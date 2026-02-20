export interface HotelDto {
  id: number;
  cityId: number;
  name: string;
  stars: number;
  address: string;
  email: string | null;
  phone: string | null;
  checkInTime: string;
  checkOutTime: string;
  latitude: number | null;
  longitude: number | null;
  margin: number;
  enabled: boolean;
  createdAt: string;
}

export interface CreateHotelRequest {
  cityId: number;
  name: string;
  stars: number;
  address: string;
  email: string | null;
  phone: string | null;
  checkInTime: string;
  checkOutTime: string;
  latitude: number | null;
  longitude: number | null;
  margin: number;
}

export interface HotelImageDto {
  id: number;
  hotelId: number;
  url: string;
  altText: string | null;
  sortOrder: number;
  createdAt: string;
}

export interface CreateHotelImageRequest {
  hotelId: number;
  url: string;
  altText: string | null;
  sortOrder: number;
}

export interface HotelAmenityDto {
  id: number;
  hotelId: number;
  amenityId: number;
  createdAt: string;
}

export interface HotelProviderDto {
  id: number;
  hotelId: number;
  providerId: number;
  enabled: boolean;
  createdAt: string;
}

export interface HotelProviderRoomTypeDto {
  id: number;
  hotelProviderId: number;
  roomTypeId: number;
  capacity: number;
  quantity: number;
  pricePerNight: number;
  currencyId: number;
  exchangeRateId: number;
  enabled: boolean;
  createdAt: string;
}

export interface CreateHotelProviderRoomTypeRequest {
  hotelProviderId: number;
  roomTypeId: number;
  capacity: number;
  quantity: number;
  pricePerNight: number;
  currencyId: number;
  exchangeRateId: number;
}

export interface HotelProviderRoomTypeBoardDto {
  id: number;
  hotelProviderRoomTypeId: number;
  boardTypeId: number;
  pricePerNight: number;
  enabled: boolean;
}

export interface HotelBlackoutDto {
  id: number;
  hotelId: number;
  startDate: string;
  endDate: string;
  reason: string | null;
  createdAt: string;
}

export interface CreateHotelBlackoutRequest {
  hotelId: number;
  startDate: string;
  endDate: string;
  reason: string | null;
}
