"use client";

import { useState } from "react";
import {
  createPaymentMethod,
  updatePaymentMethod,
  deletePaymentMethod,
  togglePaymentMethodEnabled,
} from "./actions";
import { DataTable, type Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { StatusBadge } from "@/components/status-badge";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import type { AccessLevel } from "@/lib/permissions";
import type { PaymentMethodDto } from "@/types/payment";

interface PaymentMethodsTabProps {
  paymentMethods: PaymentMethodDto[];
  access: AccessLevel;
  t: Record<string, string>;
}

interface FormState {
  name: string;
  minDaysBeforeCheckin: number;
}

const emptyForm: FormState = { name: "", minDaysBeforeCheckin: 0 };

export function PaymentMethodsTab({
  paymentMethods: initial,
  access,
  t,
}: PaymentMethodsTabProps) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<PaymentMethodDto | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [deleting, setDeleting] = useState<PaymentMethodDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const isFull = access === "full";

  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    setModalOpen(true);
  }

  function openEdit(item: PaymentMethodDto) {
    setEditing(item);
    setForm({ name: item.name, minDaysBeforeCheckin: item.minDaysBeforeCheckin });
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
        const updated = await updatePaymentMethod(editing.id, form);
        setItems((prev) =>
          prev.map((i) => (i.id === editing.id ? { ...i, ...updated } : i))
        );
        setMessage(t["admin.message.updated"] ?? "Updated successfully.");
      } else {
        const created = await createPaymentMethod(form);
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
      await deletePaymentMethod(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      setMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setDeleting(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  async function handleToggleEnabled(item: PaymentMethodDto) {
    setPending(true);
    setMessage(null);
    try {
      await togglePaymentMethodEnabled(item.id);
      setItems((prev) =>
        prev.map((i) =>
          i.id === item.id ? { ...i, enabled: !i.enabled } : i
        )
      );
      const label = !item.enabled
        ? t["admin.label.enabled"] ?? "Enabled"
        : t["admin.label.disabled"] ?? "Disabled";
      setMessage(`"${item.name}" ${label.toLowerCase()}.`);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Toggle failed");
    } finally {
      setPending(false);
    }
  }

  const columns: Column<PaymentMethodDto>[] = [
    {
      key: "name",
      header: t["admin.field.name"] ?? "Name",
      render: (item) => (
        <span className="font-medium text-gray-900">{item.name}</span>
      ),
    },
    {
      key: "minDaysBeforeCheckin",
      header: t["admin.field.minDaysBeforeCheckin"] ?? "Min Days Before Check-in",
      className: "text-right",
    },
    {
      key: "enabled",
      header: t["admin.field.enabled"] ?? "Enabled",
      render: (item) => (
        <StatusBadge
          variant={item.enabled ? "enabled" : "disabled"}
          onClick={isFull ? () => handleToggleEnabled(item) : undefined}
          disabled={pending}
        >
          {item.enabled
            ? t["admin.label.enabled"] ?? "Enabled"
            : t["admin.label.disabled"] ?? "Disabled"}
        </StatusBadge>
      ),
    },
  ];

  return (
    <>
      <ToastMessage message={message} onDismiss={() => setMessage(null)} />

      {isFull && (
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
      )}

      <DataTable
        columns={columns}
        data={items}
        keyField="id"
        emptyMessage={t["admin.financial.paymentMethods.empty"] ?? "No payment methods found."}
        actions={
          isFull
            ? (item) => (
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
              )
            : undefined
        }
      />

      <FormModal
        open={modalOpen}
        title={
          editing
            ? t["admin.financial.paymentMethods.edit"] ?? "Edit Payment Method"
            : t["admin.financial.paymentMethods.create"] ?? "Create Payment Method"
        }
        onClose={closeModal}
        onSubmit={handleSubmit}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.field.name"] ?? "Name"}
          value={form.name}
          onChange={(v) => setForm((f) => ({ ...f, name: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.minDaysBeforeCheckin"] ?? "Min Days Before Check-in"}
          type="number"
          value={form.minDaysBeforeCheckin}
          onChange={(v) => setForm((f) => ({ ...f, minDaysBeforeCheckin: Number(v) }))}
          step="1"
          required
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleting}
        title={t["admin.confirm.delete.title"] ?? "Confirm Delete"}
        message={
          (t["admin.confirm.delete.message"] ?? 'Are you sure you want to delete "{name}"?').replace(
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
