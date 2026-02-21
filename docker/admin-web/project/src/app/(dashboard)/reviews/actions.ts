"use server";

import { apiFetch } from "@/lib/api";
import type { ReviewDto, ReviewResponseDto } from "@/types/review";

export async function fetchReviews(): Promise<ReviewDto[]> {
  return apiFetch<ReviewDto[]>("/api/Reviews", { cache: "no-store" });
}

export async function fetchReviewResponses(): Promise<ReviewResponseDto[]> {
  return apiFetch<ReviewResponseDto[]>("/api/ReviewResponses", {
    cache: "no-store",
  });
}

export async function toggleReviewVisibility(
  id: number,
  visible: boolean
): Promise<void> {
  await apiFetch(`/api/Reviews/${id}/visible`, {
    method: "PATCH",
    body: JSON.stringify({ visible }),
  });
}

export async function createReviewResponse(
  reviewId: number,
  userId: number,
  comment: string
): Promise<ReviewResponseDto> {
  return apiFetch<ReviewResponseDto>("/api/ReviewResponses", {
    method: "POST",
    body: JSON.stringify({ reviewId, userId, comment }),
  });
}

export async function updateReviewResponse(
  id: number,
  comment: string
): Promise<ReviewResponseDto> {
  return apiFetch<ReviewResponseDto>(`/api/ReviewResponses/${id}`, {
    method: "PUT",
    body: JSON.stringify({ comment }),
  });
}

export async function deleteReviewResponse(id: number): Promise<void> {
  await apiFetch(`/api/ReviewResponses/${id}`, {
    method: "DELETE",
  });
}
