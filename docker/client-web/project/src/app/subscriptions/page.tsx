"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useAuth } from "@/context/AuthContext";
import { useLocale } from "@/context/LocaleContext";
import { formatPrice, formatDate } from "@/lib/utils";
import { apiGetSubscriptionPlans, apiGetMySubscription, apiSubscribe, apiCancelSubscription, apiGetPaymentMethods } from "@/lib/api";
import type { ApiSubscriptionPlan, ApiMySubscription, ApiPaymentMethod } from "@/types";

type Step = "plans" | "payment" | "processing";

const FALLBACK_PLANS: ApiSubscriptionPlan[] = [
  { id: 1, name: "Basic", price: 9.99, discount: 3, currencyId: 1, currencyCode: "EUR" },
  { id: 2, name: "Premium", price: 19.99, discount: 7, currencyId: 1, currencyCode: "EUR" },
  { id: 3, name: "VIP", price: 39.99, discount: 12, currencyId: 1, currencyCode: "EUR" },
];

export default function SubscriptionsPage() {
  const { user, refreshProfile } = useAuth();
  const { locale, currency, t } = useLocale();
  const [plans, setPlans] = useState<ApiSubscriptionPlan[]>([]);
  const [mySub, setMySub] = useState<ApiMySubscription | null>(null);
  const [loaded, setLoaded] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Step flow
  const [step, setStep] = useState<Step>("plans");
  const [selectedPlan, setSelectedPlan] = useState<ApiSubscriptionPlan | null>(null);

  // Payment methods
  const [paymentMethods, setPaymentMethods] = useState<ApiPaymentMethod[]>([]);
  const [selectedMethodId, setSelectedMethodId] = useState<number>(1);
  const selectedMethod = paymentMethods.find((m) => m.id === selectedMethodId);
  const isBankTransfer = selectedMethod?.code === "bank_transfer";

  // Card form
  const [cardNumber, setCardNumber] = useState("");
  const [cardExpiry, setCardExpiry] = useState("");
  const [cardCvc, setCardCvc] = useState("");
  const [cardName, setCardName] = useState("");

  // Cancel
  const [cancelling, setCancelling] = useState(false);

  useEffect(() => {
    apiGetSubscriptionPlans()
      .then((res) => setPlans(res.plans))
      .catch(() => setPlans(FALLBACK_PLANS))
      .finally(() => setLoaded(true));
    apiGetPaymentMethods()
      .then((methods) => {
        setPaymentMethods(methods);
        if (methods.length > 0) setSelectedMethodId(methods[0].id);
      })
      .catch(() => {});
  }, [locale]);

  useEffect(() => {
    if (!user) return;
    apiGetMySubscription()
      .then(setMySub)
      .catch(() => {});
  }, [user, locale]);

  useEffect(() => {
    if (user) {
      setCardName(`${user.first_name} ${user.last_name}`);
    }
  }, [user]);

  function handleSelectPlan(plan: ApiSubscriptionPlan) {
    setSelectedPlan(plan);
    setError(null);
    setStep("payment");
  }

  async function handlePay(e: React.FormEvent) {
    e.preventDefault();
    if (!selectedPlan) return;
    setError(null);
    setStep("processing");

    try {
      await apiSubscribe(
        selectedPlan.id,
        selectedMethodId,
        isBankTransfer ? "" : cardNumber.replace(/\s/g, ""),
        isBankTransfer ? "" : cardExpiry,
        isBankTransfer ? "" : cardCvc,
        isBankTransfer ? "" : cardName
      );
      const sub = await apiGetMySubscription();
      setMySub(sub);
      await refreshProfile();
      // Reset form
      setCardNumber("");
      setCardExpiry("");
      setCardCvc("");
      setStep("plans");
      setSelectedPlan(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Payment failed. Please try again.");
      setStep("payment");
    }
  }

  async function handleCancel() {
    setCancelling(true);
    setError(null);
    try {
      await apiCancelSubscription();
      const sub = await apiGetMySubscription();
      setMySub(sub);
      await refreshProfile();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Cancellation failed. Please try again.");
    } finally {
      setCancelling(false);
    }
  }

  if (!loaded) {
    return (
      <div className="mx-auto max-w-4xl px-4 py-8 sm:px-6">
        <h1 className="mb-6 text-2xl font-bold text-gray-900">{t("client.subscriptions.title")}</h1>
        <p className="text-gray-400">{t("client.common.loading")}</p>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-4xl px-4 py-8 sm:px-6">
      <h1 className="mb-6 text-2xl font-bold text-gray-900">{t("client.subscriptions.title")}</h1>

      {/* Error banner */}
      {error && (
        <div className="mb-6 rounded-lg border border-red-200 bg-red-50 p-4">
          <p className="text-sm text-red-700">{error}</p>
        </div>
      )}

      {/* Current subscription */}
      {user && mySub && mySub.isActive && step === "plans" && (
        <div className="mb-6 rounded-xl border-2 border-blue-200 bg-blue-50 p-5">
          <div className="flex items-start justify-between">
            <div>
              <p className="text-sm font-medium text-blue-700">{t("client.subscriptions.current_plan")}</p>
              <p className="text-lg font-bold text-gray-900">{mySub.planName}</p>
              {mySub.discount && <p className="text-sm text-gray-600">{mySub.discount}% {t("client.subscriptions.discount_off")}</p>}
              {mySub.endDate && <p className="text-xs text-gray-500">{t("client.subscriptions.active_until")} {formatDate(mySub.endDate, locale)}</p>}
            </div>
            <button
              onClick={handleCancel}
              disabled={cancelling}
              className="rounded-lg border border-red-300 px-4 py-2 text-sm font-medium text-red-600 hover:bg-red-50 transition-colors disabled:opacity-50"
            >
              {cancelling ? t("client.common.loading") : t("client.subscriptions.cancel")}
            </button>
          </div>
        </div>
      )}

      {user && mySub && !mySub.isActive && step === "plans" && (
        <div className="mb-6 rounded-xl border border-gray-200 bg-gray-50 p-5">
          <p className="text-sm text-gray-500">{t("client.subscriptions.no_plan")}</p>
        </div>
      )}

      {/* Plan selection */}
      {step === "plans" && (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {plans.map((plan) => {
            const isCurrent = mySub?.isActive && mySub.subscriptionTypeId === plan.id;
            return (
              <div
                key={plan.id}
                className={`rounded-xl border-2 p-5 ${isCurrent ? "border-blue-500 bg-blue-50" : "border-gray-200"}`}
              >
                {isCurrent && (
                  <span className="mb-2 inline-block rounded-full bg-blue-600 px-3 py-0.5 text-xs font-medium text-white">
                    {t("client.subscriptions.current_plan")}
                  </span>
                )}
                <h3 className="text-lg font-bold text-gray-900">{plan.name}</h3>
                <p className="mt-1 text-2xl font-bold text-gray-900">
                  {formatPrice(plan.price, plan.currencyCode || currency.code, locale)}
                  <span className="text-sm font-normal text-gray-500"> / {t("client.subscriptions.per_month")}</span>
                </p>
                <p className="mt-2 text-sm text-green-600 font-medium">{plan.discount}% {t("client.subscriptions.discount_off")}</p>

                {user ? (
                  isCurrent ? (
                    <p className="mt-4 text-center text-sm font-medium text-blue-600">{t("client.subscriptions.subscribed")}</p>
                  ) : (
                    <button
                      onClick={() => handleSelectPlan(plan)}
                      className="mt-4 w-full rounded-lg bg-blue-600 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 transition-colors"
                    >
                      {t("client.subscriptions.subscribe")}
                    </button>
                  )
                ) : (
                  <Link
                    href="/login?redirect=/subscriptions"
                    className="mt-4 block rounded-lg bg-gray-100 py-2.5 text-center text-sm font-medium text-gray-700 hover:bg-gray-200 transition-colors"
                  >
                    {t("client.subscriptions.login_to_subscribe")}
                  </Link>
                )}
              </div>
            );
          })}
        </div>
      )}

      {/* Payment form */}
      {(step === "payment" || step === "processing") && selectedPlan && (
        <div className="mx-auto max-w-md">
          <button
            type="button"
            onClick={() => { setStep("plans"); setError(null); }}
            className="mb-4 text-sm text-gray-500 hover:text-gray-700"
          >
            &larr; {t("client.subscriptions.back_to_plans")}
          </button>

          <div className="rounded-xl border border-gray-200 p-5">
            <div className="mb-4 rounded-lg bg-gray-50 p-3">
              <p className="text-sm text-gray-500">{t("client.subscriptions.subscribing_to")}</p>
              <p className="text-lg font-bold text-gray-900">{selectedPlan.name}</p>
              <p className="text-sm text-gray-600">
                {formatPrice(selectedPlan.price, selectedPlan.currencyCode || currency.code, locale)} / {t("client.subscriptions.per_month")}
              </p>
            </div>

            <form onSubmit={handlePay}>
              <h3 className="mb-4 text-lg font-semibold text-gray-900">{t("client.booking.payment_title")}</h3>

              {/* Payment method selector */}
              {paymentMethods.length > 0 && (
                <div className="mb-4 flex flex-wrap gap-2">
                  {paymentMethods.filter((m) => m.code !== "bank_transfer" || true).map((method) => {
                    // For subscriptions, bank transfer has no min_days constraint
                    const selected = selectedMethodId === method.id;
                    return (
                      <button
                        key={method.id}
                        type="button"
                        disabled={step === "processing"}
                        onClick={() => setSelectedMethodId(method.id)}
                        className={`rounded-lg border px-4 py-2 text-sm font-medium transition-colors ${
                          selected
                            ? "border-blue-500 bg-blue-50 text-blue-700"
                            : "border-gray-300 text-gray-700 hover:bg-gray-50"
                        } disabled:opacity-50`}
                      >
                        {method.name}
                      </button>
                    );
                  })}
                </div>
              )}

              {isBankTransfer ? (
                <div className="rounded-lg border border-amber-200 bg-amber-50 p-4">
                  <p className="mb-3 text-sm font-medium text-amber-800">
                    {t("client.booking.bank_transfer_instructions")}
                  </p>
                  <div className="space-y-2 text-sm text-gray-700">
                    <div className="flex justify-between">
                      <span className="text-gray-500">{t("client.booking.beneficiary")}</span>
                      <span className="font-medium">Viajes Altairis S.L.</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-500">IBAN</span>
                      <span className="font-mono font-medium">ES91 2100 0418 4502 0005 1332</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-500">BIC/SWIFT</span>
                      <span className="font-mono font-medium">CAIXESBBXXX</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-500">{t("client.booking.amount")}</span>
                      <span className="font-bold text-gray-900">
                        {formatPrice(selectedPlan.price, selectedPlan.currencyCode || currency.code, locale)}
                      </span>
                    </div>
                  </div>
                  <p className="mt-3 text-xs text-amber-700">
                    {t("client.booking.bank_transfer_note")}
                  </p>
                </div>
              ) : (
                <>
                  <p className="mb-4 text-xs text-gray-400">{t("client.booking.payment_secure")}</p>
                  <div className="space-y-3">
                    <div>
                      <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.booking.name_on_card")}</label>
                      <input
                        type="text" required value={cardName} onChange={(e) => setCardName(e.target.value)}
                        disabled={step === "processing"}
                        className="w-full rounded-lg border border-gray-300 px-3 py-2.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500 disabled:bg-gray-50"
                        placeholder="John Doe"
                      />
                    </div>
                    <div>
                      <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.booking.card_number")}</label>
                      <input
                        type="text" required value={cardNumber}
                        disabled={step === "processing"}
                        onChange={(e) => {
                          const v = e.target.value.replace(/\D/g, "").slice(0, 16);
                          setCardNumber(v.replace(/(\d{4})(?=\d)/g, "$1 ").trim());
                        }}
                        className="w-full rounded-lg border border-gray-300 px-3 py-2.5 text-sm font-mono focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500 disabled:bg-gray-50"
                        placeholder="1234 5678 9012 3456"
                        maxLength={19}
                      />
                    </div>
                    <div className="grid grid-cols-2 gap-3">
                      <div>
                        <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.booking.expiry")}</label>
                        <input
                          type="text" required value={cardExpiry}
                          disabled={step === "processing"}
                          onChange={(e) => {
                            let v = e.target.value.replace(/\D/g, "").slice(0, 4);
                            if (v.length > 2) v = v.slice(0, 2) + "/" + v.slice(2);
                            setCardExpiry(v);
                          }}
                          className="w-full rounded-lg border border-gray-300 px-3 py-2.5 text-sm font-mono focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500 disabled:bg-gray-50"
                          placeholder="MM/YY"
                          maxLength={5}
                        />
                      </div>
                      <div>
                        <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.booking.cvc")}</label>
                        <input
                          type="text" required value={cardCvc}
                          disabled={step === "processing"}
                          onChange={(e) => setCardCvc(e.target.value.replace(/\D/g, "").slice(0, 4))}
                          className="w-full rounded-lg border border-gray-300 px-3 py-2.5 text-sm font-mono focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500 disabled:bg-gray-50"
                          placeholder="123"
                          maxLength={4}
                        />
                      </div>
                    </div>
                  </div>
                </>
              )}

              <button
                type="submit"
                disabled={step === "processing"}
                className={`mt-5 w-full rounded-lg py-2.5 text-sm font-semibold text-white transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed ${
                  isBankTransfer ? "bg-amber-600 hover:bg-amber-700" : "bg-green-600 hover:bg-green-700"
                }`}
              >
                {step === "processing"
                  ? t("client.booking.processing")
                  : isBankTransfer
                    ? t("client.booking.confirm_bank_transfer")
                    : `${t("client.booking.pay")} ${formatPrice(selectedPlan.price, selectedPlan.currencyCode || currency.code, locale)}`}
              </button>

              <p className="mt-3 text-xs text-gray-400 text-center">{t("client.booking.demo_disclaimer")}</p>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
