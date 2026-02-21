"use client";

import { useState, useCallback, useMemo } from "react";
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

interface SubGroup {
  subKey: string;
  items: WebTranslationDto[];
}

interface Group {
  groupKey: string;
  subGroups: SubGroup[];
  totalCount: number;
}

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
  const [collapsedGroups, setCollapsedGroups] = useState<Record<string, boolean>>(() => {
    const map: Record<string, boolean> = {};
    for (const item of initial) {
      const groupKey = item.translationKey.split(".")[0] ?? "(root)";
      map[groupKey] = true;
    }
    return map;
  });
  const [collapsedSubs, setCollapsedSubs] = useState<Record<string, boolean>>(() => {
    const map: Record<string, boolean> = {};
    for (const item of initial) {
      const parts = item.translationKey.split(".");
      const subId = `${parts[0] ?? "(root)"}.${parts[1] ?? "(root)"}`;
      map[subId] = true;
    }
    return map;
  });

  const [formKey, setFormKey] = useState("");
  const [formLanguageId, setFormLanguageId] = useState<number | string>("");
  const [formValue, setFormValue] = useState("");

  const dismissMessage = useCallback(() => setMessage(null), []);

  // Two-level grouping: "admin.home.login" → group "admin", sub "home"
  const grouped = useMemo((): Group[] => {
    const map = new Map<string, Map<string, WebTranslationDto[]>>();
    for (const item of items) {
      const parts = item.translationKey.split(".");
      const groupKey = parts[0] ?? "(root)";
      const subKey = parts[1] ?? "(root)";
      if (!map.has(groupKey)) map.set(groupKey, new Map());
      const subMap = map.get(groupKey)!;
      if (!subMap.has(subKey)) subMap.set(subKey, []);
      subMap.get(subKey)!.push(item);
    }
    return [...map.entries()]
      .sort(([a], [b]) => a.localeCompare(b))
      .map(([groupKey, subMap]) => ({
        groupKey,
        totalCount: [...subMap.values()].reduce((sum, arr) => sum + arr.length, 0),
        subGroups: [...subMap.entries()]
          .sort(([a], [b]) => a.localeCompare(b))
          .map(([subKey, items]) => ({ subKey, items })),
      }));
  }, [items]);

  function toggleGroup(key: string) {
    setCollapsedGroups((prev) => ({ ...prev, [key]: !prev[key] }));
  }

  function toggleSub(key: string) {
    setCollapsedSubs((prev) => ({ ...prev, [key]: !prev[key] }));
  }

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
      header: t["admin.label.translation_key"] ?? "Key",
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

      {grouped.length === 0 ? (
        <div className="rounded-lg border border-gray-200 bg-white p-8 text-center text-sm text-gray-500">
          {t["admin.label.no_data"] ?? "No web translations found."}
        </div>
      ) : (
        <div className="space-y-3">
          {grouped.map(({ groupKey, subGroups, totalCount }) => (
            <div key={groupKey} className="overflow-hidden rounded-lg border border-gray-200 bg-white">
              {/* Top-level group header */}
              <button
                onClick={() => toggleGroup(groupKey)}
                className="flex w-full items-center justify-between bg-gray-50 px-4 py-3 text-left hover:bg-gray-100"
              >
                <span className="text-sm font-semibold text-gray-800">
                  {groupKey}
                  <span className="ml-2 text-xs font-normal text-gray-400">
                    ({totalCount})
                  </span>
                </span>
                <svg
                  className={`h-4 w-4 text-gray-400 transition-transform ${collapsedGroups[groupKey] ? "" : "rotate-180"}`}
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                  strokeWidth={2}
                >
                  <path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7" />
                </svg>
              </button>

              {!collapsedGroups[groupKey] && (
                <div className="divide-y divide-gray-100">
                  {subGroups.map(({ subKey, items: subItems }) => {
                    const subId = `${groupKey}.${subKey}`;
                    return (
                      <div key={subId}>
                        {/* Sub-group header */}
                        <button
                          onClick={() => toggleSub(subId)}
                          className="flex w-full items-center justify-between bg-gray-50/50 px-6 py-2 text-left hover:bg-gray-100/50"
                        >
                          <span className="text-xs font-medium text-gray-600">
                            {subKey}
                            <span className="ml-2 text-xs font-normal text-gray-400">
                              ({subItems.length})
                            </span>
                          </span>
                          <svg
                            className={`h-3 w-3 text-gray-400 transition-transform ${collapsedSubs[subId] ? "" : "rotate-180"}`}
                            fill="none"
                            viewBox="0 0 24 24"
                            stroke="currentColor"
                            strokeWidth={2}
                          >
                            <path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7" />
                          </svg>
                        </button>
                        {!collapsedSubs[subId] && (
                          <DataTable
                            columns={columns}
                            data={subItems}
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
                    );
                  })}
                </div>
              )}
            </div>
          ))}
        </div>
      )}

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
