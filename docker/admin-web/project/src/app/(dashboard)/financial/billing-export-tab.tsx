"use client";

import { useState } from "react";

interface BillingExportTabProps {
  t: Record<string, string>;
}

const CHANNELS = [
  { key: "client", labelKey: "admin.financial.billing.channel_client", fallback: "Client Web" },
  { key: "external", labelKey: "admin.financial.billing.channel_external", fallback: "External Clients (B2B)" },
  { key: "staff", labelKey: "admin.financial.billing.channel_staff", fallback: "Staff Bookings" },
] as const;

function getDefaultFrom() {
  const now = new Date();
  return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, "0")}-01`;
}

function getDefaultTo() {
  const now = new Date();
  const last = new Date(now.getFullYear(), now.getMonth() + 1, 0);
  return `${last.getFullYear()}-${String(last.getMonth() + 1).padStart(2, "0")}-${String(last.getDate()).padStart(2, "0")}`;
}

export function BillingExportTab({ t }: BillingExportTabProps) {
  const [from, setFrom] = useState(getDefaultFrom);
  const [to, setTo] = useState(getDefaultTo);
  const [loading, setLoading] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function handleDownload(channel: string) {
    setLoading(channel);
    setError(null);
    try {
      const res = await fetch(
        `/api/billing-export?from=${from}&to=${to}&channel=${channel}`
      );
      if (!res.ok) {
        throw new Error(`${res.status} ${res.statusText}`);
      }
      const blob = await res.blob();
      const disposition = res.headers.get("Content-Disposition");
      const fileNameMatch = disposition?.match(/filename="?([^"]+)"?/);
      const fileName = fileNameMatch?.[1] ?? `billing-${channel}.zip`;

      const url = URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = fileName;
      document.body.appendChild(a);
      a.click();
      a.remove();
      URL.revokeObjectURL(url);
    } catch (e) {
      setError(e instanceof Error ? e.message : "Download failed");
    } finally {
      setLoading(null);
    }
  }

  return (
    <div className="space-y-6">
      {/* Date range */}
      <div className="flex items-end gap-4 rounded-lg border border-gray-200 bg-white p-4">
        <div>
          <label className="mb-1 block text-xs font-medium text-gray-500">
            {t["admin.financial.billing.from"] ?? "From"}
          </label>
          <input
            type="date"
            value={from}
            onChange={(e) => setFrom(e.target.value)}
            className="rounded-md border border-gray-300 px-3 py-1.5 text-sm shadow-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
          />
        </div>
        <div>
          <label className="mb-1 block text-xs font-medium text-gray-500">
            {t["admin.financial.billing.to"] ?? "To"}
          </label>
          <input
            type="date"
            value={to}
            onChange={(e) => setTo(e.target.value)}
            className="rounded-md border border-gray-300 px-3 py-1.5 text-sm shadow-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
          />
        </div>
      </div>

      {error && (
        <p className="text-sm text-red-600">{error}</p>
      )}

      {/* Channel cards */}
      <div className="grid gap-4 sm:grid-cols-3">
        {CHANNELS.map((ch) => (
          <div
            key={ch.key}
            className="flex flex-col items-center gap-3 rounded-lg border border-gray-200 bg-white p-6"
          >
            <h3 className="text-sm font-semibold text-gray-900">
              {t[ch.labelKey] ?? ch.fallback}
            </h3>
            <p className="text-xs text-gray-500 text-center">
              {t[`admin.financial.billing.${ch.key}_desc`] ??
                `Download billing invoices for ${ch.fallback.toLowerCase()}.`}
            </p>
            <button
              onClick={() => handleDownload(ch.key)}
              disabled={loading !== null}
              className="mt-auto rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-blue-700 disabled:opacity-50"
            >
              {loading === ch.key
                ? (t["admin.financial.billing.generating"] ?? "Generating...")
                : (t["admin.financial.billing.download"] ?? "Download ZIP")}
            </button>
          </div>
        ))}
      </div>
    </div>
  );
}
