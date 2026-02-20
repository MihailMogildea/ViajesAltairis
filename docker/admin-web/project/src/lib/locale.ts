/**
 * Locale constants — safe to import from both server and client components.
 */

const SUPPORTED_LOCALES = ["en", "es"] as const;

type Locale = (typeof SUPPORTED_LOCALES)[number];

const DEFAULT_LOCALE: Locale = "en";
const LOCALE_COOKIE = "altairis_locale";

export { SUPPORTED_LOCALES, DEFAULT_LOCALE, LOCALE_COOKIE };
export type { Locale };
