"use server";

import { apiFetch } from "@/lib/api";
import type { LanguageDto } from "@/types/system";

interface CreateLanguagePayload {
  isoCode: string;
  name: string;
}

interface UpdateLanguagePayload {
  isoCode: string;
  name: string;
}

export async function createLanguage(
  payload: CreateLanguagePayload
): Promise<LanguageDto> {
  return apiFetch<LanguageDto>("/api/Languages", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateLanguage(
  id: number,
  payload: UpdateLanguagePayload
): Promise<LanguageDto> {
  return apiFetch<LanguageDto>(`/api/Languages/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteLanguage(id: number): Promise<void> {
  await apiFetch(`/api/Languages/${id}`, {
    method: "DELETE",
  });
}
