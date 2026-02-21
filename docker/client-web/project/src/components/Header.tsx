"use client";

import { useState } from "react";
import Image from "next/image";
import Link from "next/link";
import { useAuth } from "@/context/AuthContext";
import { useBooking } from "@/context/BookingContext";
import { useLocale } from "@/context/LocaleContext";
import { SUPPORTED_LOCALES } from "@/lib/locale";

export default function Header() {
  const { user, logout } = useAuth();
  const { items } = useBooking();
  const { locale, setLocale, currency, setCurrency, currencies, t } = useLocale();
  const [menuOpen, setMenuOpen] = useState(false);

  return (
    <header className="sticky top-0 z-50 border-b border-gray-200 bg-white">
      <div className="mx-auto flex max-w-7xl items-center justify-between px-4 py-3 sm:px-6">
        <Link href="/" className="flex items-center gap-2 text-xl font-bold text-blue-600">
          <Image src="/logo.jpg" alt="ViajesAltairis" width={36} height={36} className="rounded" />
          ViajesAltairis
        </Link>

        {/* Desktop nav */}
        <nav className="hidden items-center gap-6 text-sm font-medium md:flex">
          <Link href="/hotels" className="text-gray-700 hover:text-blue-600 transition-colors">
            {t("client.nav.hotels")}
          </Link>
          {user && (
            <>
              <Link href="/reservations" className="text-gray-700 hover:text-blue-600 transition-colors">
                {t("client.nav.reservations")}
              </Link>
              <Link href="/profile" className="text-gray-700 hover:text-blue-600 transition-colors">
                {t("client.nav.profile")}
              </Link>
              <Link href="/invoices" className="text-gray-700 hover:text-blue-600 transition-colors">
                {t("client.nav.invoices")}
              </Link>
              <Link href="/subscriptions" className="text-gray-700 hover:text-blue-600 transition-colors">
                {t("client.nav.subscriptions")}
              </Link>
            </>
          )}
          <Link href="/booking" className="relative text-gray-700 hover:text-blue-600 transition-colors">
            {t("client.nav.basket")}
            {items.length > 0 && (
              <span className="absolute -right-4 -top-2 flex h-5 w-5 items-center justify-center rounded-full bg-blue-600 text-xs text-white">
                {items.length}
              </span>
            )}
          </Link>

          {/* Language toggle */}
          <div className="flex gap-1">
            {SUPPORTED_LOCALES.map((loc) => (
              <button
                key={loc}
                onClick={() => setLocale(loc)}
                className={`rounded px-2 py-0.5 text-xs font-medium ${
                  loc === locale
                    ? "bg-blue-100 text-blue-700"
                    : "text-gray-400 hover:text-gray-600"
                }`}
              >
                {loc.toUpperCase()}
              </button>
            ))}
          </div>

          {/* Currency selector */}
          <select
            value={currency.code}
            onChange={(e) => setCurrency(e.target.value)}
            className="rounded border border-gray-200 px-2 py-0.5 text-xs font-medium text-gray-600"
          >
            {currencies.map((c) => (
              <option key={c.code} value={c.code}>
                {c.symbol} {c.code}
              </option>
            ))}
          </select>

          {user ? (
            <div className="flex items-center gap-3">
              <span className="text-gray-500">{user.first_name}</span>
              <button onClick={logout} className="rounded-lg bg-gray-100 px-3 py-1.5 text-gray-700 hover:bg-gray-200 transition-colors">
                {t("client.nav.logout")}
              </button>
            </div>
          ) : (
            <div className="flex items-center gap-2">
              <Link href="/login" className="rounded-lg bg-blue-600 px-4 py-1.5 text-white hover:bg-blue-700 transition-colors">
                {t("client.nav.login")}
              </Link>
              <Link href="/register" className="rounded-lg border border-gray-300 px-4 py-1.5 text-gray-700 hover:bg-gray-50 transition-colors">
                {t("client.nav.register")}
              </Link>
            </div>
          )}
        </nav>

        {/* Mobile hamburger */}
        <button onClick={() => setMenuOpen(!menuOpen)} className="md:hidden p-2" aria-label="Toggle menu">
          <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
            {menuOpen ? (
              <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
            ) : (
              <path strokeLinecap="round" strokeLinejoin="round" d="M4 6h16M4 12h16M4 18h16" />
            )}
          </svg>
        </button>
      </div>

      {/* Mobile menu */}
      {menuOpen && (
        <nav className="border-t border-gray-100 bg-white px-4 py-4 md:hidden">
          <div className="flex flex-col gap-3 text-sm font-medium">
            <Link href="/hotels" className="py-2 text-gray-700">{t("client.nav.hotels")}</Link>
            {user && (
              <>
                <Link href="/reservations" className="py-2 text-gray-700">{t("client.nav.reservations")}</Link>
                <Link href="/profile" className="py-2 text-gray-700">{t("client.nav.profile")}</Link>
                <Link href="/invoices" className="py-2 text-gray-700">{t("client.nav.invoices")}</Link>
                <Link href="/subscriptions" className="py-2 text-gray-700">{t("client.nav.subscriptions")}</Link>
              </>
            )}
            <Link href="/booking" className="py-2 text-gray-700">
              {t("client.nav.basket")} {items.length > 0 && `(${items.length})`}
            </Link>

            {/* Language + currency (mobile) */}
            <div className="flex items-center gap-3 py-2">
              <div className="flex gap-1">
                {SUPPORTED_LOCALES.map((loc) => (
                  <button
                    key={loc}
                    onClick={() => setLocale(loc)}
                    className={`rounded px-2 py-0.5 text-xs font-medium ${
                      loc === locale ? "bg-blue-100 text-blue-700" : "text-gray-400 hover:text-gray-600"
                    }`}
                  >
                    {loc.toUpperCase()}
                  </button>
                ))}
              </div>
              <select
                value={currency.code}
                onChange={(e) => setCurrency(e.target.value)}
                className="rounded border border-gray-200 px-2 py-0.5 text-xs font-medium text-gray-600"
              >
                {currencies.map((c) => (
                  <option key={c.code} value={c.code}>{c.symbol} {c.code}</option>
                ))}
              </select>
            </div>

            {user ? (
              <>
                <span className="py-2 text-gray-500">{user.first_name} {user.last_name}</span>
                <button onClick={logout} className="py-2 text-left text-red-600">{t("client.nav.logout")}</button>
              </>
            ) : (
              <>
                <Link href="/login" className="py-2 text-blue-600">{t("client.nav.login")}</Link>
                <Link href="/register" className="py-2 text-blue-600">{t("client.nav.create_account")}</Link>
              </>
            )}
          </div>
        </nav>
      )}
    </header>
  );
}
