"use server";

import { apiFetch } from "@/lib/api";
import type { HotelDto, CreateHotelRequest } from "@/types/hotel";

export async function createHotel(
  payload: CreateHotelRequest
): Promise<HotelDto> {
  return apiFetch<HotelDto>("/api/Hotels", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateHotel(
  id: number,
  payload: CreateHotelRequest
): Promise<HotelDto> {
  return apiFetch<HotelDto>(`/api/Hotels/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteHotel(id: number): Promise<void> {
  await apiFetch(`/api/Hotels/${id}`, {
    method: "DELETE",
  });
}

export async function toggleHotelEnabled(
  id: number,
  enabled: boolean
): Promise<void> {
  await apiFetch(`/api/Hotels/${id}/enabled`, {
    method: "PATCH",
    body: JSON.stringify({ enabled }),
  });
}
