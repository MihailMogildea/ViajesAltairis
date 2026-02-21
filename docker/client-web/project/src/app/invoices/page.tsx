"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useAuth } from "@/context/AuthContext";
import { useLocale } from "@/context/LocaleContext";
import { formatPrice, formatDate } from "@/lib/utils";
import { apiGetInvoices, apiGetInvoiceDetail, apiDownloadInvoicePdf } from "@/lib/api";
import type { ApiInvoiceSummary, ApiInvoiceDetail } from "@/types";

const statusColors: Record<number, string> = {
  1: "bg-yellow-100 text-yellow-700",  // created
  2: "bg-green-100 text-green-700",    // paid
  3: "bg-blue-100 text-blue-700",      // refunded
};

export default function InvoicesPage() {
  const { user } = useAuth();
  const { locale, currency, t } = useLocale();
  const [invoices, setInvoices] = useState<ApiInvoiceSummary[]>([]);
  const [loaded, setLoaded] = useState(false);
  const [expandedId, setExpandedId] = useState<number | null>(null);
  const [details, setDetails] = useState<Record<number, ApiInvoiceDetail>>({});
  const [downloading, setDownloading] = useState<number | null>(null);

  const fp = (amount: number, cur?: string) => formatPrice(amount, cur || currency.code, locale);

  useEffect(() => {
    if (!user) return;
    setDetails({});
    apiGetInvoices()
      .then((res) => { setInvoices(res.invoices); setLoaded(true); })
      .catch(() => setLoaded(true));
  }, [user, locale]);

  useEffect(() => {
    if (expandedId === null || details[expandedId]) return;
    apiGetInvoiceDetail(expandedId)
      .then((d) => setDetails((prev) => ({ ...prev, [expandedId]: d })))
      .catch(() => {});
  }, [expandedId, details, locale]);

  if (!user) {
    return (
      <div className="mx-auto max-w-lg px-4 py-16 text-center">
        <h1 className="text-2xl font-bold text-gray-900">{t("client.invoices.title")}</h1>
        <p className="mt-2 text-gray-500">{t("client.invoices.login_message")}</p>
        <Link href="/login" className="mt-4 inline-block rounded-lg bg-blue-600 px-6 py-2.5 text-sm font-semibold text-white hover:bg-blue-700">
          {t("client.common.login")}
        </Link>
      </div>
    );
  }

  if (!loaded) {
    return (
      <div className="mx-auto max-w-3xl px-4 py-8 sm:px-6">
        <h1 className="mb-6 text-2xl font-bold text-gray-900">{t("client.invoices.title")}</h1>
        <p className="text-gray-400">{t("client.invoices.loading")}</p>
      </div>
    );
  }

  if (invoices.length === 0) {
    return (
      <div className="mx-auto max-w-lg px-4 py-16 text-center">
        <h1 className="text-2xl font-bold text-gray-900">{t("client.invoices.title")}</h1>
        <p className="mt-2 text-gray-500">{t("client.invoices.empty")}</p>
        <Link href="/hotels" className="mt-4 inline-block rounded-lg bg-blue-600 px-6 py-2.5 text-sm font-semibold text-white hover:bg-blue-700">
          {t("client.invoices.browse")}
        </Link>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-3xl px-4 py-8 sm:px-6">
      <h1 className="mb-6 text-2xl font-bold text-gray-900">{t("client.invoices.title")}</h1>
      <div className="space-y-4">
        {invoices.map((inv) => (
          <div key={inv.id} className="rounded-xl border border-gray-200 overflow-hidden">
            <button
              onClick={() => setExpandedId(expandedId === inv.id ? null : inv.id)}
              className="flex w-full items-center justify-between p-5 text-left hover:bg-gray-50 transition-colors"
            >
              <div className="flex-1">
                <div className="flex flex-wrap items-center gap-2">
                  <span className="font-mono text-sm font-semibold text-gray-900">{inv.invoiceNumber}</span>
                  <span className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${statusColors[inv.statusId] || "bg-gray-100 text-gray-600"}`}>
                    {inv.status}
                  </span>
                </div>
                <p className="text-xs text-gray-400">{t("client.invoices.issued")} {formatDate(inv.issuedAt, locale)}</p>
              </div>
              <div className="ml-4 text-right">
                <p className="text-lg font-bold text-gray-900">{fp(inv.totalAmount, inv.currency)}</p>
                <svg className={`ml-auto h-5 w-5 text-gray-400 transition-transform ${expandedId === inv.id ? "rotate-180" : ""}`} fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                  <path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7" />
                </svg>
              </div>
            </button>

            {expandedId === inv.id && (
              <div className="border-t border-gray-100 bg-gray-50 p-5">
                {details[inv.id] ? (
                  <div className="text-sm">
                    <div className="space-y-1">
                      <div className="flex justify-between text-gray-500">
                        <span>{t("client.invoices.subtotal")}</span>
                        <span>{fp(details[inv.id].subTotal, details[inv.id].currency)}</span>
                      </div>
                      <div className="flex justify-between text-gray-500">
                        <span>{t("client.invoices.tax")}</span>
                        <span>{fp(details[inv.id].taxAmount, details[inv.id].currency)}</span>
                      </div>
                      <div className="flex justify-between font-bold text-gray-900">
                        <span>{t("client.invoices.total")}</span>
                        <span>{fp(details[inv.id].totalAmount, details[inv.id].currency)}</span>
                      </div>
                    </div>
                    <div className="mt-3 border-t border-gray-200 pt-3 text-gray-500">
                      <p>
                        {t("client.invoices.paid_at")}:{" "}
                        {details[inv.id].paidAt ? formatDate(details[inv.id].paidAt!, locale) : t("client.invoices.not_paid")}
                      </p>
                    </div>
                    <div className="mt-3 flex items-center gap-4">
                      <button
                        onClick={async () => {
                          setDownloading(inv.id);
                          try { await apiDownloadInvoicePdf(inv.id); } catch { }
                          setDownloading(null);
                        }}
                        disabled={downloading === inv.id}
                        className="inline-flex items-center gap-1.5 text-sm font-medium text-blue-600 hover:underline disabled:opacity-50"
                      >
                        <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                          <path strokeLinecap="round" strokeLinejoin="round" d="M12 10v6m0 0l-3-3m3 3l3-3M3 17v3a2 2 0 002 2h14a2 2 0 002-2v-3" />
                        </svg>
                        {downloading === inv.id ? t("client.invoices.downloading") : t("client.invoices.download_pdf")}
                      </button>
                      <Link
                        href="/reservations"
                        className="text-sm font-medium text-blue-600 hover:underline"
                      >
                        {t("client.invoices.view_reservation")}
                      </Link>
                    </div>
                  </div>
                ) : (
                  <p className="text-sm text-gray-400">{t("client.invoices.loading")}</p>
                )}
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}
