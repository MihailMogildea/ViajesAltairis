"use client";

import { useState } from "react";
import {
  updateHotel,
  createHotelImage,
  deleteHotelImage,
  addHotelAmenity,
  removeHotelAmenity,
} from "./actions";
import { TabBar } from "@/components/tab-bar";
import { FormField } from "@/components/form-field";
import { FormModal } from "@/components/form-modal";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { ToastMessage } from "@/components/toast-message";
import type { HotelDto, HotelImageDto, HotelAmenityDto } from "@/types/hotel";
import type { AmenityDto, AmenityCategoryDto, CityDto } from "@/types/system";
import type { AccessLevel } from "@/lib/permissions";

interface Props {
  hotel: HotelDto;
  images: HotelImageDto[];
  hotelAmenities: HotelAmenityDto[];
  amenities: AmenityDto[];
  amenityCategories: AmenityCategoryDto[];
  cities: CityDto[];
  t: Record<string, string>;
  access: AccessLevel | null;
}

const TABS = [
  { key: "info", label: "Info" },
  { key: "images", label: "Images" },
  { key: "amenities", label: "Amenities" },
];

export function HotelDetail({
  hotel: initial,
  images: initialImages,
  hotelAmenities: initialHotelAmenities,
  amenities,
  amenityCategories,
  cities,
  t,
  access,
}: Props) {
  const [activeTab, setActiveTab] = useState("info");
  const [message, setMessage] = useState<string | null>(null);

  const tabs = TABS.map((tab) => ({
    ...tab,
    label: t[`admin.hotels.tab.${tab.key}`] ?? tab.label,
  }));

  return (
    <>
      <div className="mb-4">
        <h2 className="text-2xl font-semibold">{initial.name}</h2>
      </div>

      <ToastMessage message={message} onDismiss={() => setMessage(null)} />

      <TabBar tabs={tabs} active={activeTab} onChange={setActiveTab} />

      {activeTab === "info" && (
        <InfoTab
          hotel={initial}
          cities={cities}
          t={t}
          access={access}
          onMessage={setMessage}
        />
      )}
      {activeTab === "images" && (
        <ImagesTab
          hotel={initial}
          images={initialImages}
          t={t}
          access={access}
          onMessage={setMessage}
        />
      )}
      {activeTab === "amenities" && (
        <AmenitiesTab
          hotel={initial}
          hotelAmenities={initialHotelAmenities}
          amenities={amenities}
          amenityCategories={amenityCategories}
          t={t}
          access={access}
          onMessage={setMessage}
        />
      )}
    </>
  );
}

/* ─── Info Tab ─── */

function InfoTab({
  hotel,
  cities,
  t,
  access,
  onMessage,
}: {
  hotel: HotelDto;
  cities: CityDto[];
  t: Record<string, string>;
  access: AccessLevel | null;
  onMessage: (msg: string) => void;
}) {
  const isFull = access === "full";

  const [form, setForm] = useState({
    cityId: String(hotel.cityId),
    name: hotel.name,
    stars: hotel.stars,
    address: hotel.address,
    email: hotel.email ?? "",
    phone: hotel.phone ?? "",
    checkInTime: hotel.checkInTime,
    checkOutTime: hotel.checkOutTime,
    latitude: hotel.latitude != null ? String(hotel.latitude) : "",
    longitude: hotel.longitude != null ? String(hotel.longitude) : "",
    margin: hotel.margin,
  });
  const [pending, setPending] = useState(false);

  const cityOptions = cities
    .filter((c) => c.enabled)
    .map((c) => ({ value: c.id, label: c.name }));

  async function handleSave() {
    setPending(true);
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
      await updateHotel(hotel.id, payload);
      onMessage(t["admin.message.updated"] ?? "Updated successfully.");
    } catch (e) {
      onMessage(e instanceof Error ? e.message : "Update failed");
    } finally {
      setPending(false);
    }
  }

  return (
    <div className="max-w-2xl space-y-4">
      <FormField
        label={t["admin.field.city"] ?? "City"}
        type="select"
        value={form.cityId}
        onChange={(v) => setForm((f) => ({ ...f, cityId: String(v) }))}
        options={cityOptions}
        disabled={!isFull}
        required
      />
      <FormField
        label={t["admin.field.name"] ?? "Name"}
        value={form.name}
        onChange={(v) => setForm((f) => ({ ...f, name: String(v) }))}
        disabled={!isFull}
        required
      />
      <FormField
        label={t["admin.field.stars"] ?? "Stars"}
        type="number"
        value={form.stars}
        onChange={(v) => setForm((f) => ({ ...f, stars: Number(v) }))}
        disabled={!isFull}
        step="1"
      />
      <FormField
        label={t["admin.field.address"] ?? "Address"}
        value={form.address}
        onChange={(v) => setForm((f) => ({ ...f, address: String(v) }))}
        disabled={!isFull}
        required
      />
      <FormField
        label={t["admin.field.email"] ?? "Email"}
        type="email"
        value={form.email}
        onChange={(v) => setForm((f) => ({ ...f, email: String(v) }))}
        disabled={!isFull}
      />
      <FormField
        label={t["admin.field.phone"] ?? "Phone"}
        value={form.phone}
        onChange={(v) => setForm((f) => ({ ...f, phone: String(v) }))}
        disabled={!isFull}
      />
      <FormField
        label={t["admin.field.checkInTime"] ?? "Check-in Time"}
        type="time"
        value={form.checkInTime}
        onChange={(v) => setForm((f) => ({ ...f, checkInTime: String(v) }))}
        disabled={!isFull}
      />
      <FormField
        label={t["admin.field.checkOutTime"] ?? "Check-out Time"}
        type="time"
        value={form.checkOutTime}
        onChange={(v) => setForm((f) => ({ ...f, checkOutTime: String(v) }))}
        disabled={!isFull}
      />
      <FormField
        label={t["admin.field.latitude"] ?? "Latitude"}
        type="number"
        value={form.latitude}
        onChange={(v) => setForm((f) => ({ ...f, latitude: String(v) }))}
        disabled={!isFull}
        step="0.000001"
      />
      <FormField
        label={t["admin.field.longitude"] ?? "Longitude"}
        type="number"
        value={form.longitude}
        onChange={(v) => setForm((f) => ({ ...f, longitude: String(v) }))}
        disabled={!isFull}
        step="0.000001"
      />
      <FormField
        label={t["admin.field.margin"] ?? "Margin (%)"}
        type="number"
        value={form.margin}
        onChange={(v) => setForm((f) => ({ ...f, margin: Number(v) }))}
        disabled={!isFull}
        step="0.01"
      />

      {isFull && (
        <div className="pt-4">
          <button
            onClick={handleSave}
            disabled={pending}
            className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
          >
            {pending ? "..." : t["admin.action.save"] ?? "Save"}
          </button>
        </div>
      )}
    </div>
  );
}

