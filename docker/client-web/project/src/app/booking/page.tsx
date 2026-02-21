"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useBooking } from "@/context/BookingContext";
import { useAuth } from "@/context/AuthContext";
import { useLocale } from "@/context/LocaleContext";
import { formatPrice, formatDateShort, generateReservationCode } from "@/lib/utils";
import { apiGetPaymentMethods } from "@/lib/api";
import type { ApiPaymentMethod } from "@/types";

type Step = "review" | "payment" | "submitting";

export default function BookingPage() {
  const { items, removeItem, subtotal, promoCode, promoDiscount, userDiscountAmount, taxEstimate, total, applyPromoCode, submitBooking, clearBasket } = useBooking();
  const { user } = useAuth();
  const { locale, currency, t } = useLocale();
  const [promoInput, setPromoInput] = useState("");
  const [promoError, setPromoError] = useState("");
  const [promoLoading, setPromoLoading] = useState(false);
  const [firstName, setFirstName] = useState(user?.first_name || "");
  const [lastName, setLastName] = useState(user?.last_name || "");
  const [email, setEmail] = useState(user?.email || "");
  const [submitError, setSubmitError] = useState("");
  const [step, setStep] = useState<Step>("review");

  // Payment method selection
  const [paymentMethods, setPaymentMethods] = useState<ApiPaymentMethod[]>([]);
  const [selectedMethodId, setSelectedMethodId] = useState<number>(1); // default visa

  useEffect(() => {
    apiGetPaymentMethods()
      .then((methods) => {
        setPaymentMethods(methods);
        if (methods.length > 0) setSelectedMethodId(methods[0].id);
      })
      .catch(() => {});
  }, [locale]);

  const selectedMethod = paymentMethods.find((m) => m.id === selectedMethodId);
  const isBankTransfer = selectedMethod?.code === "bank_transfer";

  // Compute earliest check-in to determine bank transfer eligibility
  const earliestCheckIn = items.length > 0
    ? items.reduce((earliest, item) => (item.check_in < earliest ? item.check_in : earliest), items[0].check_in)
    : "";
  const daysUntilCheckIn = earliestCheckIn
    ? Math.floor((new Date(earliestCheckIn).getTime() - new Date().getTime()) / (1000 * 60 * 60 * 24))
    : 0;

  // Payment form fields (placeholder — no real processing)
  const [cardNumber, setCardNumber] = useState("");
  const [cardExpiry, setCardExpiry] = useState("");
  const [cardCvc, setCardCvc] = useState("");
  const [cardName, setCardName] = useState(user ? `${user.first_name} ${user.last_name}` : "");

  async function handleApplyPromo() {
    setPromoError("");
    if (!promoInput.trim()) return;
    setPromoLoading(true);
    try {
      const ok = await applyPromoCode(promoInput);
      if (!ok) setPromoError(t("client.booking.invalid_promo"));
    } finally {
      setPromoLoading(false);
    }
  }

  function handleProceedToPayment(e: React.FormEvent) {
    e.preventDefault();
    setStep("payment");
  }

  async function handleConfirmPayment(e: React.FormEvent) {
    e.preventDefault();
    setSubmitError("");
    setStep("submitting");

    try {
      const result = await submitBooking(
        currency.code, firstName, lastName, email, selectedMethodId,
        isBankTransfer ? undefined : cardNumber,
        isBankTransfer ? undefined : cardExpiry,
        isBankTransfer ? undefined : cardCvc,
        isBankTransfer ? undefined : cardName
      );

      const params = new URLSearchParams();
      if (result) {
        params.set("code", `VA-${result.reservationId}`);
        params.set("total", result.totalAmount.toFixed(2));
        params.set("status", result.status);
      } else {
        // API unavailable — fall back to client-side confirmation
        params.set("code", generateReservationCode());
        params.set("total", total.toFixed(2));
        params.set("status", "Confirmed");
      }
      params.set("items", String(items.length));
      // Build URL first, then clear basket to avoid race condition
      const confirmUrl = `/booking/confirmation?${params.toString()}`;
      clearBasket();
      window.location.href = confirmUrl;
    } catch {
      setSubmitError("Something went wrong. Please try again.");
      setStep("payment");
    }
  }

  const fp = (amount: number) => formatPrice(amount, currency.code, locale, currency.exchangeRateToEur);

  // Empty basket
  if (items.length === 0) {
    return (
      <div className="mx-auto max-w-2xl px-4 py-16 text-center">
        <div className="text-5xl">&#128722;</div>
        <h1 className="mt-4 text-2xl font-bold text-gray-900">{t("client.booking.empty_title")}</h1>
        <p className="mt-2 text-gray-500">{t("client.booking.empty_desc")}</p>
        <Link href="/hotels" className="mt-6 inline-block rounded-lg bg-blue-600 px-6 py-2.5 text-sm font-semibold text-white hover:bg-blue-700">
          {t("client.booking.browse_hotels")}
        </Link>
      </div>
    );
  }

  // Auth guard — must be logged in to checkout
  if (!user) {
    return (
      <div className="mx-auto max-w-2xl px-4 py-16 text-center">
        <h1 className="text-2xl font-bold text-gray-900">{t("client.booking.login_required")}</h1>
        <p className="mt-2 text-gray-500">
          {t("client.booking.login_message_prefix")} {items.length} {items.length === 1 ? t("client.booking.item") : t("client.booking.items")} ({fp(subtotal)}) {t("client.booking.login_message_suffix")}
        </p>
        <div className="mt-6 flex flex-col gap-3 sm:flex-row sm:justify-center">
          <Link href="/login?redirect=/booking" className="rounded-lg bg-blue-600 px-6 py-2.5 text-sm font-semibold text-white hover:bg-blue-700">
            {t("client.nav.login")}
          </Link>
          <Link href="/register?redirect=/booking" className="rounded-lg border border-gray-300 px-6 py-2.5 text-sm font-medium text-gray-700 hover:bg-gray-50">
            {t("client.nav.create_account")}
          </Link>
        </div>
      </div>
    );
  }

  // Booking summary sidebar (shared between steps)
  const sidebar = (
    <div className="w-full shrink-0 lg:w-72">
      <div className="sticky top-20 rounded-xl border border-gray-200 p-4">
        <h3 className="font-semibold text-gray-900">{t("client.booking.summary")}</h3>
        <div className="mt-3 space-y-2 text-sm">
          {items.map((item) => (
            <div key={item.id} className="flex justify-between text-gray-600">
              <span className="truncate pr-2">{item.hotel_name}</span>
              <span className="shrink-0">{fp(item.line_total)}</span>
            </div>
          ))}
          <div className="border-t border-gray-100 pt-2">
            <div className="flex justify-between text-gray-600">
              <span>{items.length} {items.length === 1 ? t("client.booking.item") : t("client.booking.items")}</span>
              <span>{fp(subtotal)}</span>
            </div>
          </div>
          {promoDiscount > 0 && (
            <div className="flex justify-between text-green-600">
              <span>{t("client.booking.promo_discount")}</span>
              <span>-{fp(promoDiscount)}</span>
            </div>
          )}
          {userDiscountAmount > 0 && (
            <div className="flex justify-between text-green-600">
              <span>{t("client.booking.member_discount")}</span>
              <span>-{fp(userDiscountAmount)}</span>
            </div>
          )}
          {taxEstimate > 0 && (
            <div className="flex justify-between text-gray-600">
              <span>{t("client.booking.tax")}</span>
              <span>{fp(taxEstimate)}</span>
            </div>
          )}
          <div className="border-t border-gray-100 pt-2">
            <div className="flex justify-between text-lg font-bold text-gray-900">
              <span>{t("client.booking.total")}</span>
              <span>{fp(total)}</span>
            </div>
          </div>
        </div>

        {step === "review" && (
          <button
            type="submit"
            form="booking-form"
            className="mt-4 w-full rounded-lg bg-blue-600 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 transition-colors"
          >
            {t("client.booking.proceed")}
          </button>
        )}
        {step === "payment" && (
          <button
            type="submit"
            form="payment-form"
            className={`mt-4 w-full rounded-lg py-2.5 text-sm font-semibold text-white transition-colors ${
              isBankTransfer
                ? "bg-amber-600 hover:bg-amber-700"
                : "bg-green-600 hover:bg-green-700"
            }`}
          >
            {isBankTransfer
              ? t("client.booking.confirm_bank_transfer")
              : `${t("client.booking.pay")} ${fp(total)}`}
          </button>
        )}
        {step === "submitting" && (
          <button
            disabled
            className="mt-4 w-full rounded-lg bg-gray-400 py-2.5 text-sm font-semibold text-white cursor-not-allowed"
          >
            {t("client.booking.processing")}
          </button>
        )}
      </div>
    </div>
  );

  // Step indicator
  const stepIndicator = (
    <div className="mb-6 flex items-center gap-2 text-sm">
      <span className={`rounded-full px-3 py-1 font-medium ${step === "review" ? "bg-blue-600 text-white" : "bg-gray-100 text-gray-500"}`}>
        {t("client.booking.step_review")}
      </span>
      <svg className="h-4 w-4 text-gray-300" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
        <path strokeLinecap="round" strokeLinejoin="round" d="M9 5l7 7-7 7" />
      </svg>
      <span className={`rounded-full px-3 py-1 font-medium ${step === "payment" || step === "submitting" ? "bg-blue-600 text-white" : "bg-gray-100 text-gray-500"}`}>
        {t("client.booking.step_payment")}
      </span>
      <svg className="h-4 w-4 text-gray-300" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
        <path strokeLinecap="round" strokeLinejoin="round" d="M9 5l7 7-7 7" />
      </svg>
      <span className="rounded-full bg-gray-100 px-3 py-1 font-medium text-gray-500">
        {t("client.booking.step_confirmation")}
      </span>
    </div>
  );

  return (
    <div className="mx-auto max-w-4xl px-4 py-8 sm:px-6">
      <h1 className="mb-2 text-2xl font-bold text-gray-900">{t("client.booking.title")}</h1>
      {stepIndicator}

      <div className="flex flex-col gap-6 lg:flex-row">
        <div className="flex-1 space-y-4">
          {step === "review" && (
            <>
              {/* Basket items */}
              {items.map((item) => (
                <div key={item.id} className="rounded-xl border border-gray-200 p-4">
                  <div className="flex items-start justify-between">
                    <div>
                      <h3 className="font-semibold text-gray-900">{item.hotel_name}</h3>
                      <p className="text-sm text-gray-500">
                        {item.room_type_name} &middot; {item.board_type_name} &middot; {item.num_rooms} {item.num_rooms === 1 ? t("client.booking.room") : t("client.booking.rooms")}
                      </p>
                      <p className="text-sm text-gray-500">
                        {formatDateShort(item.check_in, locale)} — {formatDateShort(item.check_out, locale)} ({item.nights} {item.nights === 1 ? t("client.booking.night") : t("client.booking.nights")})
                      </p>
                    </div>
                    <div className="text-right">
                      <p className="font-semibold text-gray-900">{fp(item.line_total)}</p>
                      <button onClick={() => removeItem(item.id)} className="mt-1 text-xs text-red-500 hover:underline">
                        {t("client.booking.remove")}
                      </button>
                    </div>
                  </div>
                </div>
              ))}

              {/* Promo code */}
              <div className="rounded-xl border border-gray-200 p-4">
                <h3 className="mb-2 text-sm font-semibold text-gray-700">{t("client.booking.promo_code")}</h3>
                <div className="flex gap-2">
                  <input
                    type="text"
                    value={promoCode || promoInput}
                    onChange={(e) => setPromoInput(e.target.value)}
                    disabled={!!promoCode || promoLoading}
                    placeholder="e.g. WELCOME10"
                    className="flex-1 rounded-lg border border-gray-300 px-3 py-2 text-sm disabled:bg-gray-50"
                  />
                  {!promoCode && (
                    <button onClick={handleApplyPromo} disabled={promoLoading} className="rounded-lg bg-gray-100 px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-200 disabled:opacity-50">
                      {promoLoading ? "..." : t("client.booking.apply")}
                    </button>
                  )}
                </div>
                {promoError && <p className="mt-1 text-xs text-red-500">{promoError}</p>}
                {promoCode && <p className="mt-1 text-xs text-green-600">Code {promoCode} {t("client.booking.code_applied")}</p>}
              </div>

              {/* Guest info */}
              <form id="booking-form" onSubmit={handleProceedToPayment} className="rounded-xl border border-gray-200 p-4">
                <h3 className="mb-3 text-sm font-semibold text-gray-700">{t("client.booking.guest_info")}</h3>
                <div className="grid gap-3 sm:grid-cols-2">
                  <div>
                    <label className="mb-1 block text-xs text-gray-500">{t("client.booking.first_name")}</label>
                    <input
                      type="text" required value={firstName} onChange={(e) => setFirstName(e.target.value)}
                      className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm"
                    />
                  </div>
                  <div>
                    <label className="mb-1 block text-xs text-gray-500">{t("client.booking.last_name")}</label>
                    <input
                      type="text" required value={lastName} onChange={(e) => setLastName(e.target.value)}
                      className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm"
                    />
                  </div>
                </div>
                <div className="mt-3">
                  <label className="mb-1 block text-xs text-gray-500">{t("client.booking.email")}</label>
                  <input
                    type="email" required value={email} onChange={(e) => setEmail(e.target.value)}
                    className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm"
                  />
                </div>
              </form>
            </>
          )}

          {(step === "payment" || step === "submitting") && (
            <>
              {/* Payment method selector */}
              <div className="rounded-xl border border-gray-200 p-5">
                <h3 className="mb-4 text-lg font-semibold text-gray-900">{t("client.booking.payment_title")}</h3>

                {paymentMethods.length > 0 && (
                  <div className="mb-4 flex flex-wrap gap-2">
                    {paymentMethods.map((method) => {
                      const disabled = method.code === "bank_transfer" && daysUntilCheckIn < method.minDaysBeforeCheckin;
                      const selected = selectedMethodId === method.id;
                      return (
                        <button
                          key={method.id}
                          type="button"
                          disabled={disabled}
                          onClick={() => setSelectedMethodId(method.id)}
                          className={`rounded-lg border px-4 py-2 text-sm font-medium transition-colors ${
                            selected
                              ? "border-blue-500 bg-blue-50 text-blue-700"
                              : disabled
                                ? "border-gray-200 bg-gray-50 text-gray-300 cursor-not-allowed"
                                : "border-gray-300 text-gray-700 hover:bg-gray-50"
                          }`}
                          title={disabled ? t("client.booking.bank_transfer_min_days").replace("{days}", String(method.minDaysBeforeCheckin)) : undefined}
                        >
                          {method.name}
                        </button>
                      );
                    })}
                  </div>
                )}

                {/* Bank transfer — details shown on confirmation page after submit */}
                {isBankTransfer ? (
                  <form id="payment-form" onSubmit={handleConfirmPayment}>
                    <div className="rounded-lg border border-amber-200 bg-amber-50 p-4">
                      <p className="text-sm font-medium text-amber-800">
                        {t("client.booking.bank_transfer_confirm_note")}
                      </p>
                    </div>
                    {submitError && <p className="mt-3 text-sm text-red-600">{submitError}</p>}
                    <p className="mt-4 text-xs text-gray-400">
                      {t("client.booking.demo_disclaimer")}
                    </p>
                  </form>
                ) : (
                  /* Card payment form */
                  <form id="payment-form" onSubmit={handleConfirmPayment}>
                    <p className="mb-4 text-xs text-gray-400">
                      {t("client.booking.payment_secure")}
                    </p>
                    <div className="space-y-3">
                      <div>
                        <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.booking.name_on_card")}</label>
                        <input
                          type="text" required value={cardName} onChange={(e) => setCardName(e.target.value)}
                          className="w-full rounded-lg border border-gray-300 px-3 py-2.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
                          placeholder="John Doe"
                        />
                      </div>
                      <div>
                        <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.booking.card_number")}</label>
                        <input
                          type="text" required value={cardNumber}
                          onChange={(e) => {
                            const v = e.target.value.replace(/\D/g, "").slice(0, 16);
                            setCardNumber(v.replace(/(\d{4})(?=\d)/g, "$1 ").trim());
                          }}
                          className="w-full rounded-lg border border-gray-300 px-3 py-2.5 text-sm font-mono focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
                          placeholder="1234 5678 9012 3456"
                          maxLength={19}
                        />
                      </div>
                      <div className="grid grid-cols-2 gap-3">
                        <div>
                          <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.booking.expiry")}</label>
                          <input
                            type="text" required value={cardExpiry}
                            onChange={(e) => {
                              let v = e.target.value.replace(/\D/g, "").slice(0, 4);
                              if (v.length > 2) v = v.slice(0, 2) + "/" + v.slice(2);
                              setCardExpiry(v);
                            }}
                            className="w-full rounded-lg border border-gray-300 px-3 py-2.5 text-sm font-mono focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
                            placeholder="MM/YY"
                            maxLength={5}
                          />
                        </div>
                        <div>
                          <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.booking.cvc")}</label>
                          <input
                            type="text" required value={cardCvc}
                            onChange={(e) => setCardCvc(e.target.value.replace(/\D/g, "").slice(0, 4))}
                            className="w-full rounded-lg border border-gray-300 px-3 py-2.5 text-sm font-mono focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
                            placeholder="123"
                            maxLength={4}
                          />
                        </div>
                      </div>
                    </div>
                    {submitError && <p className="mt-3 text-sm text-red-600">{submitError}</p>}
                    <p className="mt-4 text-xs text-gray-400">
                      {t("client.booking.demo_disclaimer")}
                    </p>
                  </form>
                )}
              </div>

              <button
                type="button"
                onClick={() => setStep("review")}
                className="text-sm text-gray-500 hover:text-gray-700"
              >
                &larr; {t("client.booking.back_to_review")}
              </button>
            </>
          )}
        </div>

        {sidebar}
      </div>
    </div>
  );
}
