"use server";

import { apiFetch } from "@/lib/api";
import type { AdministrativeDivisionDto } from "@/types/system";

interface CreateAdminDivisionPayload {
  countryId: number;
  typeId: number;
  parentId: number | null;
  name: string;
  level: number;
}

interface UpdateAdminDivisionPayload {
  countryId: number;
  typeId: number;
  parentId: number | null;
  name: string;
  level: number;
}

export async function createAdminDivision(
  payload: CreateAdminDivisionPayload
): Promise<AdministrativeDivisionDto> {
  return apiFetch<AdministrativeDivisionDto>("/api/AdministrativeDivisions", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateAdminDivision(
  id: number,
  payload: UpdateAdminDivisionPayload
): Promise<AdministrativeDivisionDto> {
  return apiFetch<AdministrativeDivisionDto>(`/api/AdministrativeDivisions/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteAdminDivision(id: number): Promise<void> {
  await apiFetch(`/api/AdministrativeDivisions/${id}`, { method: "DELETE" });
}

export async function toggleAdminDivisionEnabled(
  id: number,
  enabled: boolean
): Promise<void> {
  await apiFetch(`/api/AdministrativeDivisions/${id}/enabled`, {
    method: "PATCH",
    body: JSON.stringify({ enabled }),
  });
}
