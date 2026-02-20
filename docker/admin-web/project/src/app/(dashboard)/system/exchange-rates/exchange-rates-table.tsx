"use client";

import { useState, useCallback } from "react";
import { DataTable, type Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import { createExchangeRate, updateExchangeRate, deleteExchangeRate } from "./actions";
import type { ExchangeRateDto, CurrencyDto } from "@/types/system";

interface Props {
  rates: ExchangeRateDto[];
  currencies: CurrencyDto[];
  t: Record<string, string>;
}

const emptyForm = { currencyId: 0, rateToEur: 0, validFrom: "", validTo: "" };

export function ExchangeRatesTable({ rates: initial, currencies, t }: Props) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<ExchangeRateDto | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [modalOpen, setModalOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<ExchangeRateDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const dismissMessage = useCallback(() => setMessage(null), []);

  const currencyOptions = currencies.map((c) => ({
    value: c.id,
    label: `${c.isoCode} - ${c.name}`,
  }));

  function currencyLabel(currencyId: number) {
    const c = currencies.find((x) => x.id === currencyId);
    return c ? c.isoCode : String(currencyId);
  }

  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    setModalOpen(true);
    setMessage(null);
  }

  function openEdit(item: ExchangeRateDto) {
    setEditing(item);
    setForm({
      currencyId: item.currencyId,
      rateToEur: item.rateToEur,
      validFrom: item.validFrom?.split("T")[0] ?? "",
      validTo: item.validTo?.split("T")[0] ?? "",
    });
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
      const payload = {
        currencyId: Number(form.currencyId),
        rateToEur: Number(form.rateToEur),
        validFrom: form.validFrom,
        validTo: form.validTo,
      };
      if (editing) {
        const updated = await updateExchangeRate(editing.id, payload);
        setItems((prev) => prev.map((i) => (i.id === editing.id ? updated : i)));
        setMessage(t["admin.label.updated"] ?? "Updated successfully.");
      } else {
        const created = await createExchangeRate(payload);
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
      await deleteExchangeRate(deleteTarget.id);
      setItems((prev) => prev.filter((i) => i.id !== deleteTarget.id));
      setMessage(t["admin.label.deleted"] ?? "Deleted successfully.");
      setDeleteTarget(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  const columns: Column<ExchangeRateDto>[] = [
    {
      key: "currencyId",
      header: t["admin.exchange_rates.col.currency"] ?? "Currency",
      render: (item) => currencyLabel(item.currencyId),
    },
    {
      key: "rateToEur",
      header: t["admin.exchange_rates.col.rate"] ?? "Rate to EUR",
      render: (item) => item.rateToEur.toFixed(6),
    },
    {
      key: "validFrom",
      header: t["admin.exchange_rates.col.valid_from"] ?? "Valid From",
      render: (item) => item.validFrom?.split("T")[0] ?? "",
    },
    {
      key: "validTo",
      header: t["admin.exchange_rates.col.valid_to"] ?? "Valid To",
      render: (item) => item.validTo?.split("T")[0] ?? "",
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
        emptyMessage={t["admin.exchange_rates.empty"] ?? "No exchange rates found."}
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
        title={editing ? (t["admin.exchange_rates.edit"] ?? "Edit Exchange Rate") : (t["admin.exchange_rates.add"] ?? "Add Exchange Rate")}
        onClose={closeModal}
        onSubmit={handleSave}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.exchange_rates.col.currency"] ?? "Currency"}
          type="select"
          value={form.currencyId}
          onChange={(v) => setForm({ ...form, currencyId: Number(v) })}
          options={currencyOptions}
          required
        />
        <FormField
          label={t["admin.exchange_rates.col.rate"] ?? "Rate to EUR"}
          type="number"
          value={form.rateToEur}
          onChange={(v) => setForm({ ...form, rateToEur: Number(v) })}
          step="0.000001"
          required
        />
        <FormField
          label={t["admin.exchange_rates.col.valid_from"] ?? "Valid From"}
          type="date"
          value={form.validFrom}
          onChange={(v) => setForm({ ...form, validFrom: String(v) })}
          required
        />
        <FormField
          label={t["admin.exchange_rates.col.valid_to"] ?? "Valid To"}
          type="date"
          value={form.validTo}
          onChange={(v) => setForm({ ...form, validTo: String(v) })}
          required
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleteTarget}
        title={t["admin.confirm.delete_title"] ?? "Delete Exchange Rate"}
        message={t["admin.confirm.delete_message"] ?? "Are you sure you want to delete this exchange rate?"}
        onConfirm={handleDelete}
        onCancel={() => setDeleteTarget(null)}
        loading={pending}
        confirmLabel={t["admin.action.delete"] ?? "Delete"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      />
    </>
  );
}
