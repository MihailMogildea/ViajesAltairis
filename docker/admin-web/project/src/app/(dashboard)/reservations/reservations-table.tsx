"use client";

import { useState } from "react";
import Link from "next/link";
import { DataTable, type Column } from "@/components/data-table";
import { StatusBadge } from "@/components/status-badge";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ToastMessage } from "@/components/toast-message";
import { createReservation } from "./actions";
import type {
  ReservationAdminDto,
  ReservationStatusDto,
  CreateReservationRequest,
} from "@/types/reservation";

type Variant = "enabled" | "disabled" | "success" | "warning" | "danger" | "info";

function statusVariant(statusName: string): Variant {
  const lower = statusName.toLowerCase();
  if (lower.includes("draft")) return "info";
  if (lower.includes("pending")) return "warning";
  if (lower.includes("confirmed")) return "success";
  if (lower.includes("checked")) return "success";
  if (lower.includes("completed")) return "enabled";
  if (lower.includes("cancelled")) return "danger";
  return "disabled";
}

const EMPTY_FORM: CreateReservationRequest = {
  currencyCode: "EUR",
  promoCode: null,
  ownerUserId: null,
  ownerFirstName: null,
  ownerLastName: null,
  ownerEmail: null,
  ownerPhone: null,
  ownerTaxId: null,
  ownerAddress: null,
  ownerCity: null,
  ownerPostalCode: null,
  ownerCountry: null,
};

