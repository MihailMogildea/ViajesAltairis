"use client";

import { useState, useCallback } from "react";
import { TaxTypeDto } from "@/types/system";
import { DataTable, Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import { createTaxType, updateTaxType, deleteTaxType } from "./actions";

export function TaxTypesTab({
  taxTypes: initial,
  t,
}: {
  taxTypes: TaxTypeDto[];
  t: Record<string, string>;
}) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<TaxTypeDto | null>(null);
  const [deleting, setDeleting] = useState<TaxTypeDto | null>(null);
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

  function openEdit(item: TaxTypeDto) {
    setFormName(item.name);
    setEditing(item);
    setMessage(null);
  }

  async function handleSave() {
    setPending(true);
    setMessage(null);
    try {
      if (editing) {
        const updated = await updateTaxType(editing.id, { name: formName });
        setItems((prev) => prev.map((i) => (i.id === editing.id ? updated : i)));
        setEditing(null);
        setMessage(t["admin.toast.updated"] ?? "Updated successfully.");
      } else {
        const created = await createTaxType({ name: formName });
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
      await deleteTaxType(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      setDeleting(null);
      setMessage(t["admin.toast.deleted"] ?? "Deleted successfully.");
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  const columns: Column<TaxTypeDto>[] = [
    { key: "id", header: "ID", className: "w-20" },
    { key: "name", header: t["admin.label.name"] ?? "Name" },
    {
      key: "createdAt",
      header: t["admin.label.created_at"] ?? "Created",
      render: (item) => new Date(item.createdAt).toLocaleDateString(),
    },
  ];

  return (
    <>
      <ToastMessage message={message} onDismiss={dismissMessage} />

      <SectionHeader
        title={t["admin.taxes.tax_types"] ?? "Tax Types"}
        description={t["admin.taxes.tax_types.desc"] ?? "Manage tax type definitions."}
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
        emptyMessage={t["admin.label.no_data"] ?? "No tax types found."}
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
            ? t["admin.tax_types.edit"] ?? "Edit Tax Type"
            : t["admin.tax_types.create"] ?? "Create Tax Type"
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
