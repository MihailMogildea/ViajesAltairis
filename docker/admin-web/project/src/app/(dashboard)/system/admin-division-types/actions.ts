"use server";

import { apiFetch } from "@/lib/api";
import type { AdministrativeDivisionTypeDto } from "@/types/system";

export async function getAdminDivisionTypes(): Promise<AdministrativeDivisionTypeDto[]> {
  return apiFetch<AdministrativeDivisionTypeDto[]>("/api/AdministrativeDivisionTypes", {
    cache: "no-store",
  });
}

export async function createAdminDivisionType(payload: {
  name: string;
}): Promise<AdministrativeDivisionTypeDto> {
  return apiFetch<AdministrativeDivisionTypeDto>("/api/AdministrativeDivisionTypes", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateAdminDivisionType(
  id: number,
  payload: { name: string }
): Promise<AdministrativeDivisionTypeDto> {
  return apiFetch<AdministrativeDivisionTypeDto>(
    `/api/AdministrativeDivisionTypes/${id}`,
    {
      method: "PUT",
      body: JSON.stringify(payload),
    }
  );
}

export async function deleteAdminDivisionType(id: number): Promise<void> {
  await apiFetch(`/api/AdministrativeDivisionTypes/${id}`, { method: "DELETE" });
}
