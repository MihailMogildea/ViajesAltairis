"use server";

import { apiFetch } from "@/lib/api";
import type { CurrencyDto } from "@/types/system";

interface CreateCurrencyPayload {
  isoCode: string;
  name: string;
  symbol: string;
}

interface UpdateCurrencyPayload {
  isoCode: string;
  name: string;
  symbol: string;
}

export async function createCurrency(
  payload: CreateCurrencyPayload
): Promise<CurrencyDto> {
  return apiFetch<CurrencyDto>("/api/Currencies", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateCurrency(
  id: number,
  payload: UpdateCurrencyPayload
): Promise<CurrencyDto> {
  return apiFetch<CurrencyDto>(`/api/Currencies/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteCurrency(id: number): Promise<void> {
  await apiFetch(`/api/Currencies/${id}`, {
    method: "DELETE",
  });
}
