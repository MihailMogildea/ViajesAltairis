"use server";

import { apiFetch } from "@/lib/api";
import type { SubscriptionTypeDto, CreateSubscriptionTypeRequest } from "@/types/subscription";
import type { UserSubscriptionDto } from "@/types/user";

// --- Subscription Types ---

export async function createSubscriptionType(
  payload: CreateSubscriptionTypeRequest
): Promise<SubscriptionTypeDto> {
  return apiFetch<SubscriptionTypeDto>("/api/SubscriptionTypes", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function updateSubscriptionType(
  id: number,
  payload: CreateSubscriptionTypeRequest
): Promise<SubscriptionTypeDto> {
  return apiFetch<SubscriptionTypeDto>(`/api/SubscriptionTypes/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function deleteSubscriptionType(id: number): Promise<void> {
  await apiFetch(`/api/SubscriptionTypes/${id}`, {
    method: "DELETE",
  });
}

export async function toggleSubscriptionTypeEnabled(
  id: number
): Promise<SubscriptionTypeDto> {
  return apiFetch<SubscriptionTypeDto>(`/api/SubscriptionTypes/${id}/enabled`, {
    method: "PATCH",
  });
}

// --- User Subscriptions ---

interface CreateUserSubscriptionPayload {
  userId: number;
  subscriptionTypeId: number;
  startDate: string;
  endDate: string | null;
}

export async function createUserSubscription(
  payload: CreateUserSubscriptionPayload
): Promise<UserSubscriptionDto> {
  return apiFetch<UserSubscriptionDto>("/api/UserSubscriptions", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function deleteUserSubscription(id: number): Promise<void> {
  await apiFetch(`/api/UserSubscriptions/${id}`, {
    method: "DELETE",
  });
}
