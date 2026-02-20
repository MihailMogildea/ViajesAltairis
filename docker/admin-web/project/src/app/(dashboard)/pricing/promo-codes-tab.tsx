"use client";

import { useState } from "react";
import {
  createPromoCode,
  updatePromoCode,
  deletePromoCode,
  togglePromoCodeEnabled,
} from "./actions";
import { DataTable, type Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { StatusBadge } from "@/components/status-badge";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import type { PromoCodeDto } from "@/types/pricing";
import type { CurrencyDto } from "@/types/system";
import type { AccessLevel } from "@/lib/permissions";

interface FormState {
  code: string;
  discountPercentage: string;
  discountAmount: string;
  currencyId: string;
  validFrom: string;
  validTo: string;
  maxUses: string;
}

const emptyForm: FormState = {
  code: "",
  discountPercentage: "",
  discountAmount: "",
  currencyId: "",
  validFrom: "",
  validTo: "",
  maxUses: "",
};

export function PromoCodesTab({
  promoCodes: initial,
  currencies,
  access,
  t,
}: {
  promoCodes: PromoCodeDto[];
  currencies: CurrencyDto[];
  access: AccessLevel | null;
  t: Record<string, string>;
}) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<PromoCodeDto | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [deleting, setDeleting] = useState<PromoCodeDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const isFull = access === "full";

  const currencyMap = new Map(currencies.map((c) => [c.id, c.isoCode]));
  const currencyOptions = currencies.map((c) => ({
    value: c.id,
    label: `${c.isoCode} - ${c.name}`,
  }));

  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    setModalOpen(true);
  }

  function openEdit(item: PromoCodeDto) {
    setEditing(item);
    setForm({
      code: item.code,
      discountPercentage: item.discountPercentage != null ? String(item.discountPercentage) : "",
      discountAmount: item.discountAmount != null ? String(item.discountAmount) : "",
      currencyId: item.currencyId != null ? String(item.currencyId) : "",
      validFrom: item.validFrom ? item.validFrom.slice(0, 10) : "",
      validTo: item.validTo ? item.validTo.slice(0, 10) : "",
      maxUses: item.maxUses != null ? String(item.maxUses) : "",
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
        code: form.code,
        discountPercentage: form.discountPercentage ? Number(form.discountPercentage) : null,
        discountAmount: form.discountAmount ? Number(form.discountAmount) : null,
        currencyId: form.currencyId ? Number(form.currencyId) : null,
        validFrom: form.validFrom,
        validTo: form.validTo,
        maxUses: form.maxUses ? Number(form.maxUses) : null,
      };
      if (editing) {
        const updated = await updatePromoCode(editing.id, payload);
        setItems((prev) =>
          prev.map((i) => (i.id === editing.id ? { ...i, ...updated } : i))
        );
        setMessage(t["admin.message.updated"] ?? "Updated successfully.");
      } else {
        const created = await createPromoCode(payload);
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
      await deletePromoCode(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      setMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setDeleting(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  async function handleToggle(item: PromoCodeDto) {
    setPending(true);
    setMessage(null);
    try {
      await togglePromoCodeEnabled(item.id, !item.enabled);
      setItems((prev) =>
        prev.map((i) =>
          i.id === item.id ? { ...i, enabled: !item.enabled } : i
        )
      );
      const label = !item.enabled
        ? t["admin.label.enabled"] ?? "Enabled"
        : t["admin.label.disabled"] ?? "Disabled";
      setMessage(`"${item.code}" ${label.toLowerCase()}.`);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Toggle failed");
    } finally {
      setPending(false);
    }
  }

  function formatDiscount(item: PromoCodeDto): string {
    if (item.discountPercentage != null) return `${item.discountPercentage}%`;
    if (item.discountAmount != null) {
      const currency = item.currencyId ? currencyMap.get(item.currencyId) ?? "" : "";
      return `${item.discountAmount} ${currency}`.trim();
    }
    return "-";
  }

  function formatDate(dateStr: string | null): string {
    if (!dateStr) return "-";
    return new Date(dateStr).toLocaleDateString();
  }

  const columns: Column<PromoCodeDto>[] = [
    {
      key: "code",
      header: t["admin.field.code"] ?? "Code",
      render: (item) => (
        <span className="font-mono text-xs font-medium">{item.code}</span>
      ),
    },
    {
      key: "discount",
      header: t["admin.field.discount"] ?? "Discount",
      render: formatDiscount,
    },
    {
      key: "validFrom",
      header: t["admin.field.validFrom"] ?? "Valid From",
      render: (item) => (
        <span className="text-xs text-gray-500">{formatDate(item.validFrom)}</span>
      ),
    },
    {
      key: "validTo",
      header: t["admin.field.validTo"] ?? "Valid To",
      render: (item) => (
        <span className="text-xs text-gray-500">{formatDate(item.validTo)}</span>
      ),
    },
    {
      key: "uses",
      header: t["admin.field.uses"] ?? "Uses",
      render: (item) => (
        <span className="text-xs text-gray-500">
          {item.currentUses}{item.maxUses != null ? ` / ${item.maxUses}` : ""}
        </span>
      ),
    },
    {
      key: "enabled",
      header: t["admin.field.status"] ?? "Status",
      render: (item) => (
        <StatusBadge
          variant={item.enabled ? "enabled" : "disabled"}
          onClick={isFull ? () => handleToggle(item) : undefined}
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
        emptyMessage={t["admin.pricing.promo_codes.empty"] ?? "No promo codes found."}
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
            ? t["admin.pricing.promo_codes.edit"] ?? "Edit Promo Code"
            : t["admin.pricing.promo_codes.create"] ?? "Create Promo Code"
        }
        onClose={closeModal}
        onSubmit={handleSubmit}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.field.code"] ?? "Code"}
          value={form.code}
          onChange={(v) => setForm((f) => ({ ...f, code: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.discountPercentage"] ?? "Discount Percentage (%)"}
          type="number"
          value={form.discountPercentage ? Number(form.discountPercentage) : ""}
          onChange={(v) =>
            setForm((f) => ({ ...f, discountPercentage: v ? String(v) : "" }))
          }
          step="0.01"
        />
        <FormField
          label={t["admin.field.discountAmount"] ?? "Discount Amount"}
          type="number"
          value={form.discountAmount ? Number(form.discountAmount) : ""}
          onChange={(v) =>
            setForm((f) => ({ ...f, discountAmount: v ? String(v) : "" }))
          }
          step="0.01"
        />
        <FormField
          label={t["admin.field.currency"] ?? "Currency"}
          type="select"
          value={form.currencyId}
          onChange={(v) => setForm((f) => ({ ...f, currencyId: String(v) }))}
          options={currencyOptions}
        />
        <FormField
          label={t["admin.field.validFrom"] ?? "Valid From"}
          type="date"
          value={form.validFrom}
          onChange={(v) => setForm((f) => ({ ...f, validFrom: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.validTo"] ?? "Valid To"}
          type="date"
          value={form.validTo}
          onChange={(v) => setForm((f) => ({ ...f, validTo: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.maxUses"] ?? "Max Uses"}
          type="number"
          value={form.maxUses ? Number(form.maxUses) : ""}
          onChange={(v) =>
            setForm((f) => ({ ...f, maxUses: v ? String(v) : "" }))
          }
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleting}
        title={t["admin.confirm.delete.title"] ?? "Confirm Delete"}
        message={
          (t["admin.confirm.delete.message"] ?? "Are you sure you want to delete \"{name}\"?").replace(
            "{name}",
            deleting?.code ?? ""
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
