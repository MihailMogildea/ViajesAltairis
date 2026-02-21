"use client";

import { useState } from "react";
import Link from "next/link";
import { createUser, deleteUser, toggleUserEnabled } from "./actions";
import { DataTable, type Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { StatusBadge } from "@/components/status-badge";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import type { UserDto, UserTypeDto } from "@/types/user";
import type { LanguageDto } from "@/types/system";
import type { AccessLevel } from "@/lib/permissions";

interface FormState {
  userTypeId: number;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phone: string;
  taxId: string;
  address: string;
  city: string;
  postalCode: string;
  country: string;
  languageId: string;
  businessPartnerId: string;
  providerId: string;
  discount: number;
}

const emptyForm: FormState = {
  userTypeId: 0,
  email: "",
  password: "",
  firstName: "",
  lastName: "",
  phone: "",
  taxId: "",
  address: "",
  city: "",
  postalCode: "",
  country: "",
  languageId: "",
  businessPartnerId: "",
  providerId: "",
  discount: 0,
};

export function UsersTable({
  users: initial,
  userTypes,
  languages,
  utNames,
  t,
  access,
}: {
  users: UserDto[];
  userTypes: UserTypeDto[];
  languages: LanguageDto[];
  utNames: Record<number, string>;
  t: Record<string, string>;
  access: AccessLevel | null;
}) {
  const [items, setItems] = useState(initial);
  const [modalOpen, setModalOpen] = useState(false);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [deleting, setDeleting] = useState<UserDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const isFull = access === "full";

  const userTypeMap = Object.fromEntries(userTypes.map((ut) => [ut.id, utNames[ut.id] ?? ut.name]));

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
        userTypeId: form.userTypeId,
        email: form.email,
        password: form.password,
        firstName: form.firstName,
        lastName: form.lastName,
        phone: form.phone || null,
        taxId: form.taxId || null,
        address: form.address || null,
        city: form.city || null,
        postalCode: form.postalCode || null,
        country: form.country || null,
        languageId: form.languageId ? Number(form.languageId) : null,
        businessPartnerId: form.businessPartnerId ? Number(form.businessPartnerId) : null,
        providerId: form.providerId ? Number(form.providerId) : null,
        discount: form.discount,
      };
      const created = await createUser(payload);
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
      await deleteUser(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      setMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setDeleting(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  async function handleToggleEnabled(item: UserDto) {
    setPending(true);
    setMessage(null);
    try {
      const newEnabled = !item.enabled;
      await toggleUserEnabled(item.id, newEnabled);
      setItems((prev) =>
        prev.map((i) =>
          i.id === item.id ? { ...i, enabled: newEnabled } : i
        )
      );
      const label = newEnabled
        ? t["admin.label.enabled"] ?? "Enabled"
        : t["admin.label.disabled"] ?? "Disabled";
      setMessage(`"${item.email}" ${label.toLowerCase()}.`);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Toggle failed");
    } finally {
      setPending(false);
    }
  }

  const columns: Column<UserDto>[] = [
    {
      key: "email",
      header: t["admin.field.email"] ?? "Email",
      render: (item) => (
        <Link href={`/users/${item.id}`} className="text-blue-600 hover:underline">
          {item.email}
        </Link>
      ),
    },
    {
      key: "name",
      header: t["admin.field.name"] ?? "Name",
      render: (item) => <span>{item.firstName} {item.lastName}</span>,
    },
    {
      key: "userTypeId",
      header: t["admin.field.userType"] ?? "Type",
      render: (item) => <span>{userTypeMap[item.userTypeId] ?? item.userTypeId}</span>,
    },
    {
      key: "discount",
      header: t["admin.field.discount"] ?? "Discount",
      render: (item) => <span>{item.discount}%</span>,
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
        emptyMessage={t["admin.users.empty"] ?? "No users found."}
        actions={
          isFull
            ? (item) => (
                <>
                  <Link
                    href={`/users/${item.id}`}
                    className="rounded border border-gray-300 px-3 py-1 text-xs font-medium text-gray-700 hover:bg-gray-50"
                  >
                    {t["admin.action.edit"] ?? "Edit"}
                  </Link>
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
        title={t["admin.users.create"] ?? "Create User"}
        onClose={closeModal}
        onSubmit={handleSubmit}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.field.userType"] ?? "User Type"}
          type="select"
          value={form.userTypeId}
          onChange={(v) => setForm((f) => ({ ...f, userTypeId: Number(v) }))}
          options={userTypes.map((ut) => ({ value: ut.id, label: utNames[ut.id] ?? ut.name }))}
          required
        />
        <FormField
          label={t["admin.field.email"] ?? "Email"}
          type="email"
          value={form.email}
          onChange={(v) => setForm((f) => ({ ...f, email: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.password"] ?? "Password"}
          type="password"
          value={form.password}
          onChange={(v) => setForm((f) => ({ ...f, password: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.firstName"] ?? "First Name"}
          value={form.firstName}
          onChange={(v) => setForm((f) => ({ ...f, firstName: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.lastName"] ?? "Last Name"}
          value={form.lastName}
          onChange={(v) => setForm((f) => ({ ...f, lastName: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.phone"] ?? "Phone"}
          value={form.phone}
          onChange={(v) => setForm((f) => ({ ...f, phone: String(v) }))}
        />
        <FormField
          label={t["admin.field.taxId"] ?? "Tax ID"}
          value={form.taxId}
          onChange={(v) => setForm((f) => ({ ...f, taxId: String(v) }))}
        />
        <FormField
          label={t["admin.field.address"] ?? "Address"}
          value={form.address}
          onChange={(v) => setForm((f) => ({ ...f, address: String(v) }))}
        />
        <FormField
          label={t["admin.field.city"] ?? "City"}
          value={form.city}
          onChange={(v) => setForm((f) => ({ ...f, city: String(v) }))}
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
        />
        <FormField
          label={t["admin.field.language"] ?? "Language"}
          type="select"
          value={form.languageId}
          onChange={(v) => setForm((f) => ({ ...f, languageId: String(v) }))}
          options={languages.map((l) => ({ value: l.id, label: l.name }))}
        />
        <FormField
          label={t["admin.field.businessPartnerId"] ?? "Business Partner ID"}
          type="number"
          value={form.businessPartnerId}
          onChange={(v) => setForm((f) => ({ ...f, businessPartnerId: String(v) }))}
        />
        <FormField
          label={t["admin.field.providerId"] ?? "Provider ID"}
          type="number"
          value={form.providerId}
          onChange={(v) => setForm((f) => ({ ...f, providerId: String(v) }))}
        />
        <FormField
          label={t["admin.field.discount"] ?? "Discount (%)"}
          type="number"
          step="0.01"
          value={form.discount}
          onChange={(v) => setForm((f) => ({ ...f, discount: Number(v) }))}
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleting}
        title={t["admin.confirm.delete.title"] ?? "Confirm Delete"}
        message={
          (t["admin.confirm.delete.message"] ?? "Are you sure you want to delete \"{name}\"?").replace(
            "{name}",
            deleting?.email ?? ""
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
