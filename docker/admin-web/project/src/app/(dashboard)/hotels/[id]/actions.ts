"use server";

import { apiFetch } from "@/lib/api";
import type {
  HotelDto,
  CreateHotelRequest,
  HotelImageDto,
  CreateHotelImageRequest,
  HotelAmenityDto,
  HotelProviderDto,
  HotelProviderRoomTypeDto,
  HotelProviderRoomTypeBoardDto,
} from "@/types/hotel";

export async function updateHotel(
  id: number,
  payload: CreateHotelRequest
): Promise<HotelDto> {
  return apiFetch<HotelDto>(`/api/Hotels/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function createHotelImage(
  payload: CreateHotelImageRequest
): Promise<HotelImageDto> {
  return apiFetch<HotelImageDto>("/api/HotelImages", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateHotelImage(
  id: number,
  payload: CreateHotelImageRequest
): Promise<HotelImageDto> {
  return apiFetch<HotelImageDto>(`/api/HotelImages/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteHotelImage(id: number): Promise<void> {
  await apiFetch(`/api/HotelImages/${id}`, {
    method: "DELETE",
  });
}

export async function addHotelAmenity(
  hotelId: number,
  amenityId: number
): Promise<HotelAmenityDto> {
  return apiFetch<HotelAmenityDto>("/api/HotelAmenities", {
    method: "POST",
    body: JSON.stringify({ hotelId, amenityId }),
  });
}

export async function removeHotelAmenity(id: number): Promise<void> {
  await apiFetch(`/api/HotelAmenities/${id}`, {
    method: "DELETE",
  });
}

// --- Hotel Providers ---

export async function createHotelProvider(payload: {
  hotelId: number;
  providerId: number;
}): Promise<HotelProviderDto> {
  return apiFetch<HotelProviderDto>("/api/HotelProviders", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function deleteHotelProvider(id: number): Promise<void> {
  await apiFetch(`/api/HotelProviders/${id}`, {
    method: "DELETE",
  });
}

export async function toggleHotelProviderEnabled(
  id: number,
  enabled: boolean
): Promise<void> {
  await apiFetch(`/api/HotelProviders/${id}/enabled`, {
    method: "PATCH",
    body: JSON.stringify({ enabled }),
  });
}

// --- Hotel Provider Room Types ---

export async function createHotelProviderRoomType(payload: {
  hotelProviderId: number;
  roomTypeId: number;
  capacity: number;
  quantity: number;
  pricePerNight: number;
  currencyId: number;
  exchangeRateId: number;
}): Promise<HotelProviderRoomTypeDto> {
  return apiFetch<HotelProviderRoomTypeDto>("/api/HotelProviderRoomTypes", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateHotelProviderRoomType(
  id: number,
  payload: {
    hotelProviderId: number;
    roomTypeId: number;
    capacity: number;
    quantity: number;
    pricePerNight: number;
    currencyId: number;
    exchangeRateId: number;
  }
): Promise<HotelProviderRoomTypeDto> {
  return apiFetch<HotelProviderRoomTypeDto>(
    `/api/HotelProviderRoomTypes/${id}`,
    {
      method: "PUT",
      body: JSON.stringify(payload),
    }
  );
}

export async function deleteHotelProviderRoomType(id: number): Promise<void> {
  await apiFetch(`/api/HotelProviderRoomTypes/${id}`, {
    method: "DELETE",
  });
}

export async function toggleHotelProviderRoomTypeEnabled(
  id: number,
  enabled: boolean
): Promise<void> {
  await apiFetch(`/api/HotelProviderRoomTypes/${id}/enabled`, {
    method: "PATCH",
    body: JSON.stringify({ enabled }),
  });
}

// --- Hotel Provider Room Type Boards ---

export async function createHotelProviderRoomTypeBoard(payload: {
  hotelProviderRoomTypeId: number;
  boardTypeId: number;
  pricePerNight: number;
}): Promise<HotelProviderRoomTypeBoardDto> {
  return apiFetch<HotelProviderRoomTypeBoardDto>(
    "/api/HotelProviderRoomTypeBoards",
    {
      method: "POST",
      body: JSON.stringify(payload),
    }
  );
}

export async function updateHotelProviderRoomTypeBoard(
  id: number,
  payload: {
    hotelProviderRoomTypeId: number;
    boardTypeId: number;
    pricePerNight: number;
  }
): Promise<HotelProviderRoomTypeBoardDto> {
  return apiFetch<HotelProviderRoomTypeBoardDto>(
    `/api/HotelProviderRoomTypeBoards/${id}`,
    {
      method: "PUT",
      body: JSON.stringify(payload),
    }
  );
}

export async function deleteHotelProviderRoomTypeBoard(
  id: number
): Promise<void> {
  await apiFetch(`/api/HotelProviderRoomTypeBoards/${id}`, {
    method: "DELETE",
  });
}
