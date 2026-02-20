"use client";

import { useState } from "react";
import Link from "next/link";
import {
  createProvider,
  deleteProvider,
  toggleProviderEnabled,
} from "./actions";
import { DataTable, type Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { StatusBadge } from "@/components/status-badge";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import type { ProviderDto, ProviderTypeDto } from "@/types/provider";
import type { CurrencyDto } from "@/types/system";
import type { AccessLevel } from "@/lib/permissions";

interface FormState {
  typeId: number;
  currencyId: number;
  name: string;
  apiUrl: string;
  apiUsername: string;
  apiPassword: string;
  margin: number;
}

const emptyForm: FormState = {
  typeId: 0,
  currencyId: 0,
  name: "",
  apiUrl: "",
  apiUsername: "",
  apiPassword: "",
  margin: 0,
};

export function ProvidersTable({
  providers: initial,
  providerTypes,
  currencies,
  t,
  access,
}: {
  providers: ProviderDto[];
  providerTypes: ProviderTypeDto[];
  currencies: CurrencyDto[];
  t: Record<string, string>;
  access: AccessLevel | null;
}) {
  const [items, setItems] = useState(initial);
  const [modalOpen, setModalOpen] = useState(false);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [deleting, setDeleting] = useState<ProviderDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const isFull = access === "full";

  const typeMap = new Map(providerTypes.map((pt) => [pt.id, pt.name]));

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
        typeId: form.typeId,
        currencyId: form.currencyId,
        name: form.name,
        apiUrl: form.apiUrl || null,
        apiUsername: form.apiUsername || null,
        apiPassword: form.apiPassword || null,
        margin: form.margin,
      };
      const created = await createProvider(payload);
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
      await deleteProvider(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      setMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setDeleting(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  async function handleToggleEnabled(item: ProviderDto) {
    setPending(true);
    setMessage(null);
    try {
      const newEnabled = !item.enabled;
      await toggleProviderEnabled(item.id, newEnabled);
      setItems((prev) =>
        prev.map((i) =>
          i.id === item.id ? { ...i, enabled: newEnabled } : i
        )
      );
      const label = newEnabled
        ? t["admin.label.enabled"] ?? "Enabled"
        : t["admin.label.disabled"] ?? "Disabled";
      setMessage(`"${item.name}" ${label.toLowerCase()}.`);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Toggle failed");
    } finally {
      setPending(false);
    }
  }

  const columns: Column<ProviderDto>[] = [
    {
      key: "name",
      header: t["admin.field.name"] ?? "Name",
      render: (item) => (
        <Link
          href={`/providers/${item.id}`}
          className="font-medium text-blue-600 hover:underline"
        >
          {item.name}
        </Link>
      ),
    },
    {
      key: "typeId",
      header: t["admin.field.type"] ?? "Type",
      render: (item) => <span>{typeMap.get(item.typeId) ?? item.typeId}</span>,
    },
    {
      key: "margin",
      header: t["admin.field.margin"] ?? "Margin",
      render: (item) => <span>{item.margin}%</span>,
    },
    {
      key: "syncStatus",
      header: t["admin.field.syncStatus"] ?? "Sync Status",
      render: (item) => (
        <span className="text-xs text-gray-500">
          {item.syncStatus ?? "-"}
        </span>
      ),
    },
    {
      key: "enabled",
      header: t["admin.field.enabled"] ?? "Status",
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
        emptyMessage={t["admin.providers.empty"] ?? "No providers found."}
        actions={
          isFull
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
        title={t["admin.providers.create"] ?? "Create Provider"}
        onClose={closeModal}
        onSubmit={handleSubmit}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.field.type"] ?? "Provider Type"}
          type="select"
          value={form.typeId}
          onChange={(v) => setForm((f) => ({ ...f, typeId: Number(v) }))}
          options={providerTypes.map((pt) => ({ value: pt.id, label: pt.name }))}
          required
        />
        <FormField
          label={t["admin.field.currency"] ?? "Currency"}
          type="select"
          value={form.currencyId}
          onChange={(v) => setForm((f) => ({ ...f, currencyId: Number(v) }))}
          options={currencies.map((c) => ({ value: c.id, label: `${c.isoCode} - ${c.name}` }))}
          required
        />
        <FormField
          label={t["admin.field.name"] ?? "Name"}
          value={form.name}
          onChange={(v) => setForm((f) => ({ ...f, name: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.apiUrl"] ?? "API URL"}
          value={form.apiUrl}
          onChange={(v) => setForm((f) => ({ ...f, apiUrl: String(v) }))}
        />
        <FormField
          label={t["admin.field.apiUsername"] ?? "API Username"}
          value={form.apiUsername}
          onChange={(v) => setForm((f) => ({ ...f, apiUsername: String(v) }))}
        />
        <FormField
          label={t["admin.field.apiPassword"] ?? "API Password"}
          type="password"
          value={form.apiPassword}
          onChange={(v) => setForm((f) => ({ ...f, apiPassword: String(v) }))}
        />
        <FormField
          label={t["admin.field.margin"] ?? "Margin (%)"}
          type="number"
          step="0.01"
          value={form.margin}
          onChange={(v) => setForm((f) => ({ ...f, margin: Number(v) }))}
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
