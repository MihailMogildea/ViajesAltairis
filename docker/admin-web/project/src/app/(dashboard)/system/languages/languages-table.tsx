"use client";

import { useState } from "react";
import { createLanguage, updateLanguage, deleteLanguage } from "./actions";
import { DataTable, type Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import type { LanguageDto } from "@/types/system";

interface FormState {
  isoCode: string;
  name: string;
}

const emptyForm: FormState = { isoCode: "", name: "" };

export function LanguagesTable({
  languages: initial,
  t,
}: {
  languages: LanguageDto[];
  t: Record<string, string>;
}) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<LanguageDto | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [deleting, setDeleting] = useState<LanguageDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    setModalOpen(true);
  }

  function openEdit(item: LanguageDto) {
    setEditing(item);
    setForm({ isoCode: item.isoCode, name: item.name });
    setModalOpen(true);
  }

  function closeModal() {
    setModalOpen(false);
    setEditing(null);
    setForm(emptyForm);
  }

  async function handleSubmit() {
    setPending(true);
    setMessage(null);
    try {
      if (editing) {
        const updated = await updateLanguage(editing.id, form);
        setItems((prev) =>
          prev.map((i) => (i.id === editing.id ? { ...i, ...updated } : i))
        );
        setMessage(t["admin.message.updated"] ?? "Updated successfully.");
      } else {
        const created = await createLanguage(form);
        setItems((prev) => [...prev, created]);
        setMessage(t["admin.message.created"] ?? "Created successfully.");
      }
      closeModal();
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Operation failed");
    } finally {
      setPending(false);
    }
  }

  async function handleDelete() {
    if (!deleting) return;
    setPending(true);
    setMessage(null);
    try {
      await deleteLanguage(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      setMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setDeleting(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  const columns: Column<LanguageDto>[] = [
    { key: "isoCode", header: t["admin.field.isoCode"] ?? "ISO Code" },
    { key: "name", header: t["admin.field.name"] ?? "Name" },
    {
      key: "createdAt",
      header: t["admin.field.createdAt"] ?? "Created",
      render: (item) => (
        <span className="text-xs text-gray-500">
          {new Date(item.createdAt).toLocaleDateString()}
        </span>
      ),
    },
  ];

  return (
    <>
      <ToastMessage message={message} onDismiss={() => setMessage(null)} />

      <SectionHeader
        title=""
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
        emptyMessage={t["admin.system.languages.empty"] ?? "No languages found."}
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
        open={modalOpen}
        title={
          editing
            ? t["admin.system.languages.edit"] ?? "Edit Language"
            : t["admin.system.languages.create"] ?? "Create Language"
        }
        onClose={closeModal}
        onSubmit={handleSubmit}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.field.isoCode"] ?? "ISO Code"}
          value={form.isoCode}
          onChange={(v) => setForm((f) => ({ ...f, isoCode: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.name"] ?? "Name"}
          value={form.name}
          onChange={(v) => setForm((f) => ({ ...f, name: String(v) }))}
          required
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleting}
        title={t["admin.confirm.delete.title"] ?? "Confirm Delete"}
        message={
          (t["admin.confirm.delete.message"] ?? "Are you sure you want to delete \"{name}\"?").replace(
            "{name}",
            deleting?.name ?? ""
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
