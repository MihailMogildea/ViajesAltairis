"use client";

import { Fragment, useState } from "react";
import { toggleReviewVisibility, deleteReviewResponse, createReviewResponse, updateReviewResponse } from "./actions";
import type { Column } from "@/components/data-table";
import { StatusBadge } from "@/components/status-badge";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { FormModal } from "@/components/form-modal";
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
  currentUserId,
  t,
}: {
  reviews: ReviewDto[];
  responses: ReviewResponseDto[];
  access: string | null;
  currentUserId: number;
  t: Record<string, string>;
}) {
  const [items, setItems] = useState(initial);
  const [responses, setResponses] = useState(initialResponses);
  const [expandedId, setExpandedId] = useState<number | null>(null);
  const [pending, setPending] = useState<number | null>(null);
  const [message, setMessage] = useState<string | null>(null);
  const [deleting, setDeleting] = useState<ReviewResponseDto | null>(null);

  // Reply / Edit modal state
  const [replyReviewId, setReplyReviewId] = useState<number | null>(null);
  const [editingResponse, setEditingResponse] = useState<ReviewResponseDto | null>(null);
  const [commentText, setCommentText] = useState("");
  const [modalPending, setModalPending] = useState(false);

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

  function openReply(reviewId: number) {
    setReplyReviewId(reviewId);
    setEditingResponse(null);
    setCommentText("");
    setExpandedId(reviewId);
  }

  function openEdit(resp: ReviewResponseDto) {
    setEditingResponse(resp);
    setReplyReviewId(null);
    setCommentText(resp.comment);
  }

  function closeModal() {
    setReplyReviewId(null);
    setEditingResponse(null);
    setCommentText("");
  }

  async function handleSaveModal() {
    if (!commentText.trim()) return;
    setModalPending(true);
    setMessage(null);
    try {
      if (replyReviewId) {
        const created = await createReviewResponse(replyReviewId, currentUserId, commentText.trim());
        setResponses((prev) => [...prev, created]);
        setMessage(t["admin.reviews.reply_saved"] ?? "Reply saved.");
      } else if (editingResponse) {
        const updated = await updateReviewResponse(editingResponse.id, commentText.trim());
        setResponses((prev) =>
          prev.map((r) => (r.id === updated.id ? updated : r))
        );
        setMessage(t["admin.reviews.response_updated"] ?? "Response updated.");
      }
      closeModal();
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Save failed");
    } finally {
      setModalPending(false);
    }
  }

  function toggleExpand(reviewId: number) {
    setExpandedId((prev) => (prev === reviewId ? null : reviewId));
  }

  const columns: Column<ReviewDto>[] = [
    { key: "id", header: "ID", className: "w-16" },
    { key: "hotelId", header: t["admin.field.hotelId"] ?? "Hotel ID", className: "w-24" },
    { key: "userEmail", header: t["admin.field.userEmail"] ?? "User", className: "w-48" },
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

  const modalOpen = replyReviewId !== null || editingResponse !== null;
  const modalTitle = replyReviewId
    ? `${t["admin.reviews.reply_to"] ?? "Reply to Review"} #${replyReviewId}`
    : `${t["admin.reviews.edit_response"] ?? "Edit Response"}`;

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
                {isFull && <th className="w-24 px-4 py-3" />}
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
                    {isFull && (
                      <td className="px-4 py-3">
                        {!hasResponses && (
                          <button
                            onClick={() => openReply(review.id)}
                            className="rounded border border-blue-300 px-3 py-1 text-xs font-medium text-blue-700 hover:bg-blue-50"
                          >
                            {t["admin.reviews.reply"] ?? "Reply"}
                          </button>
                        )}
                      </td>
                    )}
                  </tr>
                  {isExpanded &&
                    reviewResponses.map((resp) => (
                      <tr key={`resp-${resp.id}`} className="bg-gray-50">
                        <td className="px-4 py-2" />
                        <td colSpan={columns.length + (isFull ? 1 : 0)} className="px-4 py-2">
                          <div className="flex items-start justify-between gap-4">
                            <div>
                              <p className="text-sm text-gray-700">{resp.comment}</p>
                              <p className="mt-1 text-xs text-gray-400">
                                {resp.userEmail}
                                {" · "}
                                {new Date(resp.createdAt).toLocaleDateString()}
                              </p>
                            </div>
                            {isFull && (
                              <div className="flex shrink-0 gap-2">
                                <button
                                  onClick={() => openEdit(resp)}
                                  className="rounded border border-gray-300 px-3 py-1 text-xs font-medium text-gray-700 hover:bg-gray-100"
                                >
                                  {t["admin.action.edit"] ?? "Edit"}
                                </button>
                                <button
                                  onClick={() => setDeleting(resp)}
                                  disabled={pending === resp.id}
                                  className="rounded border border-red-300 px-3 py-1 text-xs font-medium text-red-700 hover:bg-red-50 disabled:opacity-50"
                                >
                                  {t["admin.action.delete"] ?? "Delete"}
                                </button>
                              </div>
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

      <FormModal
        open={modalOpen}
        title={modalTitle}
        onClose={closeModal}
        onSubmit={handleSaveModal}
        loading={modalPending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">
            {t["admin.field.comment"] ?? "Comment"}
          </label>
          <textarea
            value={commentText}
            onChange={(e) => setCommentText(e.target.value)}
            rows={4}
            className="w-full rounded border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
          />
        </div>
      </FormModal>

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
