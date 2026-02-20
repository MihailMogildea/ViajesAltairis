"use server";

import { apiFetch } from "@/lib/api";
import type { ExchangeRateDto, CurrencyDto } from "@/types/system";

interface CreateExchangeRatePayload {
  currencyId: number;
  rateToEur: number;
  validFrom: string;
  validTo: string;
}

interface UpdateExchangeRatePayload {
  currencyId: number;
  rateToEur: number;
  validFrom: string;
  validTo: string;
}

export async function getExchangeRates(): Promise<ExchangeRateDto[]> {
  return apiFetch<ExchangeRateDto[]>("/api/ExchangeRates", { cache: "no-store" });
}

export async function getCurrencies(): Promise<CurrencyDto[]> {
  return apiFetch<CurrencyDto[]>("/api/Currencies", { cache: "no-store" });
}

export async function createExchangeRate(
  payload: CreateExchangeRatePayload
): Promise<ExchangeRateDto> {
  return apiFetch<ExchangeRateDto>("/api/ExchangeRates", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateExchangeRate(
  id: number,
  payload: UpdateExchangeRatePayload
): Promise<ExchangeRateDto> {
  return apiFetch<ExchangeRateDto>(`/api/ExchangeRates/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteExchangeRate(id: number): Promise<void> {
  await apiFetch(`/api/ExchangeRates/${id}`, { method: "DELETE" });
}
