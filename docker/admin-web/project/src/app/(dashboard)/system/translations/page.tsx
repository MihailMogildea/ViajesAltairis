import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { Breadcrumb } from "@/components/breadcrumb";
import { TranslationDto, WebTranslationDto, LanguageDto } from "@/types/system";
import { TranslationsTabs } from "./translations-tabs";

export default async function TranslationsPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let translations: TranslationDto[] = [];
  let webTranslations: WebTranslationDto[] = [];
  let languages: LanguageDto[] = [];
  let error: string | null = null;

  try {
    [translations, webTranslations, languages] = await Promise.all([
      apiFetch<TranslationDto[]>("/api/Translations", { cache: "no-store" }),
      apiFetch<WebTranslationDto[]>("/api/WebTranslations", { cache: "no-store" }),
      apiFetch<LanguageDto[]>("/api/Languages", { cache: "no-store" }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load translations";
  }

  return (
    <div>
      <Breadcrumb
        items={[
          { label: t["admin.section.system"] ?? "System Config", href: "/system" },
          { label: t["admin.system.translations"] ?? "Translations" },
        ]}
      />

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <TranslationsTabs
          translations={translations}
          webTranslations={webTranslations}
          languages={languages}
          t={t}
        />
      )}
    </div>
  );
}
