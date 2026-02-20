"use server";

import { apiFetch } from "@/lib/api";
import { TranslationDto, WebTranslationDto } from "@/types/system";

// --- Translation actions ---

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

export async function deleteTranslation(id: number): Promise<void> {
  await apiFetch(`/api/Translations/${id}`, { method: "DELETE" });
}

// --- Web Translation actions ---

interface WebTranslationPayload {
  translationKey: string;
  languageId: number;
  value: string;
}

export async function createWebTranslation(
  payload: WebTranslationPayload
): Promise<WebTranslationDto> {
  return apiFetch<WebTranslationDto>("/api/WebTranslations", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateWebTranslation(
  id: number,
  payload: WebTranslationPayload
): Promise<WebTranslationDto> {
  return apiFetch<WebTranslationDto>(`/api/WebTranslations/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteWebTranslation(id: number): Promise<void> {
  await apiFetch(`/api/WebTranslations/${id}`, { method: "DELETE" });
}
