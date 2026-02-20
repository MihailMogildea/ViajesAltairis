"use client";

import { useState } from "react";
import {
  createBusinessPartner,
  updateBusinessPartner,
  deleteBusinessPartner,
  toggleBusinessPartnerEnabled,
} from "./actions";
import { DataTable, type Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { StatusBadge } from "@/components/status-badge";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import type { BusinessPartnerDto } from "@/types/business-partner";
import type { AccessLevel } from "@/lib/permissions";

interface FormState {
  companyName: string;
  taxId: string;
  discount: number;
  address: string;
  city: string;
  postalCode: string;
  country: string;
  contactEmail: string;
  contactPhone: string;
}

const emptyForm: FormState = {
  companyName: "",
  taxId: "",
  discount: 0,
  address: "",
  city: "",
  postalCode: "",
  country: "",
  contactEmail: "",
  contactPhone: "",
};

export function BusinessPartnersTable({
  partners: initial,
  t,
  access,
}: {
  partners: BusinessPartnerDto[];
  t: Record<string, string>;
  access: AccessLevel | null;
}) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<BusinessPartnerDto | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [deleting, setDeleting] = useState<BusinessPartnerDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const isFull = access === "full";

  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    setModalOpen(true);
  }

  function openEdit(item: BusinessPartnerDto) {
    setEditing(item);
    setForm({
      companyName: item.companyName,
      taxId: item.taxId ?? "",
      discount: item.discount,
      address: item.address,
      city: item.city,
      postalCode: item.postalCode ?? "",
      country: item.country,
      contactEmail: item.contactEmail,
      contactPhone: item.contactPhone ?? "",
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
        companyName: form.companyName,
        taxId: form.taxId || null,
        discount: form.discount,
        address: form.address,
        city: form.city,
        postalCode: form.postalCode || null,
        country: form.country,
        contactEmail: form.contactEmail,
        contactPhone: form.contactPhone || null,
      };
      if (editing) {
        const updated = await updateBusinessPartner(editing.id, payload);
        setItems((prev) =>
          prev.map((i) => (i.id === editing.id ? { ...i, ...updated } : i))
        );
        setMessage(t["admin.message.updated"] ?? "Updated successfully.");
      } else {
        const created = await createBusinessPartner(payload);
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
      await deleteBusinessPartner(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      setMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setDeleting(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  async function handleToggleEnabled(item: BusinessPartnerDto) {
    setPending(true);
    setMessage(null);
    try {
      const newEnabled = !item.enabled;
      await toggleBusinessPartnerEnabled(item.id, newEnabled);
      setItems((prev) =>
        prev.map((i) =>
          i.id === item.id ? { ...i, enabled: newEnabled } : i
        )
      );
      const label = newEnabled
        ? t["admin.label.enabled"] ?? "Enabled"
        : t["admin.label.disabled"] ?? "Disabled";
      setMessage(`"${item.companyName}" ${label.toLowerCase()}.`);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Toggle failed");
    } finally {
      setPending(false);
    }
  }

  const columns: Column<BusinessPartnerDto>[] = [
    { key: "companyName", header: t["admin.field.companyName"] ?? "Company Name" },
    { key: "contactEmail", header: t["admin.field.contactEmail"] ?? "Email" },
    {
      key: "discount",
      header: t["admin.field.discount"] ?? "Discount",
      render: (item) => <span>{item.discount}%</span>,
    },
    { key: "city", header: t["admin.field.city"] ?? "City" },
    { key: "country", header: t["admin.field.country"] ?? "Country" },
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
        emptyMessage={t["admin.business_partners.empty"] ?? "No business partners found."}
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
            ? t["admin.business_partners.edit"] ?? "Edit Business Partner"
            : t["admin.business_partners.create"] ?? "Create Business Partner"
        }
        onClose={closeModal}
        onSubmit={handleSubmit}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.field.companyName"] ?? "Company Name"}
          value={form.companyName}
          onChange={(v) => setForm((f) => ({ ...f, companyName: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.taxId"] ?? "Tax ID"}
          value={form.taxId}
          onChange={(v) => setForm((f) => ({ ...f, taxId: String(v) }))}
        />
        <FormField
          label={t["admin.field.discount"] ?? "Discount (%)"}
          type="number"
          step="0.01"
          value={form.discount}
          onChange={(v) => setForm((f) => ({ ...f, discount: Number(v) }))}
        />
        <FormField
          label={t["admin.field.address"] ?? "Address"}
          value={form.address}
          onChange={(v) => setForm((f) => ({ ...f, address: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.city"] ?? "City"}
          value={form.city}
          onChange={(v) => setForm((f) => ({ ...f, city: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.postalCode"] ?? "Postal Code"}
          value={form.postalCode}
          onChange={(v) => setForm((f) => ({ ...f, postalCode: String(v) }))}
        />
        <FormField
          label={t["admin.field.country"] ?? "Country"}
          value={form.country}
          onChange={(v) => setForm((f) => ({ ...f, country: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.contactEmail"] ?? "Contact Email"}
          type="email"
          value={form.contactEmail}
          onChange={(v) => setForm((f) => ({ ...f, contactEmail: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.contactPhone"] ?? "Contact Phone"}
          value={form.contactPhone}
          onChange={(v) => setForm((f) => ({ ...f, contactPhone: String(v) }))}
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleting}
        title={t["admin.confirm.delete.title"] ?? "Confirm Delete"}
        message={
          (t["admin.confirm.delete.message"] ?? "Are you sure you want to delete \"{name}\"?").replace(
            "{name}",
            deleting?.companyName ?? ""
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
