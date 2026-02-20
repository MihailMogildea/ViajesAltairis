"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useAuth } from "@/context/AuthContext";
import { useLocale } from "@/context/LocaleContext";
import { formatPrice, formatDate } from "@/lib/utils";
import { apiGetSubscriptionPlans, apiGetMySubscription, apiSubscribe } from "@/lib/api";
import type { ApiSubscriptionPlan, ApiMySubscription } from "@/types";

export default function SubscriptionsPage() {
  const { user, refreshProfile } = useAuth();
  const { locale, currency, t } = useLocale();
  const [plans, setPlans] = useState<ApiSubscriptionPlan[]>([]);
  const [mySub, setMySub] = useState<ApiMySubscription | null>(null);
  const [subscribingId, setSubscribingId] = useState<number | null>(null);
  const [loaded, setLoaded] = useState(false);

  useEffect(() => {
    apiGetSubscriptionPlans()
      .then((res) => setPlans(res.plans))
      .catch(() => {})
      .finally(() => setLoaded(true));
  }, []);

  useEffect(() => {
    if (!user) return;
    apiGetMySubscription()
      .then(setMySub)
      .catch(() => {});
  }, [user]);

  async function handleSubscribe(planId: number) {
    if (!user) return;
    setSubscribingId(planId);
    try {
      await apiSubscribe(planId);
      const sub = await apiGetMySubscription();
      setMySub(sub);
      await refreshProfile();
    } catch {
      // subscription failed
    } finally {
      setSubscribingId(null);
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

      {/* Current subscription */}
      {user && mySub && mySub.isActive && (
        <div className="mb-6 rounded-xl border-2 border-blue-200 bg-blue-50 p-5">
          <p className="text-sm font-medium text-blue-700">{t("client.subscriptions.current_plan")}</p>
          <p className="text-lg font-bold text-gray-900">{mySub.planName}</p>
          {mySub.discount && <p className="text-sm text-gray-600">{mySub.discount}% {t("client.subscriptions.discount_off")}</p>}
          {mySub.endDate && <p className="text-xs text-gray-500">{t("client.subscriptions.active_until")} {formatDate(mySub.endDate, locale)}</p>}
        </div>
      )}

      {user && mySub && !mySub.isActive && (
        <div className="mb-6 rounded-xl border border-gray-200 bg-gray-50 p-5">
          <p className="text-sm text-gray-500">{t("client.subscriptions.no_plan")}</p>
        </div>
      )}

      {/* Plan cards */}
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
                    onClick={() => handleSubscribe(plan.id)}
                    disabled={subscribingId !== null}
                    className="mt-4 w-full rounded-lg bg-blue-600 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 transition-colors disabled:opacity-50"
                  >
                    {subscribingId === plan.id ? t("client.subscriptions.subscribing") : t("client.subscriptions.subscribe")}
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
    </div>
  );
}
