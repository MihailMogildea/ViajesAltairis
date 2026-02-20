"use client";

import { useState } from "react";
import { useSearchParams } from "next/navigation";
import { Suspense } from "react";
import Link from "next/link";
import { useLocale } from "@/context/LocaleContext";
import { apiResetPassword } from "@/lib/api";

function ResetPasswordContent() {
  const { t } = useLocale();
  const searchParams = useSearchParams();
  const token = searchParams.get("token");

  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);
  const [loading, setLoading] = useState(false);

  if (!token) {
    return (
      <div className="mx-auto max-w-sm px-4 py-12 text-center">
        <h1 className="text-2xl font-bold text-gray-900">{t("client.reset_password.invalid_link")}</h1>
        <Link href="/forgot-password" className="mt-4 inline-block text-sm font-medium text-blue-600 hover:underline">
          {t("client.reset_password.request_new")}
        </Link>
      </div>
    );
  }

  if (success) {
    return (
      <div className="mx-auto max-w-sm px-4 py-12 text-center">
        <h1 className="text-2xl font-bold text-gray-900">{t("client.reset_password.success_title")}</h1>
        <p className="mt-3 text-gray-500">{t("client.reset_password.success_message")}</p>
        <Link href="/login" className="mt-6 inline-block rounded-lg bg-blue-600 px-6 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 transition-colors">
          {t("client.nav.login")}
        </Link>
      </div>
    );
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");
    if (newPassword.length < 8) {
      setError(t("client.profile.pw_length"));
      return;
    }
    if (newPassword !== confirmPassword) {
      setError(t("client.profile.pw_mismatch"));
      return;
    }
    setLoading(true);
    try {
      await apiResetPassword(token!, newPassword);
      setSuccess(true);
    } catch {
      setError(t("client.reset_password.error"));
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="mx-auto max-w-sm px-4 py-12">
      <h1 className="mb-6 text-2xl font-bold text-gray-900">{t("client.reset_password.title")}</h1>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.profile.new_password")}</label>
          <input
            type="password"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            required
            minLength={8}
            className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            placeholder={t("client.register.min_chars")}
          />
        </div>
        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.profile.confirm_password")}</label>
          <input
            type="password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            required
            minLength={8}
            className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
          />
        </div>
        {error && <p className="text-sm text-red-600">{error}</p>}
        <button type="submit" disabled={loading} className="w-full rounded-lg bg-blue-600 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 transition-colors disabled:opacity-50">
          {loading ? "..." : t("client.reset_password.title")}
        </button>
      </form>
    </div>
  );
}

export default function ResetPasswordPage() {
  return (
    <Suspense fallback={<div className="mx-auto max-w-sm px-4 py-12 text-center text-gray-400">Loading...</div>}>
      <ResetPasswordContent />
    </Suspense>
  );
}
