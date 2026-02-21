"use client";

import { Suspense } from "react";
import Link from "next/link";
import { useSearchParams } from "next/navigation";
import { formatPrice } from "@/lib/utils";
import { useLocale } from "@/context/LocaleContext";

function ConfirmationContent() {
  const params = useSearchParams();
  const { locale, currency, t } = useLocale();
  const code = params.get("code") || "VA-UNKNOWN";
  const total = Number(params.get("total")) || 0;
  const itemCount = Number(params.get("items")) || 0;
  const status = params.get("status") || "Confirmed";
  const isPending = status === "Pending";

  return (
    <div className="mx-auto max-w-lg px-4 py-16 text-center">
      <div className={`mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full text-3xl ${
        isPending ? "bg-amber-100" : "bg-green-100"
      }`}>
        {isPending ? "\u23F3" : "\u2713"}
      </div>
      <h1 className="text-2xl font-bold text-gray-900">
        {isPending
          ? (t("client.confirmation.pending_title") || "Reservation Pending")
          : t("client.confirmation.title")}
      </h1>
      <p className="mt-2 text-gray-500">
        {isPending
          ? (t("client.confirmation.pending_message") || "Your reservation will be confirmed once we receive the bank transfer.")
          : t("client.confirmation.message")}
      </p>

      <div className={`mt-6 rounded-xl border p-6 ${
        isPending ? "border-amber-200 bg-amber-50" : "border-gray-200"
      }`}>
        <p className="text-sm text-gray-400">{t("client.confirmation.code")}</p>
        <p className="mt-1 text-2xl font-bold tracking-wider text-blue-600">{code}</p>
        <div className="mt-4 flex justify-center gap-6 text-sm text-gray-600">
          <div>
            <p className="text-gray-400">{t("client.confirmation.items")}</p>
            <p className="font-medium">{itemCount}</p>
          </div>
          <div>
            <p className="text-gray-400">{t("client.confirmation.total")}</p>
            <p className="font-medium">{formatPrice(total, currency.code, locale, currency.exchangeRateToEur)}</p>
          </div>
        </div>
      </div>

      {isPending && (
        <div className="mt-6 rounded-xl border border-amber-200 bg-amber-50 p-5 text-left">
          <p className="mb-3 text-sm font-semibold text-amber-800">
            {t("client.confirmation.bank_transfer_title") || "Bank Transfer Details"}
          </p>
          <div className="space-y-2 text-sm text-gray-700">
            <div className="flex justify-between">
              <span className="text-gray-500">{t("client.booking.beneficiary") || "Beneficiary"}</span>
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
              <span className="text-gray-500">{t("client.confirmation.reference") || "Reference"}</span>
              <span className="font-mono font-medium">{code}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-gray-500">{t("client.booking.amount") || "Amount"}</span>
              <span className="font-bold text-gray-900">{formatPrice(total, currency.code, locale, currency.exchangeRateToEur)}</span>
            </div>
          </div>
        </div>
      )}

      <p className="mt-6 text-sm text-gray-400">{t("client.confirmation.email_sent")}</p>

      <div className="mt-6 flex flex-col gap-3 sm:flex-row sm:justify-center">
        <Link href="/reservations" className="rounded-lg bg-blue-600 px-6 py-2.5 text-sm font-semibold text-white hover:bg-blue-700">
          {t("client.confirmation.view_reservations")}
        </Link>
        <Link href="/hotels" className="rounded-lg border border-gray-300 px-6 py-2.5 text-sm font-medium text-gray-700 hover:bg-gray-50">
          {t("client.confirmation.continue")}
        </Link>
      </div>
    </div>
  );
}

export default function ConfirmationPage() {
  return (
    <Suspense fallback={<div className="py-16 text-center text-gray-400">Loading...</div>}>
      <ConfirmationContent />
    </Suspense>
  );
}
