"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useAuth } from "@/context/AuthContext";
import { useLocale } from "@/context/LocaleContext";
import { formatPrice, formatDate, calculateNights } from "@/lib/utils";
import { useRouter } from "next/navigation";
import { apiGetMyReservations, apiGetReservationDetail, apiCancelReservation, apiSubmitReview, apiGenerateInvoice } from "@/lib/api";
import type { Reservation, ApiReservationSummary, ApiReservationDetail } from "@/types";

const sampleReservations: Reservation[] = [
  {
    id: 1, code: "VA-X8K2M4NP", user_id: 8, status: "completed",
    owner_first_name: "Juan", owner_last_name: "Martínez", owner_email: "client1@example.com",
    currency_code: "EUR", exchange_rate: 1, subtotal: 540, discount_total: 0, tax_total: 54, total: 594,
    created_at: "2026-01-10",
    lines: [
      { id: 1, reservation_id: 1, hotel_name: "Hotel Altairis Palma", room_type_name: "Double", board_type_name: "Bed & Breakfast", check_in: "2026-01-12", check_out: "2026-01-15", num_rooms: 1, price_per_night: 180, nights: 3, line_total: 540, guests: [{ id: 1, first_name: "Juan", last_name: "Martínez" }, { id: 2, first_name: "Laura", last_name: "Martínez" }] },
    ],
  },
  {
    id: 2, code: "VA-R3F7H9WQ", user_id: 8, status: "confirmed",
    owner_first_name: "Juan", owner_last_name: "Martínez", owner_email: "client1@example.com",
    currency_code: "EUR", exchange_rate: 1, subtotal: 1200, discount_total: 84, tax_total: 111.6, total: 1227.6,
    promo_code: "WELCOME10",
    created_at: "2026-02-15",
    lines: [
      { id: 3, reservation_id: 2, hotel_name: "Hotel Son Vida Palace", room_type_name: "Suite", board_type_name: "Full Board", check_in: "2026-06-01", check_out: "2026-06-04", num_rooms: 1, price_per_night: 420, nights: 3, line_total: 1260, guests: [{ id: 5, first_name: "Juan", last_name: "Martínez" }, { id: 6, first_name: "Laura", last_name: "Martínez" }] },
    ],
  },
  {
    id: 3, code: "VA-T5P2J8LC", user_id: 9, status: "pending",
    owner_first_name: "Emma", owner_last_name: "Wilson", owner_email: "client2@example.com",
    currency_code: "EUR", exchange_rate: 1, subtotal: 460, discount_total: 0, tax_total: 46, total: 506,
    created_at: "2026-02-18",
    lines: [
      { id: 5, reservation_id: 3, hotel_name: "Hotel Promenade Nice", room_type_name: "Double", board_type_name: "Half Board", check_in: "2026-07-10", check_out: "2026-07-12", num_rooms: 1, price_per_night: 230, nights: 2, line_total: 460, guests: [{ id: 9, first_name: "Emma", last_name: "Wilson" }] },
    ],
  },
];

const statusColors: Record<string, string> = {
  draft: "bg-gray-100 text-gray-600",
  pending: "bg-yellow-100 text-yellow-700",
  confirmed: "bg-green-100 text-green-700",
  checked_in: "bg-blue-100 text-blue-700",
  completed: "bg-purple-100 text-purple-700",
  cancelled: "bg-red-100 text-red-700",
};

function mapApiSummaryToReservation(s: ApiReservationSummary): Reservation {
  return {
    id: s.id,
    code: `VA-${s.id}`,
    user_id: 0,
    status: s.status,
    owner_first_name: "",
    owner_last_name: "",
    owner_email: "",
    currency_code: s.currency,
    exchange_rate: 1,
    subtotal: s.totalAmount,
    discount_total: 0,
    tax_total: 0,
    total: s.totalAmount,
    created_at: s.createdAt,
    lines: [],
    hotel_names: s.hotelNames ?? undefined,
  };
}

