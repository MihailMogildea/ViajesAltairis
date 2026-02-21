"use client";

import { useState, useMemo } from "react";
import Link from "next/link";
import { createHotel, deleteHotel, toggleHotelEnabled } from "./actions";
import { DataTable, type Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { StatusBadge } from "@/components/status-badge";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import type { HotelDto } from "@/types/hotel";
import type { CityDto } from "@/types/system";
import type { AccessLevel } from "@/lib/permissions";

interface FormState {
  cityId: string;
  name: string;
  stars: number;
  address: string;
  email: string;
  phone: string;
  checkInTime: string;
  checkOutTime: string;
  latitude: string;
  longitude: string;
  margin: number;
}

const emptyForm: FormState = {
  cityId: "",
  name: "",
  stars: 3,
  address: "",
  email: "",
  phone: "",
  checkInTime: "14:00",
  checkOutTime: "11:00",
  latitude: "",
  longitude: "",
  margin: 0,
};

export function HotelsTable({
  hotels: initial,
  cities,
  t,
  access,
}: {
  hotels: HotelDto[];
  cities: CityDto[];
  t: Record<string, string>;
  access: AccessLevel | null;
}) {
  const [items, setItems] = useState(initial);
  const [modalOpen, setModalOpen] = useState(false);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [deleting, setDeleting] = useState<HotelDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const [filterStars, setFilterStars] = useState<number | null>(null);
  const [filterEnabled, setFilterEnabled] = useState<boolean | null>(null);
  const [searchText, setSearchText] = useState("");

  const isFull = access === "full";

  const cityMap = new Map(cities.map((c) => [c.id, c.name]));
  const cityOptions = cities
    .filter((c) => c.enabled)
    .map((c) => ({ value: c.id, label: c.name }));

  const filtered = useMemo(() => {
    let result = items;
    if (filterStars !== null) {
      result = result.filter((h) => h.stars === filterStars);
    }
    if (filterEnabled !== null) {
      result = result.filter((h) => h.enabled === filterEnabled);
    }
    if (searchText.trim()) {
      const q = searchText.trim().toLowerCase();
      result = result.filter(
        (h) =>
          h.name.toLowerCase().includes(q) ||
          (cityMap.get(h.cityId) ?? "").toLowerCase().includes(q)
      );
    }
    return result;
  }, [items, filterStars, filterEnabled, searchText, cityMap]);

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
        cityId: Number(form.cityId),
        name: form.name,
        stars: form.stars,
        address: form.address,
        email: form.email || null,
        phone: form.phone || null,
        checkInTime: form.checkInTime,
        checkOutTime: form.checkOutTime,
        latitude: form.latitude ? Number(form.latitude) : null,
        longitude: form.longitude ? Number(form.longitude) : null,
        margin: form.margin,
      };
      const created = await createHotel(payload);
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
      await deleteHotel(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      setMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setDeleting(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  async function handleToggleEnabled(item: HotelDto) {
    setPending(true);
    setMessage(null);
    try {
      const newEnabled = !item.enabled;
      await toggleHotelEnabled(item.id, newEnabled);
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

  const pillClass = (active: boolean) =>
    `rounded-full px-3 py-1 text-xs font-medium ${
      active
        ? "bg-blue-600 text-white"
        : "bg-gray-100 text-gray-600 hover:bg-gray-200"
    }`;

  const columns: Column<HotelDto>[] = [
    {
      key: "name",
      header: t["admin.field.name"] ?? "Name",
      sortable: true,
      render: (item) => (
        <Link
          href={`/hotels/${item.id}`}
          className="font-medium text-blue-600 hover:underline"
        >
          {item.name}
        </Link>
      ),
    },
    {
      key: "stars",
      header: t["admin.field.stars"] ?? "Stars",
      sortable: true,
      render: (item) => (
        <span className="text-amber-500">
          {"★".repeat(item.stars)}
        </span>
      ),
    },
    {
      key: "cityId",
      header: t["admin.field.city"] ?? "City",
      render: (item) => <span>{cityMap.get(item.cityId) ?? item.cityId}</span>,
    },
    {
      key: "margin",
      header: t["admin.field.margin"] ?? "Margin",
      sortable: true,
      render: (item) => <span>{item.margin}%</span>,
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

      {/* Filters */}
      <div className="mb-4 space-y-3">
        {/* Search */}
        <input
          type="text"
          placeholder={t["admin.field.search"] ?? "Search by name or city..."}
          value={searchText}
          onChange={(e) => setSearchText(e.target.value)}
          className="w-full rounded-lg border border-gray-200 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none sm:w-64"
        />

        <div className="flex flex-wrap items-center gap-4">
          {/* Stars filter */}
          <div className="flex flex-wrap gap-2">
            <button onClick={() => setFilterStars(null)} className={pillClass(filterStars === null)}>
              {t["admin.label.all"] ?? "All"}
            </button>
            {[3, 4, 5].map((s) => (
              <button key={s} onClick={() => setFilterStars(s)} className={pillClass(filterStars === s)}>
                {s}★
              </button>
            ))}
          </div>

          {/* Enabled filter */}
          <div className="flex flex-wrap gap-2">
            <button onClick={() => setFilterEnabled(null)} className={pillClass(filterEnabled === null)}>
              {t["admin.label.all"] ?? "All"}
            </button>
            <button onClick={() => setFilterEnabled(true)} className={pillClass(filterEnabled === true)}>
              {t["admin.label.enabled"] ?? "Enabled"}
            </button>
            <button onClick={() => setFilterEnabled(false)} className={pillClass(filterEnabled === false)}>
              {t["admin.label.disabled"] ?? "Disabled"}
            </button>
          </div>
        </div>
      </div>

      <DataTable
        columns={columns}
        data={filtered}
        keyField="id"
        emptyMessage={t["admin.hotels.empty"] ?? "No hotels found."}
        actions={
          isFull
            ? (item) => (
                <>
                  <Link
                    href={`/hotels/${item.id}`}
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
        title={t["admin.hotels.create"] ?? "Create Hotel"}
        onClose={closeModal}
        onSubmit={handleSubmit}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.field.city"] ?? "City"}
          type="select"
          value={form.cityId}
          onChange={(v) => setForm((f) => ({ ...f, cityId: String(v) }))}
          options={cityOptions}
          required
        />
        <FormField
          label={t["admin.field.name"] ?? "Name"}
          value={form.name}
          onChange={(v) => setForm((f) => ({ ...f, name: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.stars"] ?? "Stars"}
          type="number"
          value={form.stars}
          onChange={(v) => setForm((f) => ({ ...f, stars: Number(v) }))}
          step="1"
        />
        <FormField
          label={t["admin.field.address"] ?? "Address"}
          value={form.address}
          onChange={(v) => setForm((f) => ({ ...f, address: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.email"] ?? "Email"}
          type="email"
          value={form.email}
          onChange={(v) => setForm((f) => ({ ...f, email: String(v) }))}
        />
        <FormField
          label={t["admin.field.phone"] ?? "Phone"}
          value={form.phone}
          onChange={(v) => setForm((f) => ({ ...f, phone: String(v) }))}
        />
        <FormField
          label={t["admin.field.checkInTime"] ?? "Check-in Time"}
          type="time"
          value={form.checkInTime}
          onChange={(v) => setForm((f) => ({ ...f, checkInTime: String(v) }))}
        />
        <FormField
          label={t["admin.field.checkOutTime"] ?? "Check-out Time"}
          type="time"
          value={form.checkOutTime}
          onChange={(v) => setForm((f) => ({ ...f, checkOutTime: String(v) }))}
        />
        <FormField
          label={t["admin.field.latitude"] ?? "Latitude"}
          type="number"
          value={form.latitude}
          onChange={(v) => setForm((f) => ({ ...f, latitude: String(v) }))}
          step="0.000001"
        />
        <FormField
          label={t["admin.field.longitude"] ?? "Longitude"}
          type="number"
          value={form.longitude}
          onChange={(v) => setForm((f) => ({ ...f, longitude: String(v) }))}
          step="0.000001"
        />
        <FormField
          label={t["admin.field.margin"] ?? "Margin (%)"}
          type="number"
          value={form.margin}
          onChange={(v) => setForm((f) => ({ ...f, margin: Number(v) }))}
          step="0.01"
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
