import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { Breadcrumb } from "@/components/breadcrumb";
import { EmailTemplateDto } from "@/types/system";
import { EmailTemplatesTable } from "./email-templates-table";

export default async function EmailTemplatesPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let emailTemplates: EmailTemplateDto[] = [];
  let error: string | null = null;

  try {
    emailTemplates = await apiFetch<EmailTemplateDto[]>("/api/EmailTemplates", {
      cache: "no-store",
    });
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
        <EmailTemplatesTable emailTemplates={emailTemplates} t={t} />
      )}
    </div>
  );
}
