export const SUPPORTED_LOCALES = ["en", "es"] as const;
export type Locale = (typeof SUPPORTED_LOCALES)[number];
export const DEFAULT_LOCALE: Locale = "en";
export const LOCALE_STORAGE_KEY = "va_locale";
export const CURRENCY_STORAGE_KEY = "va_currency";
