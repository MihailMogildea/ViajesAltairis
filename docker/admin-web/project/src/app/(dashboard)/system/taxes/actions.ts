"use server";

import { apiFetch } from "@/lib/api";
import { TaxDto, TaxTypeDto } from "@/types/system";

// --- Tax actions ---

interface CreateTaxPayload {
  taxTypeId: number;
  countryId: number | null;
  administrativeDivisionId: number | null;
  cityId: number | null;
  rate: number;
  isPercentage: boolean;
}

export async function createTax(payload: CreateTaxPayload): Promise<TaxDto> {
  return apiFetch<TaxDto>("/api/Taxes", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateTax(
  id: number,
  payload: CreateTaxPayload
): Promise<TaxDto> {
  return apiFetch<TaxDto>(`/api/Taxes/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteTax(id: number): Promise<void> {
  await apiFetch(`/api/Taxes/${id}`, { method: "DELETE" });
}

export async function toggleTaxEnabled(id: number): Promise<TaxDto> {
  return apiFetch<TaxDto>(`/api/Taxes/${id}/enabled`, {
    method: "PATCH",
  });
}

// --- Tax Type actions ---

export async function createTaxType(
  payload: { name: string }
): Promise<TaxTypeDto> {
  return apiFetch<TaxTypeDto>("/api/TaxTypes", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateTaxType(
  id: number,
  payload: { name: string }
): Promise<TaxTypeDto> {
  return apiFetch<TaxTypeDto>(`/api/TaxTypes/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteTaxType(id: number): Promise<void> {
  await apiFetch(`/api/TaxTypes/${id}`, { method: "DELETE" });
}
