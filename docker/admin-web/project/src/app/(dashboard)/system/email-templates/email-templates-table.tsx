"use client";

import { useState, useCallback } from "react";
import { EmailTemplateDto, TranslationDto, LanguageDto } from "@/types/system";
import { DataTable, Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import { createTranslation, updateTranslation } from "./actions";

interface LanguageFields {
  subjectTranslationId: number | null;
  bodyTranslationId: number | null;
  subject: string;
  body: string;
}

export function EmailTemplatesTable({
  emailTemplates: initial,
  translations: initialTranslations,
  languages,
  t,
}: {
  emailTemplates: EmailTemplateDto[];
  translations: TranslationDto[];
  languages: LanguageDto[];
  t: Record<string, string>;
}) {
  const [items] = useState(initial);
  const [translations, setTranslations] = useState(initialTranslations);
  const [editing, setEditing] = useState<EmailTemplateDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  // Per-language form state: { [languageId]: { subject, body } }
  const [langFields, setLangFields] = useState<Record<number, LanguageFields>>({});

  const dismissMessage = useCallback(() => setMessage(null), []);

  function findTranslation(entityId: number, field: string, languageId: number) {
    return translations.find(
      (tr) =>
        tr.entityType === "email_template" &&
        tr.entityId === entityId &&
        tr.field === field &&
        tr.languageId === languageId
    );
  }

  function openEdit(item: EmailTemplateDto) {
    const fields: Record<number, LanguageFields> = {};
    for (const lang of languages) {
      const subjectTr = findTranslation(item.id, "subject", lang.id);
      const bodyTr = findTranslation(item.id, "body", lang.id);
      fields[lang.id] = {
        subjectTranslationId: subjectTr?.id ?? null,
        bodyTranslationId: bodyTr?.id ?? null,
        subject: subjectTr?.value ?? "",
        body: bodyTr?.value ?? "",
      };
    }
    setLangFields(fields);
    setEditing(item);
    setMessage(null);
  }

  function updateField(languageId: number, field: "subject" | "body", value: string) {
    setLangFields((prev) => ({
      ...prev,
      [languageId]: { ...prev[languageId], [field]: value },
    }));
  }

  async function handleSave() {
    if (!editing) return;
    setPending(true);
    setMessage(null);
    try {
      const updatedTranslations = [...translations];

      for (const lang of languages) {
        const lf = langFields[lang.id];
        if (!lf) continue;

        // Save subject
        const subjectPayload = {
          entityType: "email_template",
          entityId: editing.id,
          field: "subject",
          languageId: lang.id,
          value: lf.subject,
        };
        if (lf.subjectTranslationId) {
          const updated = await updateTranslation(lf.subjectTranslationId, subjectPayload);
          const idx = updatedTranslations.findIndex((tr) => tr.id === lf.subjectTranslationId);
          if (idx >= 0) updatedTranslations[idx] = updated;
        } else if (lf.subject) {
          const created = await createTranslation(subjectPayload);
          updatedTranslations.push(created);
        }

        // Save body
        const bodyPayload = {
          entityType: "email_template",
          entityId: editing.id,
          field: "body",
          languageId: lang.id,
          value: lf.body,
        };
        if (lf.bodyTranslationId) {
          const updated = await updateTranslation(lf.bodyTranslationId, bodyPayload);
          const idx = updatedTranslations.findIndex((tr) => tr.id === lf.bodyTranslationId);
          if (idx >= 0) updatedTranslations[idx] = updated;
        } else if (lf.body) {
          const created = await createTranslation(bodyPayload);
          updatedTranslations.push(created);
        }
      }

      setTranslations(updatedTranslations);
      setEditing(null);
      setMessage(t["admin.toast.updated"] ?? "Updated successfully.");
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Save failed");
    } finally {
      setPending(false);
    }
  }

  // Helper: get a preview of the subject for a given template (first language)
  function getSubjectPreview(templateId: number): string {
    const tr = translations.find(
      (tr) =>
        tr.entityType === "email_template" &&
        tr.entityId === templateId &&
        tr.field === "subject" &&
        tr.languageId === languages[0]?.id
    );
    return tr?.value ?? "";
  }

  const columns: Column<EmailTemplateDto>[] = [
    { key: "id", header: "ID", className: "w-20" },
    { key: "name", header: t["admin.label.key"] ?? "Key" },
    {
      key: "subject",
      header: t["admin.label.subject"] ?? "Subject",
      render: (item: EmailTemplateDto) => (
        <span className="text-gray-500">{getSubjectPreview(item.id)}</span>
      ),
    },
  ];

  return (
    <>
      <ToastMessage message={message} onDismiss={dismissMessage} />

      <SectionHeader
        title={t["admin.system.email_templates"] ?? "Email Templates"}
        description={
          t["admin.system.email_templates.desc"] ??
          "Edit email template subjects and bodies per language."
        }
      />

      <DataTable
        columns={columns}
        data={items}
        keyField="id"
        emptyMessage={t["admin.label.no_data"] ?? "No email templates found."}
        actions={(item) => (
          <button
            onClick={() => openEdit(item)}
            className="rounded border border-gray-300 px-3 py-1 text-xs font-medium text-gray-700 hover:bg-gray-50"
          >
            {t["admin.action.edit"] ?? "Edit"}
          </button>
        )}
      />

      <FormModal
        open={!!editing}
        wide
        title={
          (t["admin.email_templates.edit"] ?? "Edit Email Template") +
          (editing ? ` — ${editing.name}` : "")
        }
        onClose={() => setEditing(null)}
        onSubmit={handleSave}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        {languages.map((lang) => (
          <div key={lang.id} className="space-y-3">
            <h4 className="text-sm font-semibold text-gray-800">
              {lang.name} ({lang.isoCode.toUpperCase()})
            </h4>
            <FormField
              label={t["admin.label.subject"] ?? "Subject"}
              value={langFields[lang.id]?.subject ?? ""}
              onChange={(v) => updateField(lang.id, "subject", String(v))}
            />
            <FormField
              label={t["admin.label.body"] ?? "Body"}
              type="textarea"
              value={langFields[lang.id]?.body ?? ""}
              onChange={(v) => updateField(lang.id, "body", String(v))}
            />
            {lang.id !== languages[languages.length - 1]?.id && (
              <hr className="border-gray-200" />
            )}
          </div>
        ))}
      </FormModal>
    </>
  );
}
