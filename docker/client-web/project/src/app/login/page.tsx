"use client";

import { useState } from "react";
import { useSearchParams } from "next/navigation";
import { Suspense } from "react";
import Link from "next/link";
import { useAuth } from "@/context/AuthContext";
import { useLocale } from "@/context/LocaleContext";

function LoginContent() {
  const { user, login, logout } = useAuth();
  const { t } = useLocale();
  const searchParams = useSearchParams();
  const redirect = searchParams.get("redirect") || "/";
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  if (user) {
    return (
      <div className="mx-auto max-w-sm px-4 py-12">
        <div className="rounded-xl border border-gray-200 p-6 text-center">
          <div className="mx-auto mb-3 flex h-16 w-16 items-center justify-center rounded-full bg-blue-100 text-2xl">
            {user.first_name[0]}
          </div>
          <h2 className="text-xl font-semibold text-gray-900">{user.first_name} {user.last_name}</h2>
          <p className="text-sm text-gray-500">{user.email}</p>
          {user.subscription_type && (
            <span className="mt-2 inline-block rounded-full bg-purple-100 px-3 py-0.5 text-xs font-medium text-purple-700">
              {user.subscription_type} subscriber ({user.subscription_discount}% off)
            </span>
          )}
          {user.business_partner && (
            <span className="mt-2 inline-block rounded-full bg-green-100 px-3 py-0.5 text-xs font-medium text-green-700">
              {user.business_partner} ({user.business_partner_discount}% partner discount)
            </span>
          )}
          <button
            onClick={logout}
            className="mt-4 w-full rounded-lg bg-gray-100 py-2 text-sm font-medium text-gray-700 hover:bg-gray-200 transition-colors"
          >
            {t("client.nav.logout")}
          </button>
        </div>
      </div>
    );
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      const success = await login(email, password);
      if (success) {
        window.location.href = redirect;
      } else {
        setError(t("client.login.error_invalid"));
      }
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="mx-auto max-w-sm px-4 py-12">
      <h1 className="mb-6 text-2xl font-bold text-gray-900">{t("client.login.title")}</h1>
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
        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.login.password")}</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            placeholder="password123"
          />
          <div className="mt-1 text-right">
            <Link href="/forgot-password" className="text-xs text-blue-600 hover:underline">{t("client.login.forgot_password")}</Link>
          </div>
        </div>
        {error && <p className="text-sm text-red-600">{error}</p>}
        <button type="submit" disabled={loading} className="w-full rounded-lg bg-blue-600 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 transition-colors disabled:opacity-50">
          {loading ? t("client.login.logging_in") : t("client.login.title")}
        </button>
      </form>

      <p className="mt-6 text-center text-sm text-gray-500">
        {t("client.login.no_account")}{" "}
        <Link href="/register" className="font-medium text-blue-600 hover:underline">{t("client.login.create_one")}</Link>
      </p>

      <div className="mt-6 rounded-xl bg-gray-50 p-4">
        <h3 className="text-sm font-semibold text-gray-700">{t("client.login.demo_accounts")}</h3>
        <p className="mt-1 text-xs text-gray-400">{t("client.login.demo_password")}</p>
        <ul className="mt-3 space-y-2 text-sm">
          <li>
            <button onClick={() => { setEmail("client1@example.com"); setPassword("password123"); }} className="text-blue-600 hover:underline">
              client1@example.com
            </button>
            <span className="text-gray-400"> — {t("client.login.demo_juan")}</span>
          </li>
          <li>
            <button onClick={() => { setEmail("client2@example.com"); setPassword("password123"); }} className="text-blue-600 hover:underline">
              client2@example.com
            </button>
            <span className="text-gray-400"> — {t("client.login.demo_emma")}</span>
          </li>
          <li>
            <button onClick={() => { setEmail("ana@viajessol.com"); setPassword("password123"); }} className="text-blue-600 hover:underline">
              ana@viajessol.com
            </button>
            <span className="text-gray-400"> — {t("client.login.demo_ana")}</span>
          </li>
        </ul>
      </div>
    </div>
  );
}

export default function LoginPage() {
  return (
    <Suspense fallback={<div className="mx-auto max-w-sm px-4 py-12 text-center text-gray-400">Loading...</div>}>
      <LoginContent />
    </Suspense>
  );
}
