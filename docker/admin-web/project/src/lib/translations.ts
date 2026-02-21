import type { Locale } from "./locale";

const API_BASE_URL =
  process.env.NEXT_PUBLIC_ADMIN_API_URL ?? "http://admin-api:8080";

/**
 * Fetch web translations from the public (no-auth) endpoint and return
 * a lookup map for the given locale.
 */
export async function loadTranslations(
  locale: Locale
): Promise<Record<string, string>> {
  try {
    const res = await fetch(`${API_BASE_URL}/api/webtranslations/public`, {
      cache: "no-store",
      headers: { "Accept-Language": locale },
    });
    if (!res.ok) return {};

    return (await res.json()) as Record<string, string>;
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
