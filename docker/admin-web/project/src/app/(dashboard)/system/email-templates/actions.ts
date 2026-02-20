"use server";

import { apiFetch } from "@/lib/api";
import { EmailTemplateDto } from "@/types/system";

export async function createEmailTemplate(
  payload: { name: string }
): Promise<EmailTemplateDto> {
  return apiFetch<EmailTemplateDto>("/api/EmailTemplates", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateEmailTemplate(
  id: number,
  payload: { name: string }
): Promise<EmailTemplateDto> {
  return apiFetch<EmailTemplateDto>(`/api/EmailTemplates/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteEmailTemplate(id: number): Promise<void> {
  await apiFetch(`/api/EmailTemplates/${id}`, { method: "DELETE" });
}