/* ─── Images Tab ─── */

function ImagesTab({
  hotel,
  images: initial,
  t,
  access,
  onMessage,
}: {
  hotel: HotelDto;
  images: HotelImageDto[];
  t: Record<string, string>;
  access: AccessLevel | null;
  onMessage: (msg: string) => void;
}) {
  const isFull = access === "full";

  const [items, setItems] = useState(initial);
  const [modalOpen, setModalOpen] = useState(false);
  const [form, setForm] = useState({ url: "", altText: "", sortOrder: 0 });
  const [deleting, setDeleting] = useState<HotelImageDto | null>(null);
  const [pending, setPending] = useState(false);

  function openCreate() {
    setForm({ url: "", altText: "", sortOrder: 0 });
    setModalOpen(true);
  }

  async function handleSubmit() {
    setPending(true);
    try {
      const payload = {
        hotelId: hotel.id,
        url: form.url,
        altText: form.altText || null,
        sortOrder: form.sortOrder,
      };
      const created = await createHotelImage(payload);
      setItems((prev) => [...prev, created]);
      onMessage(t["admin.message.created"] ?? "Created successfully.");
      setModalOpen(false);
    } catch (e) {
      onMessage(e instanceof Error ? e.message : "Operation failed");
    } finally {
      setPending(false);
    }
  }

  async function handleDelete() {
    if (!deleting) return;
    setPending(true);
    try {
      await deleteHotelImage(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      onMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setDeleting(null);
    } catch (e) {
      onMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  return (
    <>
      {isFull && (
        <div className="mb-4">
          <button
            onClick={openCreate}
            className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
          >
            {t["admin.hotels.add_image"] ?? "Add Image"}
          </button>
        </div>
      )}

      {items.length === 0 ? (
        <div className="rounded-lg border border-gray-200 bg-white p-8 text-center text-sm text-gray-500">
          {t["admin.hotels.images_empty"] ?? "No images found."}
        </div>
      ) : (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {items
            .sort((a, b) => a.sortOrder - b.sortOrder)
            .map((img) => (
              <div
                key={img.id}
                className="overflow-hidden rounded-lg border border-gray-200 bg-white"
              >
                <div className="aspect-video bg-gray-100">
                  {/* eslint-disable-next-line @next/next/no-img-element */}
                  <img
                    src={img.url}
                    alt={img.altText ?? ""}
                    className="h-full w-full object-cover"
                  />
                </div>
                <div className="p-3">
                  <p className="truncate text-sm font-medium text-gray-900">
                    {img.altText || img.url}
                  </p>
                  <p className="text-xs text-gray-500">
                    {t["admin.field.sortOrder"] ?? "Sort Order"}: {img.sortOrder}
                  </p>
                  {isFull && (
                    <button
                      onClick={() => setDeleting(img)}
                      className="mt-2 rounded border border-red-300 px-3 py-1 text-xs font-medium text-red-700 hover:bg-red-50"
                    >
                      {t["admin.action.delete"] ?? "Delete"}
                    </button>
                  )}
                </div>
              </div>
            ))}
        </div>
      )}

      <FormModal
        open={modalOpen}
        title={t["admin.hotels.add_image"] ?? "Add Image"}
        onClose={() => setModalOpen(false)}
        onSubmit={handleSubmit}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.field.url"] ?? "URL"}
          value={form.url}
          onChange={(v) => setForm((f) => ({ ...f, url: String(v) }))}
          required
        />
        <FormField
          label={t["admin.field.altText"] ?? "Alt Text"}
          value={form.altText}
          onChange={(v) => setForm((f) => ({ ...f, altText: String(v) }))}
        />
        <FormField
          label={t["admin.field.sortOrder"] ?? "Sort Order"}
          type="number"
          value={form.sortOrder}
          onChange={(v) => setForm((f) => ({ ...f, sortOrder: Number(v) }))}
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleting}
        title={t["admin.confirm.delete.title"] ?? "Confirm Delete"}
        message={t["admin.hotels.confirm_delete_image"] ?? "Are you sure you want to delete this image?"}
        onConfirm={handleDelete}
        onCancel={() => setDeleting(null)}
        loading={pending}
        confirmLabel={t["admin.action.delete"] ?? "Delete"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      />
    </>
  );
}

/* ─── Amenities Tab ─── */

function AmenitiesTab({
  hotel,
  hotelAmenities: initial,
  amenities,
  amenityCategories,
  t,
  access,
  onMessage,
}: {
  hotel: HotelDto;
  hotelAmenities: HotelAmenityDto[];
  amenities: AmenityDto[];
  amenityCategories: AmenityCategoryDto[];
  t: Record<string, string>;
  access: AccessLevel | null;
  onMessage: (msg: string) => void;
}) {
  const isFull = access === "full";

  const [assigned, setAssigned] = useState(initial);
  const [pending, setPending] = useState<number | null>(null);

  const assignedSet = new Map(
    assigned.map((ha) => [ha.amenityId, ha])
  );

  const categoriesWithAmenities = amenityCategories
    .map((cat) => ({
      ...cat,
      items: amenities.filter((a) => a.categoryId === cat.id),
    }))
    .filter((cat) => cat.items.length > 0);

  async function handleToggle(amenity: AmenityDto) {
    if (!isFull) return;
    setPending(amenity.id);
    try {
      const existing = assignedSet.get(amenity.id);
      if (existing) {
        await removeHotelAmenity(existing.id);
        setAssigned((prev) => prev.filter((ha) => ha.id !== existing.id));
        onMessage(
          (t["admin.hotels.amenity_removed"] ?? "Removed \"{name}\".").replace(
            "{name}",
            amenity.name
          )
        );
      } else {
        const created = await addHotelAmenity(hotel.id, amenity.id);
        setAssigned((prev) => [...prev, created]);
        onMessage(
          (t["admin.hotels.amenity_added"] ?? "Added \"{name}\".").replace(
            "{name}",
            amenity.name
          )
        );
      }
    } catch (e) {
      onMessage(e instanceof Error ? e.message : "Operation failed");
    } finally {
      setPending(null);
    }
  }

  return (
    <div className="space-y-6">
      {categoriesWithAmenities.map((cat) => (
        <div key={cat.id}>
          <h3 className="mb-2 text-sm font-semibold uppercase text-gray-500">
            {cat.name}
          </h3>
          <div className="space-y-1">
            {cat.items.map((amenity) => {
              const isAssigned = assignedSet.has(amenity.id);
              const isLoading = pending === amenity.id;

              return (
                <label
                  key={amenity.id}
                  className={`flex items-center gap-2 rounded px-3 py-2 text-sm ${
                    isFull
                      ? "cursor-pointer hover:bg-gray-50"
                      : "cursor-default"
                  } ${isLoading ? "opacity-50" : ""}`}
                >
                  <input
                    type="checkbox"
                    checked={isAssigned}
                    onChange={() => handleToggle(amenity)}
                    disabled={!isFull || isLoading}
                    className="rounded border-gray-300"
                  />
                  <span className="text-gray-900">{amenity.name}</span>
                </label>
              );
            })}
          </div>
        </div>
      ))}

      {categoriesWithAmenities.length === 0 && (
        <div className="rounded-lg border border-gray-200 bg-white p-8 text-center text-sm text-gray-500">
          {t["admin.hotels.amenities_empty"] ?? "No amenities available."}
        </div>
      )}
    </div>
  );
}
