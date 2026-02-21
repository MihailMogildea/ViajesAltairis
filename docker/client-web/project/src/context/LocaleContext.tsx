"use client";

import { createContext, useContext, useState, useEffect, useCallback, type ReactNode } from "react";
import {
  SUPPORTED_LOCALES,
  DEFAULT_LOCALE,
  LOCALE_STORAGE_KEY,
  CURRENCY_STORAGE_KEY,
  type Locale,
} from "@/lib/locale";
import { fallbackTranslations } from "@/lib/translations-fallback";
import {
  apiGetWebTranslations,
  apiGetLanguages,
  apiGetCurrencies,
  type ApiLanguage,
  type ApiCurrency,
} from "@/lib/api";

export interface CurrencyInfo {
  code: string;
  symbol: string;
  exchangeRateToEur: number;
}

interface LocaleState {
  locale: Locale;
  currency: CurrencyInfo;
  t: (key: string, fallback?: string) => string;
  setLocale: (locale: Locale) => void;
  setCurrency: (code: string) => void;
  languages: ApiLanguage[];
  currencies: ApiCurrency[];
}

const DEFAULT_CURRENCY: CurrencyInfo = { code: "EUR", symbol: "€", exchangeRateToEur: 1 };

const FALLBACK_LANGUAGES: ApiLanguage[] = [
  { id: 1, code: "en", name: "English" },
  { id: 2, code: "es", name: "Español" },
];

const FALLBACK_CURRENCIES: ApiCurrency[] = [
  { id: 1, code: "EUR", name: "Euro", symbol: "€", exchangeRateToEur: 1 },
  { id: 2, code: "GBP", name: "British Pound", symbol: "£", exchangeRateToEur: 1.163 },
  { id: 3, code: "USD", name: "US Dollar", symbol: "$", exchangeRateToEur: 0.926 },
];

const LocaleContext = createContext<LocaleState>({
  locale: DEFAULT_LOCALE,
  currency: DEFAULT_CURRENCY,
  t: (key, fallback) => fallback ?? key,
  setLocale: () => {},
  setCurrency: () => {},
  languages: FALLBACK_LANGUAGES,
  currencies: FALLBACK_CURRENCIES,
});

function readStoredLocale(): Locale {
  if (typeof window === "undefined") return DEFAULT_LOCALE;
  const stored = localStorage.getItem(LOCALE_STORAGE_KEY);
  if (stored && SUPPORTED_LOCALES.includes(stored as Locale)) return stored as Locale;
  return DEFAULT_LOCALE;
}

function readStoredCurrency(currencies: ApiCurrency[]): CurrencyInfo {
  if (typeof window === "undefined") return DEFAULT_CURRENCY;
  const stored = localStorage.getItem(CURRENCY_STORAGE_KEY);
  if (stored) {
    const match = currencies.find((c) => c.code === stored);
    if (match) return { code: match.code, symbol: match.symbol, exchangeRateToEur: match.exchangeRateToEur };
  }
  return DEFAULT_CURRENCY;
}

export function LocaleProvider({ children }: { children: ReactNode }) {
  const [locale, setLocaleState] = useState<Locale>(DEFAULT_LOCALE);
  const [currency, setCurrencyState] = useState<CurrencyInfo>(DEFAULT_CURRENCY);
  const [translations, setTranslations] = useState<Record<string, string>>({});
  const [languages, setLanguages] = useState<ApiLanguage[]>(FALLBACK_LANGUAGES);
  const [currencies, setCurrencies] = useState<ApiCurrency[]>(FALLBACK_CURRENCIES);

  // Load translations for a given locale
  const loadTranslations = useCallback(async (loc: Locale) => {
    try {
      const map = await apiGetWebTranslations();
      if (Object.keys(map).length > 0) {
        setTranslations(map);
        return;
      }
    } catch { /* fall through */ }
    setTranslations(fallbackTranslations[loc] || fallbackTranslations.en);
  }, []);

  // Initialize on mount
  useEffect(() => {
    const storedLocale = readStoredLocale();
    setLocaleState(storedLocale);

    // Fetch languages and currencies
    apiGetLanguages()
      .then((langs) => { if (langs.length > 0) setLanguages(langs); })
      .catch(() => { /* keep fallback */ });

    apiGetCurrencies()
      .then((currs) => {
        if (currs.length > 0) {
          setCurrencies(currs);
          setCurrencyState(readStoredCurrency(currs));
        }
      })
      .catch(() => { /* keep fallback */ });

    loadTranslations(storedLocale);
  }, [loadTranslations]);

  function setLocale(newLocale: Locale) {
    setLocaleState(newLocale);
    localStorage.setItem(LOCALE_STORAGE_KEY, newLocale);
    loadTranslations(newLocale);
  }

  function setCurrency(code: string) {
    const match = currencies.find((c) => c.code === code);
    if (match) {
      const info: CurrencyInfo = { code: match.code, symbol: match.symbol, exchangeRateToEur: match.exchangeRateToEur };
      setCurrencyState(info);
      localStorage.setItem(CURRENCY_STORAGE_KEY, code);
    }
  }

  function t(key: string, fallback?: string): string {
    return translations[key] ?? fallbackTranslations[locale]?.[key] ?? fallback ?? key;
  }

  return (
    <LocaleContext.Provider value={{ locale, currency, t, setLocale, setCurrency, languages, currencies }}>
      {children}
    </LocaleContext.Provider>
  );
}

export function useLocale() {
  return useContext(LocaleContext);
}
