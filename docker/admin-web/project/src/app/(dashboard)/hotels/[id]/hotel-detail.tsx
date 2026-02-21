"use client";

import { useState } from "react";
import {
  updateHotel,
  createHotelImage,
  deleteHotelImage,
  addHotelAmenity,
  removeHotelAmenity,
  createHotelProvider,
  deleteHotelProvider,
  toggleHotelProviderEnabled,
  createHotelProviderRoomType,
  updateHotelProviderRoomType,
  deleteHotelProviderRoomType,
  toggleHotelProviderRoomTypeEnabled,
  createHotelProviderRoomTypeBoard,
  updateHotelProviderRoomTypeBoard,
  deleteHotelProviderRoomTypeBoard,
} from "./actions";
import { TabBar } from "@/components/tab-bar";
import { DataTable, type Column } from "@/components/data-table";
import { FormField } from "@/components/form-field";
import { FormModal } from "@/components/form-modal";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { StatusBadge } from "@/components/status-badge";
import { ToastMessage } from "@/components/toast-message";
import type {
  HotelDto,
  HotelImageDto,
  HotelAmenityDto,
  HotelProviderDto,
  HotelProviderRoomTypeDto,
  HotelProviderRoomTypeBoardDto,
} from "@/types/hotel";
import type { ProviderDto } from "@/types/provider";
import type {
  AmenityDto,
  AmenityCategoryDto,
  CityDto,
  RoomTypeDto,
  BoardTypeDto,
  CurrencyDto,
  ExchangeRateDto,
} from "@/types/system";
import type { AccessLevel } from "@/lib/permissions";

interface Props {
  hotel: HotelDto;
  images: HotelImageDto[];
  hotelAmenities: HotelAmenityDto[];
  amenities: AmenityDto[];
  amenityCategories: AmenityCategoryDto[];
  cities: CityDto[];
  hotelProviders: HotelProviderDto[];
  hpRoomTypes: HotelProviderRoomTypeDto[];
  hpRoomTypeBoards: HotelProviderRoomTypeBoardDto[];
  providers: ProviderDto[];
  roomTypes: RoomTypeDto[];
  boardTypes: BoardTypeDto[];
  currencies: CurrencyDto[];
  exchangeRates: ExchangeRateDto[];
  rtNames: Record<number, string>;
  btNames: Record<number, string>;
  t: Record<string, string>;
  access: AccessLevel | null;
}

// --- Room type form ---
interface RoomTypeFormState {
  hotelProviderId: number;
  roomTypeId: number;
  capacity: number;
  quantity: number;
  pricePerNight: number;
  currencyId: number;
  exchangeRateId: number;
}

const emptyRoomTypeForm: RoomTypeFormState = {
  hotelProviderId: 0,
  roomTypeId: 0,
  capacity: 1,
  quantity: 1,
  pricePerNight: 0,
  currencyId: 0,
  exchangeRateId: 0,
};

// --- Board form ---
interface BoardFormState {
  hotelProviderRoomTypeId: number;
  boardTypeId: number;
  pricePerNight: number;
}

const emptyBoardForm: BoardFormState = {
  hotelProviderRoomTypeId: 0,
  boardTypeId: 0,
  pricePerNight: 0,
};

const TABS = [
  { key: "info", label: "Info" },
  { key: "images", label: "Images" },
  { key: "amenities", label: "Amenities" },
  { key: "providers", label: "Providers" },
  { key: "rooms", label: "Room Config" },
];

