"use client";

import { useState } from "react";
import { TranslationDto, WebTranslationDto, LanguageDto } from "@/types/system";
import { TabBar } from "@/components/tab-bar";
import { TranslationsTab } from "./translations-tab";
import { WebTranslationsTab } from "./web-translations-tab";

interface TranslationsTabsProps {
  translations: TranslationDto[];
  webTranslations: WebTranslationDto[];
  languages: LanguageDto[];
  t: Record<string, string>;
}

export function TranslationsTabs({
  translations,
  webTranslations,
  languages,
  t,
}: TranslationsTabsProps) {
  const [activeTab, setActiveTab] = useState("translations");

  const tabs = [
    { key: "translations", label: t["admin.translations.tab_translations"] ?? "Translations" },
    { key: "web-translations", label: t["admin.translations.tab_web"] ?? "Web Translations" },
  ];

  return (
    <>
      <TabBar tabs={tabs} active={activeTab} onChange={setActiveTab} />

      {activeTab === "translations" && (
        <TranslationsTab translations={translations} languages={languages} t={t} />
      )}

      {activeTab === "web-translations" && (
        <WebTranslationsTab webTranslations={webTranslations} languages={languages} t={t} />
      )}
    </>
  );
}