export function ReservationsTable({
  reservations: initial,
  statuses,
  access,
  t,
}: {
  reservations: ReservationAdminDto[];
  statuses: ReservationStatusDto[];
  access: string | null;
  t: Record<string, string>;
}) {
  const [items, setItems] = useState(initial);
  const [filterStatusId, setFilterStatusId] = useState<number | null>(null);
  const [showCreate, setShowCreate] = useState(false);
  const [form, setForm] = useState<CreateReservationRequest>({ ...EMPTY_FORM });
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const filtered = filterStatusId
    ? items.filter((r) => r.statusId === filterStatusId)
    : items;

  const columns: Column<ReservationAdminDto>[] = [
    {
      key: "reservationCode",
      header: t["admin.reservations.col.code"] ?? "Code",
      render: (r) => (
        <Link
          href={`/reservations/${r.id}`}
          className="font-medium text-blue-600 hover:underline"
        >
          {r.reservationCode}
        </Link>
      ),
    },
    {
      key: "statusName",
      header: t["admin.reservations.col.status"] ?? "Status",
      render: (r) => (
        <StatusBadge variant={statusVariant(r.statusName)}>
          {r.statusName}
        </StatusBadge>
      ),
    },
    {
      key: "owner",
      header: t["admin.reservations.col.owner"] ?? "Owner",
      render: (r) => `${r.ownerFirstName} ${r.ownerLastName}`,
    },
    {
      key: "totalPrice",
      header: t["admin.reservations.col.total"] ?? "Total",
      className: "text-right",
      render: (r) => `${r.totalPrice.toFixed(2)} ${r.currencyCode}`,
    },
    {
      key: "lineCount",
      header: t["admin.reservations.col.lines"] ?? "Lines",
      className: "text-right",
    },
    {
      key: "createdAt",
      header: t["admin.reservations.col.created"] ?? "Created",
      render: (r) => new Date(r.createdAt).toLocaleDateString(),
    },
  ];

  async function handleCreate() {
    setPending(true);
    setMessage(null);
    try {
      const created = await createReservation({
        ...form,
        promoCode: form.promoCode || null,
        ownerUserId: form.ownerUserId || null,
        ownerFirstName: form.ownerFirstName || null,
        ownerLastName: form.ownerLastName || null,
        ownerEmail: form.ownerEmail || null,
        ownerPhone: form.ownerPhone || null,
        ownerTaxId: form.ownerTaxId || null,
        ownerAddress: form.ownerAddress || null,
        ownerCity: form.ownerCity || null,
        ownerPostalCode: form.ownerPostalCode || null,
        ownerCountry: form.ownerCountry || null,
      });
      setItems((prev) => [created, ...prev]);
      setShowCreate(false);
      setForm({ ...EMPTY_FORM });
      setMessage(t["admin.reservations.created"] ?? "Reservation created.");
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Create failed");
    } finally {
      setPending(false);
    }
  }

  return (
    <>
      <ToastMessage message={message} onDismiss={() => setMessage(null)} />

      {/* Status filter */}
      <div className="mb-4 flex flex-wrap gap-2">
        <button
          onClick={() => setFilterStatusId(null)}
          className={`rounded-full px-3 py-1 text-xs font-medium ${
            filterStatusId === null
              ? "bg-blue-600 text-white"
              : "bg-gray-100 text-gray-600 hover:bg-gray-200"
          }`}
        >
          {t["admin.label.all"] ?? "All"}
        </button>
        {statuses.map((s) => (
          <button
            key={s.id}
            onClick={() => setFilterStatusId(s.id)}
            className={`rounded-full px-3 py-1 text-xs font-medium ${
              filterStatusId === s.id
                ? "bg-blue-600 text-white"
                : "bg-gray-100 text-gray-600 hover:bg-gray-200"
            }`}
          >
            {s.name}
          </button>
        ))}
      </div>

      {/* Create button */}
      {(access === "full" || access === "own") && (
        <div className="mb-4 flex justify-end">
          <button
            onClick={() => setShowCreate(true)}
            className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
          >
            {t["admin.reservations.create"] ?? "Create Reservation"}
          </button>
        </div>
      )}

      <DataTable
        columns={columns}
        data={filtered}
        keyField="id"
        emptyMessage={t["admin.reservations.empty"] ?? "No reservations found."}
      />

      {/* Create Modal */}
      <FormModal
        open={showCreate}
        title={t["admin.reservations.create"] ?? "Create Reservation"}
        onClose={() => {
          setShowCreate(false);
          setForm({ ...EMPTY_FORM });
        }}
        onSubmit={handleCreate}
        loading={pending}
        saveLabel={t["admin.action.create"] ?? "Create"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.reservations.field.currency_code"] ?? "Currency Code"}
          value={form.currencyCode}
          onChange={(v) => setForm({ ...form, currencyCode: String(v) })}
        />
        <FormField
          label={t["admin.reservations.field.promo_code"] ?? "Promo Code"}
          value={form.promoCode ?? ""}
          onChange={(v) => setForm({ ...form, promoCode: String(v) || null })}
        />
        <FormField
          label={t["admin.reservations.field.owner_user_id"] ?? "Owner User ID"}
          type="number"
          value={form.ownerUserId ?? 0}
          onChange={(v) => setForm({ ...form, ownerUserId: Number(v) || null })}
        />
        <FormField
          label={t["admin.reservations.field.first_name"] ?? "First Name"}
          value={form.ownerFirstName ?? ""}
          onChange={(v) => setForm({ ...form, ownerFirstName: String(v) || null })}
        />
        <FormField
          label={t["admin.reservations.field.last_name"] ?? "Last Name"}
          value={form.ownerLastName ?? ""}
          onChange={(v) => setForm({ ...form, ownerLastName: String(v) || null })}
        />
        <FormField
          label={t["admin.reservations.field.email"] ?? "Email"}
          type="email"
          value={form.ownerEmail ?? ""}
          onChange={(v) => setForm({ ...form, ownerEmail: String(v) || null })}
        />
        <FormField
          label={t["admin.reservations.field.phone"] ?? "Phone"}
          value={form.ownerPhone ?? ""}
          onChange={(v) => setForm({ ...form, ownerPhone: String(v) || null })}
        />
        <FormField
          label={t["admin.reservations.field.tax_id"] ?? "Tax ID"}
          value={form.ownerTaxId ?? ""}
          onChange={(v) => setForm({ ...form, ownerTaxId: String(v) || null })}
        />
        <FormField
          label={t["admin.reservations.field.address"] ?? "Address"}
          value={form.ownerAddress ?? ""}
          onChange={(v) => setForm({ ...form, ownerAddress: String(v) || null })}
        />
        <FormField
          label={t["admin.reservations.field.city"] ?? "City"}
          value={form.ownerCity ?? ""}
          onChange={(v) => setForm({ ...form, ownerCity: String(v) || null })}
        />
        <FormField
          label={t["admin.reservations.field.postal_code"] ?? "Postal Code"}
          value={form.ownerPostalCode ?? ""}
          onChange={(v) => setForm({ ...form, ownerPostalCode: String(v) || null })}
        />
        <FormField
          label={t["admin.reservations.field.country"] ?? "Country"}
          value={form.ownerCountry ?? ""}
          onChange={(v) => setForm({ ...form, ownerCountry: String(v) || null })}
        />
      </FormModal>
    </>
  );
}
