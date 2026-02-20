"use client";

import { useRouter } from "next/navigation";
import { switchLocaleAction } from "@/lib/actions";
import type { Locale } from "@/lib/locale";
import { SUPPORTED_LOCALES } from "@/lib/locale";

const LOCALE_LABELS: Record<Locale, string> = {
  en: "EN",
  es: "ES",
};

export function LocaleSwitcher({ current }: { current: Locale }) {
  const router = useRouter();

  async function handleSwitch(locale: Locale) {
    await switchLocaleAction(locale);
    router.refresh();
  }

  return (
    <div className="flex gap-1">
      {SUPPORTED_LOCALES.map((locale) => (
        <button
          key={locale}
          onClick={() => handleSwitch(locale)}
          className={`rounded px-2 py-0.5 text-xs font-medium ${
            locale === current
              ? "bg-blue-100 text-blue-700"
              : "text-gray-400 hover:text-gray-600"
          }`}
        >
          {LOCALE_LABELS[locale]}
        </button>
      ))}
    </div>
  );
}