export function HotelDetail({
  hotel: initial,
  images: initialImages,
  hotelAmenities: initialHotelAmenities,
  amenities,
  amenityCategories,
  cities,
  hotelProviders: initialHPs,
  hpRoomTypes: initialRTs,
  hpRoomTypeBoards: initialBoards,
  providers,
  roomTypes,
  boardTypes,
  currencies,
  exchangeRates,
  rtNames,
  btNames,
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
      {activeTab === "providers" && (
        <ProvidersTab
          hotel={initial}
          hotelProviders={initialHPs}
          providers={providers}
          t={t}
          access={access}
          onMessage={setMessage}
        />
      )}
      {activeTab === "rooms" && (
        <RoomConfigTab
          hotel={initial}
          hotelProviders={initialHPs}
          hpRoomTypes={initialRTs}
          hpRoomTypeBoards={initialBoards}
          providers={providers}
          roomTypes={roomTypes}
          boardTypes={boardTypes}
          currencies={currencies}
          exchangeRates={exchangeRates}
          rtNames={rtNames}
          btNames={btNames}
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

/* ─── Providers Tab ─── */

function ProvidersTab({
  hotel,
  hotelProviders: initial,
  providers,
  t,
  access,
  onMessage,
}: {
  hotel: HotelDto;
  hotelProviders: HotelProviderDto[];
  providers: ProviderDto[];
  t: Record<string, string>;
  access: AccessLevel | null;
  onMessage: (msg: string) => void;
}) {
  const isFull = access === "full";

  const [items, setItems] = useState(initial);
  const [modalOpen, setModalOpen] = useState(false);
  const [providerId, setProviderId] = useState<number>(0);
  const [deleting, setDeleting] = useState<HotelProviderDto | null>(null);
  const [pending, setPending] = useState(false);

  const internalProviders = providers.filter((p) => p.typeId === 1);
  const providerMap = new Map(providers.map((p) => [p.id, p.name]));

  async function handleCreate() {
    setPending(true);
    try {
      const created = await createHotelProvider({
        hotelId: hotel.id,
        providerId,
      });
      setItems((prev) => [...prev, created]);
      onMessage(t["admin.message.created"] ?? "Created successfully.");
      setModalOpen(false);
      setProviderId(0);
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
      await deleteHotelProvider(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      onMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setDeleting(null);
    } catch (e) {
      onMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  async function handleToggle(item: HotelProviderDto) {
    setPending(true);
    try {
      const newEnabled = !item.enabled;
      await toggleHotelProviderEnabled(item.id, newEnabled);
      setItems((prev) =>
        prev.map((i) =>
          i.id === item.id ? { ...i, enabled: newEnabled } : i
        )
      );
      const label = newEnabled
        ? t["admin.label.enabled"] ?? "Enabled"
        : t["admin.label.disabled"] ?? "Disabled";
      onMessage(`"${providerMap.get(item.providerId) ?? item.providerId}" ${label.toLowerCase()}.`);
    } catch (e) {
      onMessage(e instanceof Error ? e.message : "Toggle failed");
    } finally {
      setPending(false);
    }
  }

  const columns: Column<HotelProviderDto>[] = [
    {
      key: "providerId",
      header: t["admin.field.provider"] ?? "Provider",
      render: (item) => (
        <span>{providerMap.get(item.providerId) ?? item.providerId}</span>
      ),
    },
    {
      key: "enabled",
      header: t["admin.field.enabled"] ?? "Status",
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
      {isFull && (
        <div className="mb-4 flex justify-end">
          <button
            onClick={() => {
              setProviderId(0);
              setModalOpen(true);
            }}
            className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
          >
            {t["admin.hotels.assign_provider"] ?? "Assign Provider"}
          </button>
        </div>
      )}

      <DataTable
        columns={columns}
        data={items}
        keyField="id"
        emptyMessage={t["admin.hotels.no_providers"] ?? "No providers linked to this hotel."}
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
        title={t["admin.hotels.assign_provider"] ?? "Assign Provider"}
        onClose={() => setModalOpen(false)}
        onSubmit={handleCreate}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.field.provider"] ?? "Provider"}
          type="select"
          value={providerId}
          onChange={(v) => setProviderId(Number(v))}
          options={internalProviders.map((p) => ({ value: p.id, label: p.name }))}
          required
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleting}
        title={t["admin.confirm.delete.title"] ?? "Confirm Delete"}
        message={
          (t["admin.confirm.delete.message"] ?? "Are you sure you want to delete \"{name}\"?").replace(
            "{name}",
            deleting ? (providerMap.get(deleting.providerId) ?? String(deleting.providerId)) : ""
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

/* ─── Room Config Tab ─── */

function RoomConfigTab({
  hotel,
  hotelProviders,
  hpRoomTypes: initialRTs,
  hpRoomTypeBoards: initialBoards,
  providers,
  roomTypes,
  boardTypes,
  currencies,
  exchangeRates,
  rtNames,
  btNames,
  t,
  access,
  onMessage,
}: {
  hotel: HotelDto;
  hotelProviders: HotelProviderDto[];
  hpRoomTypes: HotelProviderRoomTypeDto[];
  hpRoomTypeBoards: HotelProviderRoomTypeBoardDto[];
  providers: ProviderDto[];
  roomTypes: RoomTypeDto[];
  boardTypes: BoardTypeDto[];
  currencies: CurrencyDto[];
  exchangeRates: ExchangeRateDto[];
  rtNames: Record<number, string>;
  btNames: Record<number, string>;
  t: Record<string, string>;
  access: AccessLevel | null;
  onMessage: (msg: string) => void;
}) {
  const isFull = access === "full";

  // Filter to only this hotel's hotelProvider IDs
  const hpIds = new Set(hotelProviders.map((hp) => hp.id));

  const [rtItems, setRtItems] = useState(
    initialRTs.filter((rt) => hpIds.has(rt.hotelProviderId))
  );
  const [rtModalOpen, setRtModalOpen] = useState(false);
  const [rtEditing, setRtEditing] = useState<HotelProviderRoomTypeDto | null>(null);
  const [rtForm, setRtForm] = useState<RoomTypeFormState>(emptyRoomTypeForm);
  const [rtDeleting, setRtDeleting] = useState<HotelProviderRoomTypeDto | null>(null);
  const [pending, setPending] = useState(false);

  // Board state
  const [boardItems, setBoardItems] = useState(initialBoards);
  const [expandedRtId, setExpandedRtId] = useState<number | null>(null);
  const [boardModalOpen, setBoardModalOpen] = useState(false);
  const [boardEditing, setBoardEditing] = useState<HotelProviderRoomTypeBoardDto | null>(null);
  const [boardForm, setBoardForm] = useState<BoardFormState>(emptyBoardForm);
  const [boardDeleting, setBoardDeleting] = useState<HotelProviderRoomTypeBoardDto | null>(null);

  // Provider collapse state
  const [expandedHpId, setExpandedHpId] = useState<number | null>(null);

  // Lookup maps
  const providerMap = new Map(providers.map((p) => [p.id, p.name]));
  const roomTypeMap = new Map(roomTypes.map((rt) => [rt.id, rtNames[rt.id] ?? rt.name]));
  const boardTypeMap = new Map(boardTypes.map((bt) => [bt.id, btNames[bt.id] ?? bt.name]));
  const currencyMap = new Map(currencies.map((c) => [c.id, c.isoCode]));

  // Only internal providers linked to this hotel
  const internalHPs = hotelProviders.filter((hp) => {
    const p = providers.find((pr) => pr.id === hp.providerId);
    return p && p.typeId === 1;
  });

  // ===================== ROOM TYPES =====================

  function openRtCreate(hpId: number) {
    setRtEditing(null);
    setRtForm({
      ...emptyRoomTypeForm,
      hotelProviderId: hpId,
    });
    setRtModalOpen(true);
  }

  function openRtEdit(item: HotelProviderRoomTypeDto) {
    setRtEditing(item);
    setRtForm({
      hotelProviderId: item.hotelProviderId,
      roomTypeId: item.roomTypeId,
      capacity: item.capacity,
      quantity: item.quantity,
      pricePerNight: item.pricePerNight,
      currencyId: item.currencyId,
      exchangeRateId: item.exchangeRateId,
    });
    setRtModalOpen(true);
  }

  async function handleRtSubmit() {
    setPending(true);
    try {
      if (rtEditing) {
        const updated = await updateHotelProviderRoomType(rtEditing.id, rtForm);
        setRtItems((prev) =>
          prev.map((i) => (i.id === rtEditing.id ? { ...i, ...updated } : i))
        );
        onMessage(t["admin.message.updated"] ?? "Updated successfully.");
      } else {
        const created = await createHotelProviderRoomType(rtForm);
        setRtItems((prev) => [...prev, created]);
        onMessage(t["admin.message.created"] ?? "Created successfully.");
      }
      setRtModalOpen(false);
      setRtEditing(null);
    } catch (e) {
      onMessage(e instanceof Error ? e.message : "Operation failed");
    } finally {
      setPending(false);
    }
  }

  async function handleRtDelete() {
    if (!rtDeleting) return;
    setPending(true);
    try {
      await deleteHotelProviderRoomType(rtDeleting.id);
      setRtItems((prev) => prev.filter((i) => i.id !== rtDeleting.id));
      setBoardItems((prev) =>
        prev.filter((b) => b.hotelProviderRoomTypeId !== rtDeleting.id)
      );
      onMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setRtDeleting(null);
    } catch (e) {
      onMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  async function handleRtToggle(item: HotelProviderRoomTypeDto) {
    setPending(true);
    try {
      const newEnabled = !item.enabled;
      await toggleHotelProviderRoomTypeEnabled(item.id, newEnabled);
      setRtItems((prev) =>
        prev.map((i) =>
          i.id === item.id ? { ...i, enabled: newEnabled } : i
        )
      );
      const label = newEnabled
        ? t["admin.label.enabled"] ?? "Enabled"
        : t["admin.label.disabled"] ?? "Disabled";
      onMessage(`"${roomTypeMap.get(item.roomTypeId) ?? item.roomTypeId}" ${label.toLowerCase()}.`);
    } catch (e) {
      onMessage(e instanceof Error ? e.message : "Toggle failed");
    } finally {
      setPending(false);
    }
  }

  // ===================== BOARDS =====================

  function openBoardCreate(rtId: number) {
    setBoardEditing(null);
    setBoardForm({ ...emptyBoardForm, hotelProviderRoomTypeId: rtId });
    setBoardModalOpen(true);
  }

  function openBoardEdit(item: HotelProviderRoomTypeBoardDto) {
    setBoardEditing(item);
    setBoardForm({
      hotelProviderRoomTypeId: item.hotelProviderRoomTypeId,
      boardTypeId: item.boardTypeId,
      pricePerNight: item.pricePerNight,
    });
    setBoardModalOpen(true);
  }

  async function handleBoardSubmit() {
    setPending(true);
    try {
      if (boardEditing) {
        const updated = await updateHotelProviderRoomTypeBoard(
          boardEditing.id,
          boardForm
        );
        setBoardItems((prev) =>
          prev.map((i) => (i.id === boardEditing.id ? { ...i, ...updated } : i))
        );
        onMessage(t["admin.message.updated"] ?? "Updated successfully.");
      } else {
        const created = await createHotelProviderRoomTypeBoard(boardForm);
        setBoardItems((prev) => [...prev, created]);
        onMessage(t["admin.message.created"] ?? "Created successfully.");
      }
      setBoardModalOpen(false);
      setBoardEditing(null);
    } catch (e) {
      onMessage(e instanceof Error ? e.message : "Operation failed");
    } finally {
      setPending(false);
    }
  }

  async function handleBoardDelete() {
    if (!boardDeleting) return;
    setPending(true);
    try {
      await deleteHotelProviderRoomTypeBoard(boardDeleting.id);
      setBoardItems((prev) => prev.filter((i) => i.id !== boardDeleting.id));
      onMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setBoardDeleting(null);
    } catch (e) {
      onMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  return (
    <>
      {internalHPs.length === 0 ? (
        <div className="rounded-lg border border-gray-200 bg-white p-8 text-center text-sm text-gray-500">
          {t["admin.hotels.no_internal_providers"] ?? "No internal providers linked to this hotel."}
        </div>
      ) : (
        <div className="space-y-4">
          {internalHPs.map((hp) => {
            const provName = providerMap.get(hp.providerId) ?? String(hp.providerId);
            const isExpanded = expandedHpId === hp.id;
            const hpRts = rtItems.filter((rt) => rt.hotelProviderId === hp.id);

            return (
              <div key={hp.id} className="overflow-hidden rounded-lg border border-gray-200 bg-white">
                {/* Provider header row */}
                <button
                  onClick={() => setExpandedHpId(isExpanded ? null : hp.id)}
                  className="flex w-full items-center justify-between px-4 py-3 text-left hover:bg-gray-50"
                >
                  <div className="flex items-center gap-3">
                    <span className="text-xs text-gray-400">{isExpanded ? "\u25BC" : "\u25B6"}</span>
                    <span className="text-sm font-semibold text-gray-900">{provName}</span>
                    <span className="text-xs text-gray-500">
                      ({hpRts.length} {t["admin.hotels.room_types_count"] ?? "room types"})
                    </span>
                  </div>
                  <StatusBadge variant={hp.enabled ? "enabled" : "disabled"}>
                    {hp.enabled
                      ? t["admin.label.enabled"] ?? "Enabled"
                      : t["admin.label.disabled"] ?? "Disabled"}
                  </StatusBadge>
                </button>

                {/* Expanded: room types table */}
                {isExpanded && (
                  <div className="border-t border-gray-200">
                    {isFull && (
                      <div className="flex justify-end px-4 py-2">
                        <button
                          onClick={() => openRtCreate(hp.id)}
                          className="rounded bg-blue-600 px-3 py-1 text-xs font-medium text-white hover:bg-blue-700"
                        >
                          {t["admin.providers.create_room_type"] ?? "Create Room Type"}
                        </button>
                      </div>
                    )}

                    {hpRts.length === 0 ? (
                      <p className="px-4 pb-4 text-sm text-gray-500">
                        {t["admin.hotels.no_room_types"] ?? "No room types configured."}
                      </p>
                    ) : (
                      <table className="w-full text-left text-sm">
                        <thead className="border-b border-gray-100 bg-gray-50 text-xs font-medium uppercase text-gray-500">
                          <tr>
                            <th className="px-4 py-2">{t["admin.field.roomType"] ?? "Room Type"}</th>
                            <th className="px-4 py-2">{t["admin.field.capacity"] ?? "Capacity"}</th>
                            <th className="px-4 py-2">{t["admin.field.quantity"] ?? "Quantity"}</th>
                            <th className="px-4 py-2">{t["admin.field.pricePerNight"] ?? "Price/Night"}</th>
                            <th className="px-4 py-2">{t["admin.field.enabled"] ?? "Status"}</th>
                            <th className="px-4 py-2 text-right">{t["admin.field.actions"] ?? "Actions"}</th>
                          </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-100">
                          {hpRts.map((rt) => {
                            const rtBoards = boardItems.filter(
                              (b) => b.hotelProviderRoomTypeId === rt.id
                            );
                            const isBoardExpanded = expandedRtId === rt.id;

                            return (
                              <RoomTypeRow
                                key={rt.id}
                                rt={rt}
                                isBoardExpanded={isBoardExpanded}
                                boards={rtBoards}
                                roomTypeMap={roomTypeMap}
                                currencyMap={currencyMap}
                                boardTypeMap={boardTypeMap}
                                isFull={isFull}
                                pending={pending}
                                t={t}
                                onToggleBoardExpand={() =>
                                  setExpandedRtId(isBoardExpanded ? null : rt.id)
                                }
                                onToggleEnabled={() => handleRtToggle(rt)}
                                onEdit={() => openRtEdit(rt)}
                                onDelete={() => setRtDeleting(rt)}
                                onBoardCreate={() => openBoardCreate(rt.id)}
                                onBoardEdit={openBoardEdit}
                                onBoardDelete={setBoardDeleting}
                              />
                            );
                          })}
                        </tbody>
                      </table>
                    )}
                  </div>
                )}
              </div>
            );
          })}
        </div>
      )}

      {/* Room Type Modal */}
      <FormModal
        open={rtModalOpen}
        title={
          rtEditing
            ? t["admin.providers.edit_room_type"] ?? "Edit Room Type"
            : t["admin.providers.create_room_type"] ?? "Create Room Type"
        }
        onClose={() => {
          setRtModalOpen(false);
          setRtEditing(null);
        }}
        onSubmit={handleRtSubmit}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.field.roomType"] ?? "Room Type"}
          type="select"
          value={rtForm.roomTypeId}
          onChange={(v) => setRtForm((f) => ({ ...f, roomTypeId: Number(v) }))}
          options={roomTypes.map((rt) => ({ value: rt.id, label: rtNames[rt.id] ?? rt.name }))}
          required
        />
        <FormField
          label={t["admin.field.capacity"] ?? "Capacity"}
          type="number"
          value={rtForm.capacity}
          onChange={(v) => setRtForm((f) => ({ ...f, capacity: Number(v) }))}
          required
        />
        <FormField
          label={t["admin.field.quantity"] ?? "Quantity"}
          type="number"
          value={rtForm.quantity}
          onChange={(v) => setRtForm((f) => ({ ...f, quantity: Number(v) }))}
          required
        />
        <FormField
          label={t["admin.field.pricePerNight"] ?? "Price per Night"}
          type="number"
          step="0.01"
          value={rtForm.pricePerNight}
          onChange={(v) => setRtForm((f) => ({ ...f, pricePerNight: Number(v) }))}
          required
        />
        <FormField
          label={t["admin.field.currency"] ?? "Currency"}
          type="select"
          value={rtForm.currencyId}
          onChange={(v) => setRtForm((f) => ({ ...f, currencyId: Number(v) }))}
          options={currencies.map((c) => ({ value: c.id, label: `${c.isoCode} - ${c.name}` }))}
          required
        />
        <FormField
          label={t["admin.field.exchangeRate"] ?? "Exchange Rate"}
          type="select"
          value={rtForm.exchangeRateId}
          onChange={(v) => setRtForm((f) => ({ ...f, exchangeRateId: Number(v) }))}
          options={exchangeRates.map((er) => ({
            value: er.id,
            label: `${currencyMap.get(er.currencyId) ?? er.currencyId} - ${er.rateToEur} (${er.validFrom})`,
          }))}
          required
        />
      </FormModal>

      {/* Room Type Delete Confirm */}
      <ConfirmDialog
        open={!!rtDeleting}
        title={t["admin.confirm.delete.title"] ?? "Confirm Delete"}
        message={
          (t["admin.confirm.delete.message"] ?? "Are you sure you want to delete \"{name}\"?").replace(
            "{name}",
            rtDeleting ? (roomTypeMap.get(rtDeleting.roomTypeId) ?? String(rtDeleting.id)) : ""
          )
        }
        onConfirm={handleRtDelete}
        onCancel={() => setRtDeleting(null)}
        loading={pending}
        confirmLabel={t["admin.action.delete"] ?? "Delete"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      />

      {/* Board Modal */}
      <FormModal
        open={boardModalOpen}
        title={
          boardEditing
            ? t["admin.providers.edit_board"] ?? "Edit Board Type"
            : t["admin.providers.create_board"] ?? "Add Board Type"
        }
        onClose={() => {
          setBoardModalOpen(false);
          setBoardEditing(null);
        }}
        onSubmit={handleBoardSubmit}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.field.boardType"] ?? "Board Type"}
          type="select"
          value={boardForm.boardTypeId}
          onChange={(v) => setBoardForm((f) => ({ ...f, boardTypeId: Number(v) }))}
          options={boardTypes.map((bt) => ({ value: bt.id, label: btNames[bt.id] ?? bt.name }))}
          required
        />
        <FormField
          label={t["admin.field.pricePerNight"] ?? "Price per Night"}
          type="number"
          step="0.01"
          value={boardForm.pricePerNight}
          onChange={(v) => setBoardForm((f) => ({ ...f, pricePerNight: Number(v) }))}
          required
        />
      </FormModal>

      {/* Board Delete Confirm */}
      <ConfirmDialog
        open={!!boardDeleting}
        title={t["admin.confirm.delete.title"] ?? "Confirm Delete"}
        message={
          (t["admin.confirm.delete.message"] ?? "Are you sure you want to delete \"{name}\"?").replace(
            "{name}",
            boardDeleting ? (boardTypeMap.get(boardDeleting.boardTypeId) ?? String(boardDeleting.id)) : ""
          )
        }
        onConfirm={handleBoardDelete}
        onCancel={() => setBoardDeleting(null)}
        loading={pending}
        confirmLabel={t["admin.action.delete"] ?? "Delete"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      />
    </>
  );
}

// ===================== ROOM TYPE ROW (with expandable boards) =====================

function RoomTypeRow({
  rt,
  isBoardExpanded,
  boards,
  roomTypeMap,
  currencyMap,
  boardTypeMap,
  isFull,
  pending,
  t,
  onToggleBoardExpand,
  onToggleEnabled,
  onEdit,
  onDelete,
  onBoardCreate,
  onBoardEdit,
  onBoardDelete,
}: {
  rt: HotelProviderRoomTypeDto;
  isBoardExpanded: boolean;
  boards: HotelProviderRoomTypeBoardDto[];
  roomTypeMap: Map<number, string>;
  currencyMap: Map<number, string>;
  boardTypeMap: Map<number, string>;
  isFull: boolean;
  pending: boolean;
  t: Record<string, string>;
  onToggleBoardExpand: () => void;
  onToggleEnabled: () => void;
  onEdit: () => void;
  onDelete: () => void;
  onBoardCreate: () => void;
  onBoardEdit: (item: HotelProviderRoomTypeBoardDto) => void;
  onBoardDelete: (item: HotelProviderRoomTypeBoardDto) => void;
}) {
  return (
    <>
      <tr className="hover:bg-gray-50">
        <td className="px-4 py-3">
          {roomTypeMap.get(rt.roomTypeId) ?? rt.roomTypeId}
        </td>
        <td className="px-4 py-3">{rt.capacity}</td>
        <td className="px-4 py-3">{rt.quantity}</td>
        <td className="px-4 py-3">
          {rt.pricePerNight} {currencyMap.get(rt.currencyId) ?? ""}
        </td>
        <td className="px-4 py-3">
          <StatusBadge
            variant={rt.enabled ? "enabled" : "disabled"}
            onClick={isFull ? onToggleEnabled : undefined}
            disabled={pending}
          >
            {rt.enabled
              ? t["admin.label.enabled"] ?? "Enabled"
              : t["admin.label.disabled"] ?? "Disabled"}
          </StatusBadge>
        </td>
        <td className="px-4 py-3 text-right">
          <div className="flex items-center justify-end gap-2">
            <button
              onClick={onToggleBoardExpand}
              className="rounded border border-gray-300 px-3 py-1 text-xs font-medium text-gray-700 hover:bg-gray-50"
            >
              {isBoardExpanded
                ? t["admin.action.collapse"] ?? "Collapse"
                : `${t["admin.action.boards"] ?? "Boards"} (${boards.length})`}
            </button>
            {isFull && (
              <>
                <button
                  onClick={onEdit}
                  className="rounded border border-gray-300 px-3 py-1 text-xs font-medium text-gray-700 hover:bg-gray-50"
                >
                  {t["admin.action.edit"] ?? "Edit"}
                </button>
                <button
                  onClick={onDelete}
                  className="rounded border border-red-300 px-3 py-1 text-xs font-medium text-red-700 hover:bg-red-50"
                >
                  {t["admin.action.delete"] ?? "Delete"}
                </button>
              </>
            )}
          </div>
        </td>
      </tr>
      {isBoardExpanded && (
        <tr>
          <td colSpan={6} className="bg-gray-50 px-8 py-4">
            <div className="mb-2 flex items-center justify-between">
              <span className="text-xs font-semibold uppercase text-gray-500">
                {t["admin.providers.board_types"] ?? "Board Types"}
              </span>
              {isFull && (
                <button
                  onClick={onBoardCreate}
                  className="rounded bg-blue-600 px-3 py-1 text-xs font-medium text-white hover:bg-blue-700"
                >
                  {t["admin.action.add"] ?? "Add"}
                </button>
              )}
            </div>
            {boards.length === 0 ? (
              <p className="text-xs text-gray-400">
                {t["admin.providers.no_boards"] ?? "No board types assigned."}
              </p>
            ) : (
              <table className="w-full text-left text-xs">
                <thead className="text-xs font-medium uppercase text-gray-400">
                  <tr>
                    <th className="pb-2">{t["admin.field.boardType"] ?? "Board Type"}</th>
                    <th className="pb-2">{t["admin.field.pricePerNight"] ?? "Price/Night"}</th>
                    <th className="pb-2">{t["admin.field.enabled"] ?? "Status"}</th>
                    {isFull && (
                      <th className="pb-2 text-right">
                        {t["admin.field.actions"] ?? "Actions"}
                      </th>
                    )}
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-200">
                  {boards.map((board) => (
                    <tr key={board.id}>
                      <td className="py-2">
                        {boardTypeMap.get(board.boardTypeId) ?? board.boardTypeId}
                      </td>
                      <td className="py-2">{board.pricePerNight}</td>
                      <td className="py-2">
                        <StatusBadge
                          variant={board.enabled ? "enabled" : "disabled"}
                        >
                          {board.enabled
                            ? t["admin.label.enabled"] ?? "Enabled"
                            : t["admin.label.disabled"] ?? "Disabled"}
                        </StatusBadge>
                      </td>
                      {isFull && (
                        <td className="py-2 text-right">
                          <div className="flex items-center justify-end gap-2">
                            <button
                              onClick={() => onBoardEdit(board)}
                              disabled={pending}
                              className="rounded border border-gray-300 px-2 py-0.5 text-xs font-medium text-gray-700 hover:bg-gray-50"
                            >
                              {t["admin.action.edit"] ?? "Edit"}
                            </button>
                            <button
                              onClick={() => onBoardDelete(board)}
                              disabled={pending}
                              className="rounded border border-red-300 px-2 py-0.5 text-xs font-medium text-red-700 hover:bg-red-50"
                            >
                              {t["admin.action.delete"] ?? "Delete"}
                            </button>
                          </div>
                        </td>
                      )}
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </td>
        </tr>
      )}
    </>
  );
}
