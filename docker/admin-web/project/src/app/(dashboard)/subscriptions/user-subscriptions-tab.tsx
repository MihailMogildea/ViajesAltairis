"use client";

import { useState } from "react";
import { createUserSubscription, deleteUserSubscription } from "./actions";
import { DataTable, type Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { StatusBadge } from "@/components/status-badge";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import type { UserSubscriptionDto } from "@/types/user";
import type { SubscriptionTypeDto } from "@/types/subscription";

interface FormState {
  userId: number;
  subscriptionTypeId: string;
  startDate: string;
  endDate: string;
}

const emptyForm: FormState = { userId: 0, subscriptionTypeId: "", startDate: "", endDate: "" };

export function UserSubscriptionsTab({
  subscriptions: initial,
  types,
  access,
  t,
}: {
  subscriptions: UserSubscriptionDto[];
  types: SubscriptionTypeDto[];
  access: string | null;
  t: Record<string, string>;
}) {
  const [items, setItems] = useState(initial);
  const [modalOpen, setModalOpen] = useState(false);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [deleting, setDeleting] = useState<UserSubscriptionDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const typeMap = new Map(types.map((t) => [t.id, t]));

  function openCreate() {
    setForm(emptyForm);
    setModalOpen(true);
  }

  function closeModal() {
    setModalOpen(false);
    setForm(emptyForm);
  }

  async function handleSubmit() {
    setPending(true);
    setMessage(null);
    try {
      const payload = {
        userId: form.userId,
        subscriptionTypeId: Number(form.subscriptionTypeId),
        startDate: form.startDate,
        endDate: form.endDate || null,
      };
      const created = await createUserSubscription(payload);
      setItems((prev) => [...prev, created]);
      setMessage(t["admin.message.created"] ?? "Created successfully.");
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
      await deleteUserSubscription(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      setMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setDeleting(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  const typeOptions = types.map((t) => ({
    value: t.id,
    label: t.name,
  }));

  const columns: Column<UserSubscriptionDto>[] = [
    { key: "userEmail", header: t["admin.field.userEmail"] ?? "User" },
    {
      key: "subscriptionTypeId",
      header: t["admin.field.subscriptionType"] ?? "Subscription Type",
      render: (item) => typeMap.get(item.subscriptionTypeId)?.name ?? String(item.subscriptionTypeId),
    },
    {
      key: "startDate",
      header: t["admin.field.startDate"] ?? "Start Date",
      render: (item) => new Date(item.startDate).toLocaleDateString(),
    },
    {
      key: "endDate",
      header: t["admin.field.endDate"] ?? "End Date",
      render: (item) =>
        item.endDate ? new Date(item.endDate).toLocaleDateString() : (t["admin.label.none"] ?? "—"),
    },
    {
      key: "active",
      header: t["admin.field.status"] ?? "Status",
      render: (item) => (
        <StatusBadge variant={item.active ? "success" : "disabled"}>
          {item.active
            ? t["admin.label.active"] ?? "Active"
            : t["admin.label.inactive"] ?? "Inactive"}
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
        emptyMessage={t["admin.subscriptions.user.empty"] ?? "No user subscriptions found."}
        actions={
          access === "full"
            ? (item) => (
                <button
                  onClick={() => setDeleting(item)}
                  className="rounded border border-red-300 px-3 py-1 text-xs font-medium text-red-700 hover:bg-red-50"
                >
                  {t["admin.action.delete"] ?? "Delete"}
                </button>
              )
            : undefined
        }
      />

      <FormModal
        open={modalOpen}
        title={t["admin.subscriptions.user.create"] ?? "Assign Subscription"}
        onClose={closeModal}
        onSubmit={handleSubmit}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.field.userId"] ?? "User ID"}
          type="number"
          value={form.userId}
          onChange={(v) => setForm((f) => ({ ...f, userId: Number(v) }))}
          required
        />
        <FormField
          label={t["admin.field.subscriptionType"] ?? "Subscription Type"}
          type="select"
          value={form.subscriptionTypeId}
          onChange={(v) => setForm((f) => ({ ...f, subscriptionTypeId: String(v) }))}
          options={typeOptions}
          required
        />
        <FormField
          label={t["admin.field.startDate"] ?? "Start Date"}
          type="date"
          value={form.startDate}
          onChange={(v) => setForm((f) => ({ ...f, startDate: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.endDate"] ?? "End Date"}
          type="date"
          value={form.endDate}
          onChange={(v) => setForm((f) => ({ ...f, endDate: String(v) }))}
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleting}
        title={t["admin.confirm.delete.title"] ?? "Confirm Delete"}
        message={
          t["admin.subscriptions.user.delete.message"] ??
          "Are you sure you want to delete this user subscription?"
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
