"use server";

import { apiFetch } from "@/lib/api";
import type { CountryDto } from "@/types/system";

interface CreateCountryPayload {
  isoCode: string;
  name: string;
  currencyId: number;
}

interface UpdateCountryPayload {
  isoCode: string;
  name: string;
  currencyId: number;
}

export async function createCountry(
  payload: CreateCountryPayload
): Promise<CountryDto> {
  return apiFetch<CountryDto>("/api/Countries", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateCountry(
  id: number,
  payload: UpdateCountryPayload
): Promise<CountryDto> {
  return apiFetch<CountryDto>(`/api/Countries/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteCountry(id: number): Promise<void> {
  await apiFetch(`/api/Countries/${id}`, {
    method: "DELETE",
  });
}

export async function toggleCountryEnabled(
  id: number,
  enabled: boolean
): Promise<void> {
  await apiFetch(`/api/Countries/${id}/enabled`, {
    method: "PATCH",
    body: JSON.stringify({ enabled }),
  });
}
