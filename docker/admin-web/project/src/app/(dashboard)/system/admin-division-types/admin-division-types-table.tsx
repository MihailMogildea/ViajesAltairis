"use client";

import { useState, useCallback } from "react";
import { DataTable, type Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import {
  createAdminDivisionType,
  updateAdminDivisionType,
  deleteAdminDivisionType,
} from "./actions";
import type { AdministrativeDivisionTypeDto } from "@/types/system";

interface Props {
  types: AdministrativeDivisionTypeDto[];
  t: Record<string, string>;
}

const emptyForm = { name: "" };

export function AdminDivisionTypesTable({ types: initial, t }: Props) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<AdministrativeDivisionTypeDto | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [modalOpen, setModalOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<AdministrativeDivisionTypeDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const dismissMessage = useCallback(() => setMessage(null), []);

  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    setModalOpen(true);
    setMessage(null);
  }

  function openEdit(item: AdministrativeDivisionTypeDto) {
    setEditing(item);
    setForm({ name: item.name });
    setModalOpen(true);
    setMessage(null);
  }

  function closeModal() {
    setModalOpen(false);
    setEditing(null);
  }

  async function handleSave() {
    setPending(true);
    setMessage(null);
    try {
      const payload = { name: form.name };
      if (editing) {
        const updated = await updateAdminDivisionType(editing.id, payload);
        setItems((prev) => prev.map((i) => (i.id === editing.id ? updated : i)));
        setMessage(t["admin.label.updated"] ?? "Updated successfully.");
      } else {
        const created = await createAdminDivisionType(payload);
        setItems((prev) => [...prev, created]);
        setMessage(t["admin.label.created"] ?? "Created successfully.");
      }
      closeModal();
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Save failed");
    } finally {
      setPending(false);
    }
  }

  async function handleDelete() {
    if (!deleteTarget) return;
    setPending(true);
    setMessage(null);
    try {
      await deleteAdminDivisionType(deleteTarget.id);
      setItems((prev) => prev.filter((i) => i.id !== deleteTarget.id));
      setMessage(t["admin.label.deleted"] ?? "Deleted successfully.");
      setDeleteTarget(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  const columns: Column<AdministrativeDivisionTypeDto>[] = [
    {
      key: "name",
      header: t["admin.label.name"] ?? "Name",
    },
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
        title=""
        action={
          <button
            onClick={openCreate}
            className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
          >
            {t["admin.action.add"] ?? "Add"}
          </button>
        }
      />

      <DataTable
        columns={columns}
        data={items}
        keyField="id"
        emptyMessage={t["admin.admin_division_types.empty"] ?? "No division types found."}
        actions={(item) => (
          <>
            <button
              onClick={() => openEdit(item)}
              className="rounded border border-gray-300 px-3 py-1 text-xs font-medium text-gray-700 hover:bg-gray-50"
            >
              {t["admin.action.edit"] ?? "Edit"}
            </button>
            <button
              onClick={() => setDeleteTarget(item)}
              className="rounded border border-red-300 px-3 py-1 text-xs font-medium text-red-700 hover:bg-red-50"
            >
              {t["admin.action.delete"] ?? "Delete"}
            </button>
          </>
        )}
      />

      <FormModal
        open={modalOpen}
        title={editing ? (t["admin.admin_division_types.edit"] ?? "Edit Division Type") : (t["admin.admin_division_types.add"] ?? "Add Division Type")}
        onClose={closeModal}
        onSubmit={handleSave}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.label.name"] ?? "Name"}
          type="text"
          value={form.name}
          onChange={(v) => setForm({ ...form, name: String(v) })}
          required
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleteTarget}
        title={t["admin.confirm.delete_title"] ?? "Delete Division Type"}
        message={t["admin.confirm.delete_message"] ?? "Are you sure you want to delete this division type?"}
        onConfirm={handleDelete}
        onCancel={() => setDeleteTarget(null)}
        loading={pending}
        confirmLabel={t["admin.action.delete"] ?? "Delete"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      />
    </>
  );
}