function applyApiDetail(res: Reservation, detail: ApiReservationDetail): Reservation {
  return {
    ...res,
    subtotal: detail.totalAmount + detail.totalDiscount,
    discount_total: detail.totalDiscount,
    total: detail.totalAmount,
    promo_code: detail.promoCode ?? undefined,
    lines: detail.lines.map((l) => ({
      id: l.id,
      reservation_id: detail.id,
      hotel_name: l.hotelName,
      room_type_name: l.roomType,
      board_type_name: l.boardType,
      check_in: l.checkIn,
      check_out: l.checkOut,
      num_rooms: 1,
      price_per_night: 0,
      nights: calculateNights(l.checkIn, l.checkOut),
      line_total: l.lineTotal,
      guests: (l.guests ?? []).map((g) => ({
        id: g.id,
        first_name: g.firstName,
        last_name: g.lastName,
      })),
    })),
  };
}

export default function ReservationsPage() {
  const { user } = useAuth();
  const router = useRouter();
  const { locale, t } = useLocale();
  const [expandedId, setExpandedId] = useState<number | null>(null);
  const [reservations, setReservations] = useState<Reservation[]>([]);
  const [loaded, setLoaded] = useState(false);
  const [cancellingId, setCancellingId] = useState<number | null>(null);

  // Review modal state
  const [reviewLineId, setReviewLineId] = useState<number | null>(null);
  const [reviewRating, setReviewRating] = useState(5);
  const [reviewTitle, setReviewTitle] = useState("");
  const [reviewComment, setReviewComment] = useState("");
  const [reviewSubmitting, setReviewSubmitting] = useState(false);
  const [reviewSuccess, setReviewSuccess] = useState<Set<number>>(new Set());
  const [generatingInvoiceId, setGeneratingInvoiceId] = useState<number | null>(null);

  useEffect(() => {
    if (!user) return;

    apiGetMyReservations()
      .then((res) => {
        setReservations(res.reservations.map(mapApiSummaryToReservation));
        setLoaded(true);
      })
      .catch(() => {
        setReservations([]);
        setLoaded(true);
      });
  }, [user, locale]);

  // Fetch detail when expanding a reservation
  useEffect(() => {
    if (expandedId === null) return;
    apiGetReservationDetail(expandedId)
      .then((detail) => {
        setReservations((prev) =>
          prev.map((r) => (r.id === expandedId ? applyApiDetail(r, detail) : r))
        );
      })
      .catch(() => { /* keep what we have */ });
  }, [expandedId, locale]);

  async function handleCancel(id: number) {
    setCancellingId(id);
    try {
      await apiCancelReservation(id);
      setReservations((prev) =>
        prev.map((r) => (r.id === id ? { ...r, status: "cancelled" } : r))
      );
    } catch {
      // API unavailable — update locally anyway for demo
      setReservations((prev) =>
        prev.map((r) => (r.id === id ? { ...r, status: "cancelled" } : r))
      );
    } finally {
      setCancellingId(null);
    }
  }

  async function handleReviewSubmit() {
    if (!reviewLineId) return;
    setReviewSubmitting(true);
    try {
      await apiSubmitReview({
        reservationLineId: reviewLineId,
        rating: reviewRating,
        title: reviewTitle || undefined,
        comment: reviewComment || undefined,
      });
      setReviewSuccess((prev) => new Set(prev).add(reviewLineId));
    } catch {
      // silently fail — API may reject duplicates
    } finally {
      setReviewSubmitting(false);
      setReviewLineId(null);
      setReviewRating(5);
      setReviewTitle("");
      setReviewComment("");
    }
  }

  async function handleGenerateInvoice(reservationId: number) {
    setGeneratingInvoiceId(reservationId);
    try {
      await apiGenerateInvoice(reservationId);
      router.push("/invoices");
    } catch {
      // silently fail
    } finally {
      setGeneratingInvoiceId(null);
    }
  }

  if (!user) {
    return (
      <div className="mx-auto max-w-lg px-4 py-16 text-center">
        <h1 className="text-2xl font-bold text-gray-900">{t("client.reservations.title")}</h1>
        <p className="mt-2 text-gray-500">{t("client.reservations.login_message")}</p>
        <Link href="/login" className="mt-4 inline-block rounded-lg bg-blue-600 px-6 py-2.5 text-sm font-semibold text-white hover:bg-blue-700">
          {t("client.common.login")}
        </Link>
      </div>
    );
  }

  if (!loaded) {
    return (
      <div className="mx-auto max-w-3xl px-4 py-8 sm:px-6">
        <h1 className="mb-6 text-2xl font-bold text-gray-900">{t("client.reservations.title")}</h1>
        <p className="text-gray-400">{t("client.reservations.loading")}</p>
      </div>
    );
  }

  if (reservations.length === 0) {
    return (
      <div className="mx-auto max-w-lg px-4 py-16 text-center">
        <h1 className="text-2xl font-bold text-gray-900">{t("client.reservations.title")}</h1>
        <p className="mt-2 text-gray-500">{t("client.reservations.empty")}</p>
        <Link href="/hotels" className="mt-4 inline-block rounded-lg bg-blue-600 px-6 py-2.5 text-sm font-semibold text-white hover:bg-blue-700">
          {t("client.reservations.browse")}
        </Link>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-3xl px-4 py-8 sm:px-6">
      <h1 className="mb-6 text-2xl font-bold text-gray-900">{t("client.reservations.title")}</h1>
      <div className="space-y-4">
        {reservations.map((res) => (
          <div key={res.id} className="rounded-xl border border-gray-200 overflow-hidden">
            <button
              onClick={() => setExpandedId(expandedId === res.id ? null : res.id)}
              className="flex w-full items-center justify-between p-5 text-left hover:bg-gray-50 transition-colors"
            >
              <div className="flex-1">
                <div className="flex flex-wrap items-center gap-2">
                  <span className="font-mono text-sm font-semibold text-gray-900">{res.code}</span>
                  <span className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${statusColors[res.status]}`}>
                    {t(`client.status.${res.status}`)}
                  </span>
                </div>
                <p className="mt-1 text-sm text-gray-500">
                  {res.lines.length > 0 ? res.lines.map((l) => l.hotel_name).join(", ") : res.hotel_names ?? t("client.reservations.loading_details")}
                </p>
                <p className="text-xs text-gray-400">{t("client.reservations.booked")} {formatDate(res.created_at, locale)}</p>
              </div>
              <div className="ml-4 text-right">
                <p className="text-lg font-bold text-gray-900">{formatPrice(res.total, res.currency_code, locale)}</p>
                <svg className={`ml-auto h-5 w-5 text-gray-400 transition-transform ${expandedId === res.id ? "rotate-180" : ""}`} fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                  <path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7" />
                </svg>
              </div>
            </button>

            {expandedId === res.id && (
              <div className="border-t border-gray-100 bg-gray-50 p-5">
                {res.lines.length === 0 ? (
                  <p className="text-sm text-gray-400">{t("client.reservations.loading_details")}</p>
                ) : (
                  <>
                    {res.lines.map((line) => (
                      <div key={line.id} className="mb-4 last:mb-0">
                        <div className="flex justify-between">
                          <div>
                            <p className="font-medium text-gray-900">{line.hotel_name}</p>
                            <p className="text-sm text-gray-500">
                              {line.room_type_name} &middot; {line.board_type_name} &middot; {line.num_rooms} {line.num_rooms === 1 ? t("client.reservations.room") : t("client.reservations.rooms")}
                            </p>
                            <p className="text-sm text-gray-500">
                              {formatDate(line.check_in, locale)} — {formatDate(line.check_out, locale)} ({line.nights} {line.nights === 1 ? t("client.booking.night") : t("client.booking.nights")})
                            </p>
                          </div>
                          <p className="font-medium text-gray-900">{formatPrice(line.line_total, res.currency_code, locale)}</p>
                        </div>
                        {line.guests.length > 0 && (
                          <div className="mt-2">
                            <p className="text-xs font-medium text-gray-400">{t("client.reservations.guests")}</p>
                            <p className="text-sm text-gray-600">
                              {line.guests.map((g) => `${g.first_name} ${g.last_name}`).join(", ")}
                            </p>
                          </div>
                        )}
                        {res.status === "completed" && (
                          <div className="mt-2">
                            {reviewSuccess.has(line.id) ? (
                              <span className="text-xs font-medium text-green-600">{t("client.reservations.review_submitted")}</span>
                            ) : (
                              <button
                                onClick={() => { setReviewLineId(line.id); setReviewRating(5); setReviewTitle(""); setReviewComment(""); }}
                                className="rounded bg-purple-50 px-3 py-1 text-xs font-medium text-purple-600 hover:bg-purple-100 transition-colors"
                              >
                                {t("client.reservations.write_review")}
                              </button>
                            )}
                          </div>
                        )}
                      </div>
                    ))}
                    {res.promo_code && (
                      <p className="mt-2 text-sm text-green-600">{t("client.reservations.promo_code")} {res.promo_code}</p>
                    )}
                    <div className="mt-3 border-t border-gray-200 pt-3 text-sm">
                      <div className="flex justify-between text-gray-500">
                        <span>{t("client.reservations.subtotal")}</span><span>{formatPrice(res.subtotal, res.currency_code, locale)}</span>
                      </div>
                      {res.discount_total > 0 && (
                        <div className="flex justify-between text-green-600">
                          <span>{t("client.reservations.discount")}</span><span>-{formatPrice(res.discount_total, res.currency_code, locale)}</span>
                        </div>
                      )}
                      <div className="flex justify-between text-gray-500">
                        <span>{t("client.reservations.tax")}</span><span>{formatPrice(res.tax_total, res.currency_code, locale)}</span>
                      </div>
                      <div className="flex justify-between font-bold text-gray-900">
                        <span>{t("client.reservations.total")}</span><span>{formatPrice(res.total, res.currency_code, locale)}</span>
                      </div>
                    </div>

                    {/* Action buttons for pending/confirmed/completed */}
                    {(res.status === "pending" || res.status === "confirmed" || res.status === "completed") && (
                      <div className="mt-4 border-t border-gray-200 pt-3 flex gap-2">
                        {(res.status === "pending" || res.status === "confirmed") && (
                          <button
                            onClick={() => handleCancel(res.id)}
                            disabled={cancellingId === res.id}
                            className="rounded-lg bg-red-50 px-4 py-2 text-sm font-medium text-red-600 hover:bg-red-100 transition-colors disabled:opacity-50"
                          >
                            {cancellingId === res.id ? t("client.reservations.cancelling") : t("client.reservations.cancel")}
                          </button>
                        )}
                        {(res.status === "confirmed" || res.status === "completed") && (
                          <button
                            onClick={() => handleGenerateInvoice(res.id)}
                            disabled={generatingInvoiceId === res.id}
                            className="rounded-lg bg-blue-50 px-4 py-2 text-sm font-medium text-blue-600 hover:bg-blue-100 transition-colors disabled:opacity-50"
                          >
                            {generatingInvoiceId === res.id ? t("client.reservations.generating_invoice") : t("client.reservations.export_invoice")}
                          </button>
                        )}
                      </div>
                    )}
                  </>
                )}
              </div>
            )}
          </div>
        ))}
      </div>

      {/* Review modal */}
      {reviewLineId !== null && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
          <div className="mx-4 w-full max-w-md rounded-xl bg-white p-6 shadow-xl">
            <h3 className="text-lg font-semibold text-gray-900">{t("client.reservations.write_review")}</h3>

            {/* Star rating */}
            <div className="mt-4">
              <p className="mb-1 text-sm font-medium text-gray-700">{t("client.reservations.rating")}</p>
              <div className="flex gap-1">
                {[1, 2, 3, 4, 5].map((star) => (
                  <button
                    key={star}
                    onClick={() => setReviewRating(star)}
                    className={`text-2xl ${star <= reviewRating ? "text-yellow-400" : "text-gray-300"}`}
                  >
                    &#9733;
                  </button>
                ))}
              </div>
            </div>

            <div className="mt-3">
              <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.reservations.review_title")}</label>
              <input
                type="text"
                value={reviewTitle}
                onChange={(e) => setReviewTitle(e.target.value)}
                placeholder={t("client.reservations.review_title_placeholder")}
                className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
              />
            </div>

            <div className="mt-3">
              <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.reservations.review_comment")}</label>
              <textarea
                value={reviewComment}
                onChange={(e) => setReviewComment(e.target.value)}
                placeholder={t("client.reservations.review_comment_placeholder")}
                rows={3}
                className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
              />
            </div>

            <div className="mt-4 flex justify-end gap-2">
              <button
                onClick={() => setReviewLineId(null)}
                className="rounded-lg px-4 py-2 text-sm font-medium text-gray-600 hover:bg-gray-100 transition-colors"
              >
                {t("client.reservations.cancel_review")}
              </button>
              <button
                onClick={handleReviewSubmit}
                disabled={reviewSubmitting}
                className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700 transition-colors disabled:opacity-50"
              >
                {reviewSubmitting ? t("client.reservations.submitting_review") : t("client.reservations.submit_review")}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
