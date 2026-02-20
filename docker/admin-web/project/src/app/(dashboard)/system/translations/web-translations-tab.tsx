"use client";

import { useState, useCallback } from "react";
import { WebTranslationDto, LanguageDto } from "@/types/system";
import { DataTable, Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import {
  createWebTranslation,
  updateWebTranslation,
  deleteWebTranslation,
} from "./actions";

export function WebTranslationsTab({
  webTranslations: initial,
  languages,
  t,
}: {
  webTranslations: WebTranslationDto[];
  languages: LanguageDto[];
  t: Record<string, string>;
}) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<WebTranslationDto | null>(null);
  const [deleting, setDeleting] = useState<WebTranslationDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [showCreate, setShowCreate] = useState(false);

  const [formKey, setFormKey] = useState("");
  const [formLanguageId, setFormLanguageId] = useState<number | string>("");
  const [formValue, setFormValue] = useState("");

  const dismissMessage = useCallback(() => setMessage(null), []);

  function buildPayload() {
    return {
      translationKey: formKey,
      languageId: Number(formLanguageId),
      value: formValue,
    };
  }

  function openCreate() {
    setFormKey("");
    setFormLanguageId("");
    setFormValue("");
    setShowCreate(true);
    setMessage(null);
  }

  function openEdit(item: WebTranslationDto) {
    setFormKey(item.translationKey);
    setFormLanguageId(item.languageId);
    setFormValue(item.value);
    setEditing(item);
    setMessage(null);
  }

  async function handleSave() {
    setPending(true);
    setMessage(null);
    try {
      const payload = buildPayload();
      if (editing) {
        const updated = await updateWebTranslation(editing.id, payload);
        setItems((prev) => prev.map((i) => (i.id === editing.id ? updated : i)));
        setEditing(null);
        setMessage(t["admin.toast.updated"] ?? "Updated successfully.");
      } else {
        const created = await createWebTranslation(payload);
        setItems((prev) => [...prev, created]);
        setShowCreate(false);
        setMessage(t["admin.toast.created"] ?? "Created successfully.");
      }
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Save failed");
    } finally {
      setPending(false);
    }
  }

  async function handleDelete() {
    if (!deleting) return;
    setPending(true);
    setMessage(null);
    try {
      await deleteWebTranslation(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      setDeleting(null);
      setMessage(t["admin.toast.deleted"] ?? "Deleted successfully.");
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  function lookupLanguage(id: number) {
    return languages.find((l) => l.id === id)?.isoCode ?? String(id);
  }

  const languageOptions = languages.map((l) => ({
    value: l.id,
    label: `${l.isoCode} - ${l.name}`,
  }));

  const columns: Column<WebTranslationDto>[] = [
    { key: "id", header: "ID", className: "w-16" },
    {
      key: "translationKey",
      header: t["admin.label.translation_key"] ?? "Translation Key",
    },
    {
      key: "languageId",
      header: t["admin.label.language"] ?? "Language",
      render: (item) => lookupLanguage(item.languageId),
    },
    {
      key: "value",
      header: t["admin.label.value"] ?? "Value",
      render: (item) =>
        item.value.length > 60 ? item.value.slice(0, 60) + "..." : item.value,
    },
  ];

  return (
    <>
      <ToastMessage message={message} onDismiss={dismissMessage} />

      <SectionHeader
        title={t["admin.translations.web"] ?? "Web Translations"}
        description={t["admin.translations.web.desc"] ?? "Manage web UI translations."}
        action={
          <button
            onClick={openCreate}
            className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
          >
            {t["admin.action.create"] ?? "Create"}
          </button>
        }
      />

      <DataTable
        columns={columns}
        data={items}
        keyField="id"
        emptyMessage={t["admin.label.no_data"] ?? "No web translations found."}
        actions={(item) => (
          <>
            <button
              onClick={() => openEdit(item)}
              className="rounded border border-gray-300 px-3 py-1 text-xs font-medium text-gray-700 hover:bg-gray-50"
            >
              {t["admin.action.edit"] ?? "Edit"}
            </button>
            <button
              onClick={() => setDeleting(item)}
              className="rounded border border-red-300 px-3 py-1 text-xs font-medium text-red-700 hover:bg-red-50"
            >
              {t["admin.action.delete"] ?? "Delete"}
            </button>
          </>
        )}
      />

      <FormModal
        open={showCreate || !!editing}
        title={
          editing
            ? t["admin.web_translations.edit"] ?? "Edit Web Translation"
            : t["admin.web_translations.create"] ?? "Create Web Translation"
        }
        onClose={() => { setShowCreate(false); setEditing(null); }}
        onSubmit={handleSave}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.label.translation_key"] ?? "Translation Key"}
          value={formKey}
          onChange={(v) => setFormKey(String(v))}
          required
        />
        <FormField
          label={t["admin.label.language"] ?? "Language"}
          type="select"
          value={formLanguageId}
          onChange={(v) => setFormLanguageId(v as number)}
          options={languageOptions}
          required
        />
        <FormField
          label={t["admin.label.value"] ?? "Value"}
          type="textarea"
          value={formValue}
          onChange={(v) => setFormValue(String(v))}
          required
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleting}
        title={t["admin.confirm.delete_title"] ?? "Confirm Delete"}
        message={
          (t["admin.confirm.delete_message"] ?? "Delete web translation #{id}?").replace(
            "{id}",
            String(deleting?.id ?? "")
          )
        }
        onConfirm={handleDelete}
        onCancel={() => setDeleting(null)}
        loading={pending}
        confirmLabel={t["admin.action.delete"] ?? "Delete"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      />
    </>
  );
}
