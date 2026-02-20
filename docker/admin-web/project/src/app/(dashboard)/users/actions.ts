"use server";

import { apiFetch } from "@/lib/api";
import type {
  UserDto,
  CreateUserRequest,
} from "@/types/user";

export async function createUser(
  payload: CreateUserRequest
): Promise<UserDto> {
  return apiFetch<UserDto>("/api/Users", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export async function deleteUser(id: number): Promise<void> {
  await apiFetch(`/api/Users/${id}`, {
    method: "DELETE",
  });
}

export async function toggleUserEnabled(
  id: number,
  enabled: boolean
): Promise<void> {
  await apiFetch(`/api/Users/${id}/enabled`, {
    method: "PATCH",
    body: JSON.stringify({ enabled }),
  });
}
