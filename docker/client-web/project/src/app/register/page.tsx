"use client";

import { useState } from "react";
import { useSearchParams } from "next/navigation";
import { Suspense } from "react";
import Link from "next/link";
import { useAuth } from "@/context/AuthContext";
import { useLocale } from "@/context/LocaleContext";

function RegisterContent() {
  const { user, register } = useAuth();
  const { t } = useLocale();
  const searchParams = useSearchParams();
  const redirect = searchParams.get("redirect") || "/";
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [phone, setPhone] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  if (user) {
    return (
      <div className="mx-auto max-w-sm px-4 py-12 text-center">
        <h1 className="text-2xl font-bold text-gray-900">{t("client.register.already_logged")}</h1>
        <p className="mt-2 text-gray-500">{t("client.register.logged_as")} {user.first_name} {user.last_name}.</p>
        <Link href="/" className="mt-4 inline-block rounded-lg bg-blue-600 px-6 py-2.5 text-sm font-semibold text-white hover:bg-blue-700">
          {t("client.register.go_home")}
        </Link>
      </div>
    );
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");

    if (password !== confirmPassword) {
      setError(t("client.register.error_mismatch"));
      return;
    }
    if (password.length < 8) {
      setError(t("client.register.error_length"));
      return;
    }

    setLoading(true);
    try {
      const err = await register({
        email,
        password,
        firstName,
        lastName,
        phone: phone || undefined,
      });
      if (err) {
        setError(err);
      } else {
        window.location.href = redirect;
      }
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="mx-auto max-w-sm px-4 py-12">
      <h1 className="mb-6 text-2xl font-bold text-gray-900">{t("client.register.title")}</h1>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div className="grid gap-4 sm:grid-cols-2">
          <div>
            <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.register.first_name")}</label>
            <input
              type="text"
              value={firstName}
              onChange={(e) => setFirstName(e.target.value)}
              required
              className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            />
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.register.last_name")}</label>
            <input
              type="text"
              value={lastName}
              onChange={(e) => setLastName(e.target.value)}
              required
              className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            />
          </div>
        </div>
        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.register.email")}</label>
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
          <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.register.phone")} <span className="text-gray-400">{t("client.register.phone_optional")}</span></label>
          <input
            type="tel"
            value={phone}
            onChange={(e) => setPhone(e.target.value)}
            className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            placeholder="+34 600 000 000"
          />
        </div>
        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.register.password")}</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            minLength={8}
            className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            placeholder={t("client.register.min_chars")}
          />
        </div>
        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.register.confirm_password")}</label>
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
        <button
          type="submit"
          disabled={loading}
          className="w-full rounded-lg bg-blue-600 py-2.5 text-sm font-semibold text-white hover:bg-blue-700 transition-colors disabled:opacity-50"
        >
          {loading ? t("client.register.creating") : t("client.register.title")}
        </button>
      </form>

      <p className="mt-6 text-center text-sm text-gray-500">
        {t("client.register.already_have")}{" "}
        <Link href={redirect !== "/" ? `/login?redirect=${encodeURIComponent(redirect)}` : "/login"} className="font-medium text-blue-600 hover:underline">{t("client.nav.login")}</Link>
      </p>
    </div>
  );
}

export default function RegisterPage() {
  return (
    <Suspense fallback={<div className="mx-auto max-w-sm px-4 py-12 text-center text-gray-400">Loading...</div>}>
      <RegisterContent />
    </Suspense>
  );
}
