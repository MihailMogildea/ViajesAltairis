import type { Locale } from "./locale";

interface WebTranslationDto {
  id: number;
  translationKey: string;
  languageId: number;
  value: string;
}

const API_BASE_URL =
  process.env.NEXT_PUBLIC_ADMIN_API_URL ?? "http://admin-api:8080";

/** Language IDs matching DB seed: en=1, es=2 */
const LOCALE_TO_LANGUAGE_ID: Record<Locale, number> = {
  en: 1,
  es: 2,
};

/**
 * Fetch web translations from the public (no-auth) endpoint and return
 * a lookup map for the given locale.
 */
export async function loadTranslations(
  locale: Locale
): Promise<Record<string, string>> {
  const langId = LOCALE_TO_LANGUAGE_ID[locale];

  try {
    const res = await fetch(`${API_BASE_URL}/api/webtranslations/public`, {
      next: { revalidate: 60 },
    });
    if (!res.ok) return {};

    const all = (await res.json()) as WebTranslationDto[];
    const map: Record<string, string> = {};
    for (const item of all) {
      if (item.languageId === langId) {
        map[item.translationKey] = item.value;
      }
    }
    return map;
  } catch {
    // If API is down, return empty — UI will fall back to keys
    return {};
  }
}

/** Create a translation function from a loaded map. */
export function createT(
  translations: Record<string, string>
): (key: string, fallback?: string) => string {
  return (key: string, fallback?: string) =>
    translations[key] ?? fallback ?? key;
}
