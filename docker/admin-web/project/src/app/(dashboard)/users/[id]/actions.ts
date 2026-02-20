"use server";

import { apiFetch } from "@/lib/api";
import type { UserDto, UpdateUserRequest, UserHotelDto, UserSubscriptionDto } from "@/types/user";

export async function updateUser(
  id: number,
  payload: UpdateUserRequest
): Promise<UserDto> {
  return apiFetch<UserDto>(`/api/Users/${id}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function createUserHotel(
  userId: number,
  hotelId: number
): Promise<UserHotelDto> {
  return apiFetch<UserHotelDto>("/api/UserHotels", {
    method: "POST",
    body: JSON.stringify({ userId, hotelId }),
  });
}

export async function deleteUserHotel(id: number): Promise<void> {
  await apiFetch(`/api/UserHotels/${id}`, {
    method: "DELETE",
  });
}

export async function createUserSubscription(
  userId: number,
  subscriptionTypeId: number,
  startDate: string,
  endDate: string | null
): Promise<UserSubscriptionDto> {
  return apiFetch<UserSubscriptionDto>("/api/UserSubscriptions", {
    method: "POST",
    body: JSON.stringify({ userId, subscriptionTypeId, startDate, endDate }),
  });
}

export async function deleteUserSubscription(id: number): Promise<void> {
  await apiFetch(`/api/UserSubscriptions/${id}`, {
    method: "DELETE",
  });
}
