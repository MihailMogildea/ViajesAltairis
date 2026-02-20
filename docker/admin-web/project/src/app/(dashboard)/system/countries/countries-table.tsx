"use client";

import { useState } from "react";
import {
  createCountry,
  updateCountry,
  deleteCountry,
  toggleCountryEnabled,
} from "./actions";
import { DataTable, type Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { StatusBadge } from "@/components/status-badge";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import type { CountryDto, CurrencyDto } from "@/types/system";

interface FormState {
  isoCode: string;
  name: string;
  currencyId: string;
}

const emptyForm: FormState = { isoCode: "", name: "", currencyId: "" };

export function CountriesTable({
  countries: initial,
  currencies,
  t,
}: {
  countries: CountryDto[];
  currencies: CurrencyDto[];
  t: Record<string, string>;
}) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<CountryDto | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [deleting, setDeleting] = useState<CountryDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const currencyOptions = currencies.map((c) => ({
    value: c.id,
    label: `${c.isoCode} - ${c.name}`,
  }));

  function currencyName(currencyId: number): string {
    const c = currencies.find((cur) => cur.id === currencyId);
    return c ? c.isoCode : String(currencyId);
  }

  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    setModalOpen(true);
  }

  function openEdit(item: CountryDto) {
    setEditing(item);
    setForm({
      isoCode: item.isoCode,
      name: item.name,
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
        isoCode: form.isoCode,
        name: form.name,
        currencyId: Number(form.currencyId),
      };
      if (editing) {
        const updated = await updateCountry(editing.id, payload);
        setItems((prev) =>
          prev.map((i) => (i.id === editing.id ? { ...i, ...updated } : i))
        );
        setMessage(t["admin.message.updated"] ?? "Updated successfully.");
      } else {
        const created = await createCountry(payload);
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
      await deleteCountry(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      setMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setDeleting(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  async function handleToggle(item: CountryDto) {
    setPending(true);
    setMessage(null);
    try {
      await toggleCountryEnabled(item.id, !item.enabled);
      setItems((prev) =>
        prev.map((i) =>
          i.id === item.id ? { ...i, enabled: !item.enabled } : i
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

  const columns: Column<CountryDto>[] = [
    { key: "isoCode", header: t["admin.field.isoCode"] ?? "ISO Code" },
    { key: "name", header: t["admin.field.name"] ?? "Name" },
    {
      key: "currencyId",
      header: t["admin.field.currency"] ?? "Currency",
      render: (item) => currencyName(item.currencyId),
    },
    {
      key: "enabled",
      header: t["admin.field.status"] ?? "Status",
      render: (item) => (
        <StatusBadge
          variant={item.enabled ? "enabled" : "disabled"}
          onClick={() => handleToggle(item)}
          disabled={pending}
        >
          {item.enabled
            ? t["admin.label.enabled"] ?? "Enabled"
            : t["admin.label.disabled"] ?? "Disabled"}
        </StatusBadge>
      ),
    },
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
        emptyMessage={t["admin.system.countries.empty"] ?? "No countries found."}
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
            ? t["admin.system.countries.edit"] ?? "Edit Country"
            : t["admin.system.countries.create"] ?? "Create Country"
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
