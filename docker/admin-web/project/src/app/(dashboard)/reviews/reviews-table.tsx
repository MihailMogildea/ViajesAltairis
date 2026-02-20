"use client";

import { Fragment, useState } from "react";
import { toggleReviewVisibility, deleteReviewResponse } from "./actions";
import type { Column } from "@/components/data-table";
import { StatusBadge } from "@/components/status-badge";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { ToastMessage } from "@/components/toast-message";
import type { ReviewDto, ReviewResponseDto } from "@/types/review";

function renderStars(rating: number) {
  const filled = Math.min(Math.max(Math.round(rating), 0), 5);
  return (
    <span className="text-amber-400">
      {"★".repeat(filled)}
      {"☆".repeat(5 - filled)}
    </span>
  );
}

export function ReviewsTable({
  reviews: initial,
  responses: initialResponses,
  access,
  t,
}: {
  reviews: ReviewDto[];
  responses: ReviewResponseDto[];
  access: string | null;
  t: Record<string, string>;
}) {
  const [items, setItems] = useState(initial);
  const [responses, setResponses] = useState(initialResponses);
  const [expandedId, setExpandedId] = useState<number | null>(null);
  const [pending, setPending] = useState<number | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [deleting, setDeleting] = useState<ReviewResponseDto | null>(null);

  const isFull = access === "full";

  async function handleToggleVisibility(review: ReviewDto) {
    setPending(review.id);
    setMessage(null);
    try {
      const newVisible = !review.visible;
      await toggleReviewVisibility(review.id, newVisible);
      setItems((prev) =>
        prev.map((r) =>
          r.id === review.id ? { ...r, visible: newVisible } : r
        )
      );
      const label = newVisible
        ? t["admin.label.visible"] ?? "Visible"
        : t["admin.label.hidden"] ?? "Hidden";
      setMessage(`Review #${review.id} set to ${label.toLowerCase()}.`);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Toggle failed");
    } finally {
      setPending(null);
    }
  }

  async function handleDeleteResponse() {
    if (!deleting) return;
    setPending(deleting.id);
    setMessage(null);
    try {
      await deleteReviewResponse(deleting.id);
      setResponses((prev) => prev.filter((r) => r.id !== deleting.id));
      setMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setDeleting(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(null);
    }
  }

  function toggleExpand(reviewId: number) {
    setExpandedId((prev) => (prev === reviewId ? null : reviewId));
  }

  const columns: Column<ReviewDto>[] = [
    { key: "id", header: "ID", className: "w-16" },
    { key: "hotelId", header: t["admin.field.hotelId"] ?? "Hotel ID", className: "w-24" },
    { key: "userId", header: t["admin.field.userId"] ?? "User ID", className: "w-24" },
    {
      key: "rating",
      header: t["admin.field.rating"] ?? "Rating",
      render: (item) => renderStars(item.rating),
    },
    {
      key: "title",
      header: t["admin.field.title"] ?? "Title",
      render: (item) => (
        <span className="text-gray-700">{item.title ?? "-"}</span>
      ),
    },
    {
      key: "visible",
      header: t["admin.field.visible"] ?? "Visible",
      render: (item) =>
        isFull ? (
          <StatusBadge
            variant={item.visible ? "enabled" : "disabled"}
            onClick={() => handleToggleVisibility(item)}
            disabled={pending === item.id}
          >
            {item.visible
              ? t["admin.label.visible"] ?? "Visible"
              : t["admin.label.hidden"] ?? "Hidden"}
          </StatusBadge>
        ) : (
          <StatusBadge variant={item.visible ? "enabled" : "disabled"}>
            {item.visible
              ? t["admin.label.visible"] ?? "Visible"
              : t["admin.label.hidden"] ?? "Hidden"}
          </StatusBadge>
        ),
    },
  ];

  // Build a custom table to support expandable response rows
  const reviewRows = items.map((review) => {
    const reviewResponses = responses.filter((r) => r.reviewId === review.id);
    const isExpanded = expandedId === review.id;
    const hasResponses = reviewResponses.length > 0;

    return { review, reviewResponses, isExpanded, hasResponses };
  });

  return (
    <>
      <ToastMessage message={message} onDismiss={() => setMessage(null)} />

      {items.length === 0 ? (
        <div className="rounded-lg border border-gray-200 bg-white p-8 text-center text-sm text-gray-500">
          {t["admin.reviews.empty"] ?? "No reviews found."}
        </div>
      ) : (
        <div className="overflow-hidden rounded-lg border border-gray-200 bg-white">
          <table className="w-full text-left text-sm">
            <thead className="border-b border-gray-200 bg-gray-50 text-xs font-medium uppercase text-gray-500">
              <tr>
                <th className="w-10 px-4 py-3" />
                {columns.map((col) => (
                  <th key={col.key} className={`px-4 py-3 ${col.className ?? ""}`}>
                    {col.header}
                  </th>
                ))}
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {reviewRows.map(({ review, reviewResponses, isExpanded, hasResponses }) => (
                <Fragment key={review.id}>
                  <tr className="hover:bg-gray-50">
                    <td className="px-4 py-3">
                      {hasResponses ? (
                        <button
                          onClick={() => toggleExpand(review.id)}
                          className="text-gray-400 hover:text-gray-600"
                          title={isExpanded ? "Collapse" : "Expand"}
                        >
                          {isExpanded ? "▼" : "▶"}
                        </button>
                      ) : (
                        <span className="text-gray-200">-</span>
                      )}
                    </td>
                    {columns.map((col) => (
                      <td key={col.key} className={`px-4 py-3 ${col.className ?? ""}`}>
                        {col.render
                          ? col.render(review)
                          : String((review as unknown as Record<string, unknown>)[col.key] ?? "")}
                      </td>
                    ))}
                  </tr>
                  {isExpanded &&
                    reviewResponses.map((resp) => (
                      <tr key={`resp-${resp.id}`} className="bg-gray-50">
                        <td className="px-4 py-2" />
                        <td colSpan={columns.length} className="px-4 py-2">
                          <div className="flex items-start justify-between gap-4">
                            <div>
                              <p className="text-sm text-gray-700">{resp.comment}</p>
                              <p className="mt-1 text-xs text-gray-400">
                                {t["admin.field.userId"] ?? "User ID"}: {resp.userId}
                                {" · "}
                                {new Date(resp.createdAt).toLocaleDateString()}
                              </p>
                            </div>
                            {isFull && (
                              <button
                                onClick={() => setDeleting(resp)}
                                disabled={pending === resp.id}
                                className="shrink-0 rounded border border-red-300 px-3 py-1 text-xs font-medium text-red-700 hover:bg-red-50 disabled:opacity-50"
                              >
                                {t["admin.action.delete"] ?? "Delete"}
                              </button>
                            )}
                          </div>
                        </td>
                      </tr>
                    ))}
                </Fragment>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <ConfirmDialog
        open={!!deleting}
        title={t["admin.confirm.delete.title"] ?? "Confirm Delete"}
        message={
          t["admin.confirm.delete.response"] ??
          "Are you sure you want to delete this response?"
        }
        onConfirm={handleDeleteResponse}
        onCancel={() => setDeleting(null)}
        loading={pending === deleting?.id}
        confirmLabel={t["admin.action.delete"] ?? "Delete"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      />
    </>
  );
}
