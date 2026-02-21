import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import type { ReviewDto, ReviewResponseDto } from "@/types/review";
import { ReviewsTable } from "./reviews-table";

export default async function ReviewsPage() {
  const session = await getSession();
  const access = session ? getAccessLevel(session.role, "reviews") : null;
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let reviews: ReviewDto[] = [];
  let responses: ReviewResponseDto[] = [];
  let error: string | null = null;

  try {
    [reviews, responses] = await Promise.all([
      apiFetch<ReviewDto[]>("/api/Reviews", { cache: "no-store" }),
      apiFetch<ReviewResponseDto[]>("/api/ReviewResponses", {
        cache: "no-store",
      }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load reviews";
  }

  return (
    <div>
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">
          {t["admin.section.reviews"] ?? "Reviews"}
        </h2>
        <p className="mt-1 text-sm text-gray-500">
          {t["admin.reviews.desc"] ?? "View and moderate customer reviews."}
        </p>
      </div>

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <ReviewsTable
          reviews={reviews}
          responses={responses}
          access={access}
          currentUserId={session?.userId ?? 0}
          t={t}
        />
      )}
    </div>
  );
}
