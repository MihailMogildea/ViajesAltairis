"use client";

import type { Review } from "@/types";
import StarRating from "./StarRating";
import { formatDate } from "@/lib/utils";
import { useLocale } from "@/context/LocaleContext";

export default function ReviewCard({ review }: { review: Review }) {
  const { locale, t } = useLocale();

  return (
    <div className="rounded-xl border border-gray-200 p-5">
      <div className="flex items-start justify-between gap-3">
        <div>
          <div className="flex items-center gap-2">
            <span className="font-medium text-gray-900">{review.user_name}</span>
            <StarRating stars={review.rating} size="sm" />
          </div>
          <p className="text-xs text-gray-400">{formatDate(review.created_at, locale)}</p>
        </div>
        <span className="inline-flex h-8 w-8 items-center justify-center rounded-full bg-blue-50 text-sm font-bold text-blue-700">
          {review.rating}
        </span>
      </div>
      {review.title && <h4 className="mt-2 font-medium text-gray-900">{review.title}</h4>}
      <p className={`${review.title ? "mt-1" : "mt-2"} text-sm text-gray-600 leading-relaxed`}>{review.comment}</p>
      {review.response && (
        <div className="mt-3 rounded-lg bg-gray-50 p-3">
          <p className="text-xs font-medium text-gray-500">{t("client.review.response_from")} {review.response.responder_name}</p>
          <p className="mt-1 text-sm text-gray-600">{review.response.comment}</p>
        </div>
      )}
    </div>
  );
}
