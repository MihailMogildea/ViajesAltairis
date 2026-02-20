"use server";

import { apiFetch } from "@/lib/api";

interface ProviderTypeDto {
  id: number;
  name: string;
  createdAt: string;
}

export async function createProviderType(
  payload: { name: string }
): Promise<ProviderTypeDto> {
  return apiFetch<ProviderTypeDto>("/api/ProviderTypes", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateProviderType(
  id: number,
  payload: { name: string }
): Promise<ProviderTypeDto> {
  return apiFetch<ProviderTypeDto>(`/api/ProviderTypes/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteProviderType(id: number): Promise<void> {
  await apiFetch(`/api/ProviderTypes/${id}`, { method: "DELETE" });
}
