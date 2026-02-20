"use server";

import { apiFetch } from "@/lib/api";
import type {
  SeasonalMarginDto,
  CreateSeasonalMarginRequest,
  PromoCodeDto,
  CreatePromoCodeRequest,
} from "@/types/pricing";

// --- Seasonal Margins ---

export async function createSeasonalMargin(
  payload: CreateSeasonalMarginRequest
): Promise<SeasonalMarginDto> {
  return apiFetch<SeasonalMarginDto>("/api/SeasonalMargins", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateSeasonalMargin(
  id: number,
  payload: CreateSeasonalMarginRequest
): Promise<SeasonalMarginDto> {
  return apiFetch<SeasonalMarginDto>(`/api/SeasonalMargins/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteSeasonalMargin(id: number): Promise<void> {
  await apiFetch(`/api/SeasonalMargins/${id}`, {
    method: "DELETE",
  });
}

// --- Promo Codes ---

export async function createPromoCode(
  payload: CreatePromoCodeRequest
): Promise<PromoCodeDto> {
  return apiFetch<PromoCodeDto>("/api/PromoCodes", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updatePromoCode(
  id: number,
  payload: CreatePromoCodeRequest
): Promise<PromoCodeDto> {
  return apiFetch<PromoCodeDto>(`/api/PromoCodes/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deletePromoCode(id: number): Promise<void> {
  await apiFetch(`/api/PromoCodes/${id}`, {
    method: "DELETE",
  });
}

export async function togglePromoCodeEnabled(
  id: number,
  enabled: boolean
): Promise<void> {
  await apiFetch(`/api/PromoCodes/${id}/enabled`, {
    method: "PATCH",
    body: JSON.stringify({ enabled }),
  });
}
