import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { Breadcrumb } from "@/components/breadcrumb";
import { EmailTemplateDto, TranslationDto, LanguageDto } from "@/types/system";
import { EmailTemplatesTable } from "./email-templates-table";

export default async function EmailTemplatesPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let emailTemplates: EmailTemplateDto[] = [];
  let translations: TranslationDto[] = [];
  let languages: LanguageDto[] = [];
  let error: string | null = null;

  try {
    [emailTemplates, translations, languages] = await Promise.all([
      apiFetch<EmailTemplateDto[]>("/api/EmailTemplates", { cache: "no-store" }),
      apiFetch<TranslationDto[]>("/api/Translations", { cache: "no-store" }),
      apiFetch<LanguageDto[]>("/api/Languages", { cache: "no-store" }),
    ]);
    // Only keep email_template translations
    translations = translations.filter((tr) => tr.entityType === "email_template");
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load email templates";
  }

  return (
    <div>
      <Breadcrumb
        items={[
          { label: t["admin.section.system"] ?? "System Config", href: "/system" },
          { label: t["admin.system.email_templates"] ?? "Email Templates" },
        ]}
      />

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <EmailTemplatesTable
          emailTemplates={emailTemplates}
          translations={translations}
          languages={languages}
          t={t}
        />
      )}
    </div>
  );
}
