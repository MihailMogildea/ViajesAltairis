import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { Breadcrumb } from "@/components/breadcrumb";
import { LanguagesTable } from "./languages-table";
import type { LanguageDto } from "@/types/system";

export default async function LanguagesPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let languages: LanguageDto[] = [];
  let error: string | null = null;

  try {
    languages = await apiFetch<LanguageDto[]>("/api/Languages", {
      cache: "no-store",
    });
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load languages";
  }

  return (
    <div>
      <Breadcrumb
        items={[
          { label: t["admin.section.system"] ?? "System Config", href: "/system" },
          { label: t["admin.system.languages"] ?? "Languages" },
        ]}
      />
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">
          {t["admin.system.languages"] ?? "Languages"}
        </h2>
        <p className="mt-1 text-sm text-gray-500">
          {t["admin.system.languages.desc"] ?? "Manage supported languages for the platform."}
        </p>
      </div>

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <LanguagesTable languages={languages} t={t} />
      )}
    </div>
  );
}
