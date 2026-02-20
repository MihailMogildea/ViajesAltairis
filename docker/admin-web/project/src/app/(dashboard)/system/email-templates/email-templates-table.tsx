"use client";

import { useState, useCallback } from "react";
import { EmailTemplateDto } from "@/types/system";
import { DataTable, Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import {
  createEmailTemplate,
  updateEmailTemplate,
  deleteEmailTemplate,
} from "./actions";

export function EmailTemplatesTable({
  emailTemplates: initial,
  t,
}: {
  emailTemplates: EmailTemplateDto[];
  t: Record<string, string>;
}) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<EmailTemplateDto | null>(null);
  const [deleting, setDeleting] = useState<EmailTemplateDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [showCreate, setShowCreate] = useState(false);

  const [formName, setFormName] = useState("");

  const dismissMessage = useCallback(() => setMessage(null), []);

  function openCreate() {
    setFormName("");
    setShowCreate(true);
    setMessage(null);
  }

  function openEdit(item: EmailTemplateDto) {
    setFormName(item.name);
    setEditing(item);
    setMessage(null);
  }

  async function handleSave() {
    setPending(true);
    setMessage(null);
    try {
      if (editing) {
        const updated = await updateEmailTemplate(editing.id, { name: formName });
        setItems((prev) => prev.map((i) => (i.id === editing.id ? updated : i)));
        setEditing(null);
        setMessage(t["admin.toast.updated"] ?? "Updated successfully.");
      } else {
        const created = await createEmailTemplate({ name: formName });
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
      await deleteEmailTemplate(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      setDeleting(null);
      setMessage(t["admin.toast.deleted"] ?? "Deleted successfully.");
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  const columns: Column<EmailTemplateDto>[] = [
    { key: "id", header: "ID", className: "w-20" },
    { key: "name", header: t["admin.label.name"] ?? "Name" },
  ];

  return (
    <>
      <ToastMessage message={message} onDismiss={dismissMessage} />

      <SectionHeader
        title={t["admin.system.email_templates"] ?? "Email Templates"}
        description={t["admin.system.email_templates.desc"] ?? "Manage email template definitions."}
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
        emptyMessage={t["admin.label.no_data"] ?? "No email templates found."}
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
            ? t["admin.email_templates.edit"] ?? "Edit Email Template"
            : t["admin.email_templates.create"] ?? "Create Email Template"
        }
        onClose={() => { setShowCreate(false); setEditing(null); }}
        onSubmit={handleSave}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.label.name"] ?? "Name"}
          value={formName}
          onChange={(v) => setFormName(String(v))}
          required
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleting}
        title={t["admin.confirm.delete_title"] ?? "Confirm Delete"}
        message={
          (t["admin.confirm.delete_message"] ?? 'Delete "{name}"?').replace(
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
