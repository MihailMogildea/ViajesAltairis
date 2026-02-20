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

  return (
    <div className="mx-auto max-w-lg px-4 py-16 text-center">
      <div className="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-green-100 text-3xl">
        &#10003;
      </div>
      <h1 className="text-2xl font-bold text-gray-900">{t("client.confirmation.title")}</h1>
      <p className="mt-2 text-gray-500">{t("client.confirmation.message")}</p>

      <div className="mt-6 rounded-xl border border-gray-200 p-6">
        <p className="text-sm text-gray-400">{t("client.confirmation.code")}</p>
        <p className="mt-1 text-2xl font-bold tracking-wider text-blue-600">{code}</p>
        <div className="mt-4 flex justify-center gap-6 text-sm text-gray-600">
          <div>
            <p className="text-gray-400">{t("client.confirmation.items")}</p>
            <p className="font-medium">{itemCount}</p>
          </div>
          <div>
            <p className="text-gray-400">{t("client.confirmation.total")}</p>
            <p className="font-medium">{formatPrice(total, currency.code, locale)}</p>
          </div>
        </div>
      </div>

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
