"use server";

import { apiFetch } from "@/lib/api";
import type { ProviderDto, CreateProviderRequest } from "@/types/provider";

export async function createProvider(
  payload: CreateProviderRequest
): Promise<ProviderDto> {
  return apiFetch<ProviderDto>("/api/Providers", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateProvider(
  id: number,
  payload: CreateProviderRequest
): Promise<ProviderDto> {
  return apiFetch<ProviderDto>(`/api/Providers/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteProvider(id: number): Promise<void> {
  await apiFetch(`/api/Providers/${id}`, {
    method: "DELETE",
  });
}

export async function toggleProviderEnabled(
  id: number,
  enabled: boolean
): Promise<void> {
  await apiFetch(`/api/Providers/${id}/enabled`, {
    method: "PATCH",
    body: JSON.stringify({ enabled }),
  });
}
