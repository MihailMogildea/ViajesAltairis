"use server";

import { apiFetch } from "@/lib/api";
import type { CityDto } from "@/types/system";

export async function createCity(payload: {
  administrativeDivisionId: number;
  name: string;
}): Promise<CityDto> {
  return apiFetch<CityDto>("/api/Cities", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateCity(
  id: number,
  payload: { administrativeDivisionId: number; name: string }
): Promise<CityDto> {
  return apiFetch<CityDto>(`/api/Cities/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteCity(id: number): Promise<void> {
  await apiFetch(`/api/Cities/${id}`, { method: "DELETE" });
}

export async function toggleCityEnabled(
  id: number,
  enabled: boolean
): Promise<void> {
  await apiFetch(`/api/Cities/${id}/enabled`, {
    method: "PATCH",
    body: JSON.stringify({ enabled }),
  });
}
