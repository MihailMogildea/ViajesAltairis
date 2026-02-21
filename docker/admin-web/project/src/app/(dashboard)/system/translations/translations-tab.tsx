"use client";

import { useState, useCallback, useMemo } from "react";
import { TranslationDto, LanguageDto } from "@/types/system";
import { DataTable, Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import {
  createTranslation,
  updateTranslation,
  deleteTranslation,
} from "./actions";

export function TranslationsTab({
  translations: initial,
  languages,
  t,
}: {
  translations: TranslationDto[];
  languages: LanguageDto[];
  t: Record<string, string>;
}) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<TranslationDto | null>(null);
  const [deleting, setDeleting] = useState<TranslationDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [showCreate, setShowCreate] = useState(false);
  const [collapsed, setCollapsed] = useState<Record<string, boolean>>(() => {
    const map: Record<string, boolean> = {};
    for (const item of initial) map[item.entityType] = true;
    return map;
  });

  const [formEntityType, setFormEntityType] = useState("");
  const [formEntityId, setFormEntityId] = useState<number>(0);
  const [formField, setFormField] = useState("");
  const [formLanguageId, setFormLanguageId] = useState<number | string>("");
  const [formValue, setFormValue] = useState("");

  const dismissMessage = useCallback(() => setMessage(null), []);

  // Group items by entityType
  const grouped = useMemo(() => {
    const map = new Map<string, TranslationDto[]>();
    for (const item of items) {
      const group = map.get(item.entityType) ?? [];
      group.push(item);
      map.set(item.entityType, group);
    }
    // Sort groups alphabetically
    return [...map.entries()].sort(([a], [b]) => a.localeCompare(b));
  }, [items]);

  function toggleGroup(key: string) {
    setCollapsed((prev) => ({ ...prev, [key]: !prev[key] }));
  }

  function buildPayload() {
    return {
      entityType: formEntityType,
      entityId: formEntityId,
      field: formField,
      languageId: Number(formLanguageId),
      value: formValue,
    };
  }

  function openCreate() {
    setFormEntityType("");
    setFormEntityId(0);
    setFormField("");
    setFormLanguageId("");
    setFormValue("");
    setShowCreate(true);
    setMessage(null);
  }

  function openEdit(item: TranslationDto) {
    setFormEntityType(item.entityType);
    setFormEntityId(item.entityId);
    setFormField(item.field);
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
        const updated = await updateTranslation(editing.id, payload);
        setItems((prev) => prev.map((i) => (i.id === editing.id ? updated : i)));
        setEditing(null);
        setMessage(t["admin.toast.updated"] ?? "Updated successfully.");
      } else {
        const created = await createTranslation(payload);
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
      await deleteTranslation(deleting.id);
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

  const columns: Column<TranslationDto>[] = [
    { key: "id", header: "ID", className: "w-16" },
    { key: "entityId", header: t["admin.label.entity_id"] ?? "Entity ID", className: "w-24" },
    { key: "field", header: t["admin.label.field"] ?? "Field" },
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
        title={t["admin.translations.translations"] ?? "Translations"}
        description={t["admin.translations.translations.desc"] ?? "Manage entity translations."}
        action={
          <button
            onClick={openCreate}
            className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
          >
            {t["admin.action.create"] ?? "Create"}
          </button>
        }
      />

      {grouped.length === 0 ? (
        <div className="rounded-lg border border-gray-200 bg-white p-8 text-center text-sm text-gray-500">
          {t["admin.label.no_data"] ?? "No translations found."}
        </div>
      ) : (
        <div className="space-y-3">
          {grouped.map(([entityType, groupItems]) => (
            <div key={entityType} className="overflow-hidden rounded-lg border border-gray-200 bg-white">
              <button
                onClick={() => toggleGroup(entityType)}
                className="flex w-full items-center justify-between bg-gray-50 px-4 py-3 text-left hover:bg-gray-100"
              >
                <span className="text-sm font-semibold text-gray-800">
                  {entityType}
                  <span className="ml-2 text-xs font-normal text-gray-400">
                    ({groupItems.length})
                  </span>
                </span>
                <svg
                  className={`h-4 w-4 text-gray-400 transition-transform ${collapsed[entityType] ? "" : "rotate-180"}`}
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                  strokeWidth={2}
                >
                  <path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7" />
                </svg>
              </button>
              {!collapsed[entityType] && (
                <DataTable
                  columns={columns}
                  data={groupItems}
                  keyField="id"
                  emptyMessage=""
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
              )}
            </div>
          ))}
        </div>
      )}

      <FormModal
        open={showCreate || !!editing}
        title={
          editing
            ? t["admin.translations.edit"] ?? "Edit Translation"
            : t["admin.translations.create"] ?? "Create Translation"
        }
        onClose={() => { setShowCreate(false); setEditing(null); }}
        onSubmit={handleSave}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.label.entity_type"] ?? "Entity Type"}
          value={formEntityType}
          onChange={(v) => setFormEntityType(String(v))}
          required
        />
        <FormField
          label={t["admin.label.entity_id"] ?? "Entity ID"}
          type="number"
          value={formEntityId}
          onChange={(v) => setFormEntityId(Number(v))}
          required
        />
        <FormField
          label={t["admin.label.field"] ?? "Field"}
          value={formField}
          onChange={(v) => setFormField(String(v))}
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
          (t["admin.confirm.delete_message"] ?? "Delete translation #{id}?").replace(
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
