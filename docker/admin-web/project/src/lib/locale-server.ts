import { cookies } from "next/headers";
import { LOCALE_COOKIE, DEFAULT_LOCALE, SUPPORTED_LOCALES } from "./locale";
import type { Locale } from "./locale";

/** Read the user's locale preference from cookie. Falls back to "en". */
export async function getLocale(): Promise<Locale> {
  const cookieStore = await cookies();
  const value = cookieStore.get(LOCALE_COOKIE)?.value;
  if (value && SUPPORTED_LOCALES.includes(value as Locale)) {
    return value as Locale;
  }
  return DEFAULT_LOCALE;
}

/** Set the locale preference cookie. */
export async function setLocale(locale: Locale): Promise<void> {
  const cookieStore = await cookies();
  cookieStore.set(LOCALE_COOKIE, locale, {
    path: "/",
    sameSite: "lax",
    maxAge: 60 * 60 * 24 * 365,
  });
}
