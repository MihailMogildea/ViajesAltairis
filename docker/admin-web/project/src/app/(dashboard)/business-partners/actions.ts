"use server";

import { apiFetch } from "@/lib/api";
import type {
  BusinessPartnerDto,
  CreateBusinessPartnerRequest,
} from "@/types/business-partner";

export async function createBusinessPartner(
  payload: CreateBusinessPartnerRequest
): Promise<BusinessPartnerDto> {
  return apiFetch<BusinessPartnerDto>("/api/BusinessPartners", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateBusinessPartner(
  id: number,
  payload: CreateBusinessPartnerRequest
): Promise<BusinessPartnerDto> {
  return apiFetch<BusinessPartnerDto>(`/api/BusinessPartners/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteBusinessPartner(id: number): Promise<void> {
  await apiFetch(`/api/BusinessPartners/${id}`, {
    method: "DELETE",
  });
}

export async function toggleBusinessPartnerEnabled(
  id: number,
  enabled: boolean
): Promise<void> {
  await apiFetch(`/api/BusinessPartners/${id}/enabled`, {
    method: "PATCH",
    body: JSON.stringify({ enabled }),
  });
}
