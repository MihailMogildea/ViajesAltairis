"use server";

import { apiFetch } from "@/lib/api";
import type { HotelBlackoutDto, CreateHotelBlackoutRequest } from "@/types/hotel";
import type { CancellationDto, CancellationPolicyDto } from "@/types/reservation";

// --- Hotel Blackouts ---

export async function getBlackouts(): Promise<HotelBlackoutDto[]> {
  return apiFetch<HotelBlackoutDto[]>("/api/HotelBlackouts", {
    cache: "no-store",
  });
}

export async function createBlackout(
  payload: CreateHotelBlackoutRequest
): Promise<HotelBlackoutDto> {
  return apiFetch<HotelBlackoutDto>("/api/HotelBlackouts", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateBlackout(
  id: number,
  payload: CreateHotelBlackoutRequest
): Promise<HotelBlackoutDto> {
  return apiFetch<HotelBlackoutDto>(`/api/HotelBlackouts/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteBlackout(id: number): Promise<void> {
  await apiFetch(`/api/HotelBlackouts/${id}`, {
    method: "DELETE",
  });
}

// --- Cancellations ---

export async function getCancellations(): Promise<CancellationDto[]> {
  return apiFetch<CancellationDto[]>("/api/Cancellations", {
    cache: "no-store",
  });
}

// --- Cancellation Policies ---

export async function getCancellationPolicies(): Promise<CancellationPolicyDto[]> {
  return apiFetch<CancellationPolicyDto[]>("/api/CancellationPolicies", {
    cache: "no-store",
  });
}

interface CreateCancellationPolicyPayload {
  hotelId: number;
  freeCancellationHours: number;
  penaltyPercentage: number;
}

export async function createCancellationPolicy(
  payload: CreateCancellationPolicyPayload
): Promise<CancellationPolicyDto> {
  return apiFetch<CancellationPolicyDto>("/api/CancellationPolicies", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateCancellationPolicy(
  id: number,
  payload: CreateCancellationPolicyPayload
): Promise<CancellationPolicyDto> {
  return apiFetch<CancellationPolicyDto>(`/api/CancellationPolicies/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteCancellationPolicy(id: number): Promise<void> {
  await apiFetch(`/api/CancellationPolicies/${id}`, {
    method: "DELETE",
  });
}

export async function toggleCancellationPolicyEnabled(
  id: number
): Promise<CancellationPolicyDto> {
  return apiFetch<CancellationPolicyDto>(`/api/CancellationPolicies/${id}/enabled`, {
    method: "PATCH",
  });
}
