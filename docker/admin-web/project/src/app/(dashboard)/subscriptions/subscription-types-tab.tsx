"use client";

import { useState } from "react";
import {
  createSubscriptionType,
  updateSubscriptionType,
  deleteSubscriptionType,
  toggleSubscriptionTypeEnabled,
} from "./actions";
import { DataTable, type Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { StatusBadge } from "@/components/status-badge";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import type { SubscriptionTypeDto } from "@/types/subscription";
import type { CurrencyDto } from "@/types/system";

interface FormState {
  name: string;
  pricePerMonth: number;
  discount: number;
  currencyId: string;
}

const emptyForm: FormState = { name: "", pricePerMonth: 0, discount: 0, currencyId: "" };

export function SubscriptionTypesTab({
  types: initial,
  currencies,
  access,
  t,
}: {
  types: SubscriptionTypeDto[];
  currencies: CurrencyDto[];
  access: string | null;
  t: Record<string, string>;
}) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<SubscriptionTypeDto | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [deleting, setDeleting] = useState<SubscriptionTypeDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const currencyMap = new Map(currencies.map((c) => [c.id, c]));

  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    setModalOpen(true);
  }

  function openEdit(item: SubscriptionTypeDto) {
    setEditing(item);
    setForm({
      name: item.name,
      pricePerMonth: item.pricePerMonth,
      discount: item.discount,
      currencyId: String(item.currencyId),
    });
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
      const payload = {
        name: form.name,
        pricePerMonth: form.pricePerMonth,
        discount: form.discount,
        currencyId: Number(form.currencyId),
      };
      if (editing) {
        const updated = await updateSubscriptionType(editing.id, payload);
        setItems((prev) =>
          prev.map((i) => (i.id === editing.id ? { ...i, ...updated } : i))
        );
        setMessage(t["admin.message.updated"] ?? "Updated successfully.");
      } else {
        const created = await createSubscriptionType(payload);
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
      await deleteSubscriptionType(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      setMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setDeleting(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  async function handleToggle(item: SubscriptionTypeDto) {
    setPending(true);
    setMessage(null);
    try {
      const updated = await toggleSubscriptionTypeEnabled(item.id);
      setItems((prev) =>
        prev.map((i) => (i.id === item.id ? { ...i, ...updated } : i))
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

  const currencyOptions = currencies.map((c) => ({
    value: c.id,
    label: `${c.isoCode} - ${c.name}`,
  }));

  const columns: Column<SubscriptionTypeDto>[] = [
    { key: "name", header: t["admin.field.name"] ?? "Name" },
    {
      key: "pricePerMonth",
      header: t["admin.field.pricePerMonth"] ?? "Price / Month",
      render: (item) => item.pricePerMonth.toFixed(2),
    },
    {
      key: "discount",
      header: t["admin.field.discount"] ?? "Discount",
      render: (item) => `${(item.discount * 100).toFixed(1)}%`,
    },
    {
      key: "currencyId",
      header: t["admin.field.currency"] ?? "Currency",
      render: (item) => currencyMap.get(item.currencyId)?.isoCode ?? String(item.currencyId),
    },
    {
      key: "enabled",
      header: t["admin.field.status"] ?? "Status",
      render: (item) => (
        <StatusBadge
          variant={item.enabled ? "enabled" : "disabled"}
          onClick={access === "full" ? () => handleToggle(item) : undefined}
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

      {access === "full" && (
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
        emptyMessage={t["admin.subscriptions.types.empty"] ?? "No subscription types found."}
        actions={
          access === "full"
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
            ? t["admin.subscriptions.types.edit"] ?? "Edit Subscription Type"
            : t["admin.subscriptions.types.create"] ?? "Create Subscription Type"
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
          label={t["admin.field.pricePerMonth"] ?? "Price / Month"}
          type="number"
          step="0.01"
          value={form.pricePerMonth}
          onChange={(v) => setForm((f) => ({ ...f, pricePerMonth: Number(v) }))}
          required
        />
        <FormField
          label={t["admin.field.discount"] ?? "Discount"}
          type="number"
          step="0.01"
          value={form.discount}
          onChange={(v) => setForm((f) => ({ ...f, discount: Number(v) }))}
          required
        />
        <FormField
          label={t["admin.field.currency"] ?? "Currency"}
          type="select"
          value={form.currencyId}
          onChange={(v) => setForm((f) => ({ ...f, currencyId: String(v) }))}
          options={currencyOptions}
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
