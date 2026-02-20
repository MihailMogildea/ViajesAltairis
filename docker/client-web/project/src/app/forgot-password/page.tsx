"use client";

import { useState } from "react";
import Link from "next/link";
import { useLocale } from "@/context/LocaleContext";
import { apiForgotPassword } from "@/lib/api";

export default function ForgotPasswordPage() {
  const { t } = useLocale();
  const [email, setEmail] = useState("");
  const [sent, setSent] = useState(false);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setLoading(true);
    try {
      await apiForgotPassword(email);
    } catch {
      // API always returns 200, but handle network errors gracefully
    } finally {
      setLoading(false);
      setSent(true);
    }
  }

  if (sent) {
    return (
      <div className="mx-auto max-w-sm px-4 py-12 text-center">
        <h1 className="text-2xl font-bold text-gray-900">{t("client.forgot_password.sent_title")}</h1>
        <p className="mt-3 text-gray-500">{t("client.forgot_password.sent_message")}</p>
        <Link href="/login" className="mt-6 inline-block rounded-lg bg-blue-600 px-6 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 transition-colors">
          {t("client.forgot_password.back_to_login")}
        </Link>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-sm px-4 py-12">
      <h1 className="mb-2 text-2xl font-bold text-gray-900">{t("client.forgot_password.title")}</h1>
      <p className="mb-6 text-sm text-gray-500">{t("client.forgot_password.description")}</p>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.login.email")}</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            placeholder="you@example.com"
          />
        </div>
        <button type="submit" disabled={loading} className="w-full rounded-lg bg-blue-600 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 transition-colors disabled:opacity-50">
          {loading ? "..." : t("client.forgot_password.send")}
        </button>
      </form>
      <p className="mt-6 text-center text-sm text-gray-500">
        <Link href="/login" className="font-medium text-blue-600 hover:underline">{t("client.forgot_password.back_to_login")}</Link>
      </p>
    </div>
  );
}
