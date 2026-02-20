"use server";

import { apiFetch } from "@/lib/api";
import type {
  HotelDto,
  CreateHotelRequest,
  HotelImageDto,
  CreateHotelImageRequest,
  HotelAmenityDto,
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
