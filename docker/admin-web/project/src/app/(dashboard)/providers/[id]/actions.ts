"use server";

import { apiFetch } from "@/lib/api";
import type { ProviderDto, CreateProviderRequest } from "@/types/provider";
import type {
  HotelProviderDto,
  HotelProviderRoomTypeDto,
  HotelProviderRoomTypeBoardDto,
} from "@/types/hotel";

// --- Provider ---

export async function updateProvider(
  id: number,
  payload: CreateProviderRequest
): Promise<ProviderDto> {
  return apiFetch<ProviderDto>(`/api/Providers/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
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
