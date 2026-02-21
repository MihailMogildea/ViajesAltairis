"use server";

import { apiFetch } from "@/lib/api";
import { TranslationDto } from "@/types/system";

interface TranslationPayload {
  entityType: string;
  entityId: number;
  field: string;
  languageId: number;
  value: string;
}

export async function createTranslation(
  payload: TranslationPayload
): Promise<TranslationDto> {
  return apiFetch<TranslationDto>("/api/Translations", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateTranslation(
  id: number,
  payload: TranslationPayload
): Promise<TranslationDto> {
  return apiFetch<TranslationDto>(`/api/Translations/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}
