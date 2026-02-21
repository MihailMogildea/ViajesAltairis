"use client";

import { useState } from "react";
import {
  updateProvider,
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
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { StatusBadge } from "@/components/status-badge";
import { ToastMessage } from "@/components/toast-message";
import type { ProviderDto, ProviderTypeDto } from "@/types/provider";
import type {
  HotelDto,
  HotelProviderDto,
  HotelProviderRoomTypeDto,
  HotelProviderRoomTypeBoardDto,
} from "@/types/hotel";
import type { CurrencyDto, RoomTypeDto, BoardTypeDto, ExchangeRateDto } from "@/types/system";
import type { AccessLevel } from "@/lib/permissions";

// --- Info tab form ---
interface InfoFormState {
  typeId: number;
  currencyId: number;
  name: string;
  apiUrl: string;
  apiUsername: string;
  apiPassword: string;
  margin: number;
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
  { key: "hotel-links", label: "Hotel Links" },
  { key: "room-types", label: "Room Types" },
];

export function ProviderDetail({
  provider: initial,
  providerTypes,
  currencies,
  hotels,
  hotelProviders: initialHPs,
  roomTypes,
  boardTypes,
  hpRoomTypes: initialRTs,
  hpRoomTypeBoards: initialBoards,
  exchangeRates,
  rtNames,
  btNames,
  t,
  access,
}: {
  provider: ProviderDto;
  providerTypes: ProviderTypeDto[];
  currencies: CurrencyDto[];
  hotels: HotelDto[];
  hotelProviders: HotelProviderDto[];
  roomTypes: RoomTypeDto[];
  boardTypes: BoardTypeDto[];
  hpRoomTypes: HotelProviderRoomTypeDto[];
  hpRoomTypeBoards: HotelProviderRoomTypeBoardDto[];
  exchangeRates: ExchangeRateDto[];
  rtNames: Record<number, string>;
  btNames: Record<number, string>;
  t: Record<string, string>;
  access: AccessLevel | null;
}) {
  const isFull = access === "full";
  const [activeTab, setActiveTab] = useState("info");
  const [provider, setProvider] = useState(initial);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  // --- Info tab state ---
  const [infoForm, setInfoForm] = useState<InfoFormState>({
    typeId: initial.typeId,
    currencyId: initial.currencyId,
    name: initial.name,
    apiUrl: initial.apiUrl ?? "",
    apiUsername: initial.apiUsername ?? "",
    apiPassword: "",
    margin: initial.margin,
  });

  // --- Hotel Links state ---
  const [hotelProviders, setHotelProviders] = useState(initialHPs);
  const [hpModalOpen, setHpModalOpen] = useState(false);
  const [hpHotelId, setHpHotelId] = useState<number>(0);
  const [hpDeleting, setHpDeleting] = useState<HotelProviderDto | null>(null);

  // --- Room Types state ---
  const hpIds = new Set(hotelProviders.map((hp) => hp.id));
  const [rtItems, setRtItems] = useState(
    initialRTs.filter((rt) => hpIds.has(rt.hotelProviderId))
  );
  const [rtModalOpen, setRtModalOpen] = useState(false);
  const [rtEditing, setRtEditing] = useState<HotelProviderRoomTypeDto | null>(null);
  const [rtForm, setRtForm] = useState<RoomTypeFormState>(emptyRoomTypeForm);
  const [rtDeleting, setRtDeleting] = useState<HotelProviderRoomTypeDto | null>(null);

  // --- Board state ---
  const [boardItems, setBoardItems] = useState(initialBoards);
  const [expandedRtId, setExpandedRtId] = useState<number | null>(null);
  const [boardModalOpen, setBoardModalOpen] = useState(false);
  const [boardEditing, setBoardEditing] = useState<HotelProviderRoomTypeBoardDto | null>(null);
  const [boardForm, setBoardForm] = useState<BoardFormState>(emptyBoardForm);
  const [boardDeleting, setBoardDeleting] = useState<HotelProviderRoomTypeBoardDto | null>(null);

  // Lookup maps
  const hotelMap = new Map(hotels.map((h) => [h.id, h.name]));
  const roomTypeMap = new Map(roomTypes.map((rt) => [rt.id, rtNames[rt.id] ?? rt.name]));
  const boardTypeMap = new Map(boardTypes.map((bt) => [bt.id, btNames[bt.id] ?? bt.name]));
  const currencyMap = new Map(currencies.map((c) => [c.id, c.isoCode]));

  const localizedTabs = TABS.map((tab) => ({
    ...tab,
    label: t[`admin.providers.tab.${tab.key}`] ?? tab.label,
  }));

  // ===================== INFO TAB =====================

  async function handleInfoSave() {
    setPending(true);
    setMessage(null);
    try {
      const payload = {
        typeId: infoForm.typeId,
        currencyId: infoForm.currencyId,
        name: infoForm.name,
        apiUrl: infoForm.apiUrl || null,
        apiUsername: infoForm.apiUsername || null,
        apiPassword: infoForm.apiPassword || null,
        margin: infoForm.margin,
      };
      const updated = await updateProvider(provider.id, payload);
      setProvider(updated);
      setInfoForm((f) => ({ ...f, apiPassword: "" }));
      setMessage(t["admin.message.updated"] ?? "Updated successfully.");
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Update failed");
    } finally {
      setPending(false);
    }
  }

  // ===================== HOTEL LINKS TAB =====================

  async function handleHpCreate() {
    setPending(true);
    setMessage(null);
    try {
      const created = await createHotelProvider({
        hotelId: hpHotelId,
        providerId: provider.id,
      });
      setHotelProviders((prev) => [...prev, created]);
      setMessage(t["admin.message.created"] ?? "Created successfully.");
      setHpModalOpen(false);
      setHpHotelId(0);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Operation failed");
    } finally {
      setPending(false);
    }
  }

  async function handleHpDelete() {
    if (!hpDeleting) return;
    setPending(true);
    setMessage(null);
    try {
      await deleteHotelProvider(hpDeleting.id);
      setHotelProviders((prev) => prev.filter((i) => i.id !== hpDeleting.id));
      setMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setHpDeleting(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  async function handleHpToggle(item: HotelProviderDto) {
    setPending(true);
    setMessage(null);
    try {
      const newEnabled = !item.enabled;
      await toggleHotelProviderEnabled(item.id, newEnabled);
      setHotelProviders((prev) =>
        prev.map((i) =>
          i.id === item.id ? { ...i, enabled: newEnabled } : i
        )
      );
      const label = newEnabled
        ? t["admin.label.enabled"] ?? "Enabled"
        : t["admin.label.disabled"] ?? "Disabled";
      setMessage(`"${hotelMap.get(item.hotelId) ?? item.hotelId}" ${label.toLowerCase()}.`);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Toggle failed");
    } finally {
      setPending(false);
    }
  }

  // ===================== ROOM TYPES TAB =====================

  function openRtCreate() {
    setRtEditing(null);
    setRtForm({
      ...emptyRoomTypeForm,
      hotelProviderId: hotelProviders[0]?.id ?? 0,
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
    setMessage(null);
    try {
      if (rtEditing) {
        const updated = await updateHotelProviderRoomType(rtEditing.id, rtForm);
        setRtItems((prev) =>
          prev.map((i) => (i.id === rtEditing.id ? { ...i, ...updated } : i))
        );
        setMessage(t["admin.message.updated"] ?? "Updated successfully.");
      } else {
        const created = await createHotelProviderRoomType(rtForm);
        setRtItems((prev) => [...prev, created]);
        setMessage(t["admin.message.created"] ?? "Created successfully.");
      }
      setRtModalOpen(false);
      setRtEditing(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Operation failed");
    } finally {
      setPending(false);
    }
  }

  async function handleRtDelete() {
    if (!rtDeleting) return;
    setPending(true);
    setMessage(null);
    try {
      await deleteHotelProviderRoomType(rtDeleting.id);
      setRtItems((prev) => prev.filter((i) => i.id !== rtDeleting.id));
      setBoardItems((prev) =>
        prev.filter((b) => b.hotelProviderRoomTypeId !== rtDeleting.id)
      );
      setMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setRtDeleting(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  async function handleRtToggle(item: HotelProviderRoomTypeDto) {
    setPending(true);
    setMessage(null);
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
      setMessage(`"${roomTypeMap.get(item.roomTypeId) ?? item.roomTypeId}" ${label.toLowerCase()}.`);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Toggle failed");
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
    setMessage(null);
    try {
      if (boardEditing) {
        const updated = await updateHotelProviderRoomTypeBoard(
          boardEditing.id,
          boardForm
        );
        setBoardItems((prev) =>
          prev.map((i) => (i.id === boardEditing.id ? { ...i, ...updated } : i))
        );
        setMessage(t["admin.message.updated"] ?? "Updated successfully.");
      } else {
        const created = await createHotelProviderRoomTypeBoard(boardForm);
        setBoardItems((prev) => [...prev, created]);
        setMessage(t["admin.message.created"] ?? "Created successfully.");
      }
      setBoardModalOpen(false);
      setBoardEditing(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Operation failed");
    } finally {
      setPending(false);
    }
  }

  async function handleBoardDelete() {
    if (!boardDeleting) return;
    setPending(true);
    setMessage(null);
    try {
      await deleteHotelProviderRoomTypeBoard(boardDeleting.id);
      setBoardItems((prev) => prev.filter((i) => i.id !== boardDeleting.id));
      setMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setBoardDeleting(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  // ===================== HOTEL LINKS COLUMNS =====================

  const hpColumns: Column<HotelProviderDto>[] = [
    {
      key: "hotelId",
      header: t["admin.field.hotel"] ?? "Hotel",
      render: (item) => (
        <span>{hotelMap.get(item.hotelId) ?? item.hotelId}</span>
      ),
    },
    {
      key: "enabled",
      header: t["admin.field.enabled"] ?? "Status",
      render: (item) => (
        <StatusBadge
          variant={item.enabled ? "enabled" : "disabled"}
          onClick={isFull ? () => handleHpToggle(item) : undefined}
          disabled={pending}
        >
          {item.enabled
            ? t["admin.label.enabled"] ?? "Enabled"
            : t["admin.label.disabled"] ?? "Disabled"}
        </StatusBadge>
      ),
    },
  ];

  // ===================== ROOM TYPE COLUMNS =====================

  // Build hotelProvider lookup for display
  const hpMap = new Map(hotelProviders.map((hp) => [hp.id, hp]));

  const rtColumns: Column<HotelProviderRoomTypeDto>[] = [
    {
      key: "hotelProviderId",
      header: t["admin.field.hotel"] ?? "Hotel",
      render: (item) => {
        const hp = hpMap.get(item.hotelProviderId);
        return <span>{hp ? hotelMap.get(hp.hotelId) ?? hp.hotelId : item.hotelProviderId}</span>;
      },
    },
    {
      key: "roomTypeId",
      header: t["admin.field.roomType"] ?? "Room Type",
      render: (item) => (
        <span>{roomTypeMap.get(item.roomTypeId) ?? item.roomTypeId}</span>
      ),
    },
    {
      key: "capacity",
      header: t["admin.field.capacity"] ?? "Capacity",
    },
    {
      key: "quantity",
      header: t["admin.field.quantity"] ?? "Quantity",
    },
    {
      key: "pricePerNight",
      header: t["admin.field.pricePerNight"] ?? "Price/Night",
      render: (item) => (
        <span>
          {item.pricePerNight} {currencyMap.get(item.currencyId) ?? ""}
        </span>
      ),
    },
    {
      key: "enabled",
      header: t["admin.field.enabled"] ?? "Status",
      render: (item) => (
        <StatusBadge
          variant={item.enabled ? "enabled" : "disabled"}
          onClick={isFull ? () => handleRtToggle(item) : undefined}
          disabled={pending}
        >
          {item.enabled
            ? t["admin.label.enabled"] ?? "Enabled"
            : t["admin.label.disabled"] ?? "Disabled"}
        </StatusBadge>
      ),
    },
  ];

  // ===================== RENDER =====================

  return (
    <>
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">{provider.name}</h2>
        <p className="mt-1 text-sm text-gray-500">
          {t["admin.providers.detail_desc"] ?? "View and manage provider details, hotel links, and room configuration."}
        </p>
      </div>

      <ToastMessage message={message} onDismiss={() => setMessage(null)} />

      <TabBar tabs={localizedTabs} active={activeTab} onChange={setActiveTab} />

      {/* ========== INFO TAB ========== */}
      {activeTab === "info" && (
        <div className="max-w-lg space-y-4">
          <FormField
            label={t["admin.field.name"] ?? "Name"}
            value={infoForm.name}
            onChange={(v) => setInfoForm((f) => ({ ...f, name: String(v) }))}
            disabled={!isFull}
            required
          />
          <FormField
            label={t["admin.field.type"] ?? "Provider Type"}
            type="select"
            value={infoForm.typeId}
            onChange={(v) => setInfoForm((f) => ({ ...f, typeId: Number(v) }))}
            options={providerTypes.map((pt) => ({ value: pt.id, label: pt.name }))}
            disabled={!isFull}
          />
          <FormField
            label={t["admin.field.currency"] ?? "Currency"}
            type="select"
            value={infoForm.currencyId}
            onChange={(v) => setInfoForm((f) => ({ ...f, currencyId: Number(v) }))}
            options={currencies.map((c) => ({ value: c.id, label: `${c.isoCode} - ${c.name}` }))}
            disabled={!isFull}
          />
          <FormField
            label={t["admin.field.apiUrl"] ?? "API URL"}
            value={infoForm.apiUrl}
            onChange={(v) => setInfoForm((f) => ({ ...f, apiUrl: String(v) }))}
            disabled={!isFull}
          />
          <FormField
            label={t["admin.field.apiUsername"] ?? "API Username"}
            value={infoForm.apiUsername}
            onChange={(v) => setInfoForm((f) => ({ ...f, apiUsername: String(v) }))}
            disabled={!isFull}
          />
          <FormField
            label={t["admin.field.apiPassword"] ?? "API Password"}
            type="password"
            value={infoForm.apiPassword}
            onChange={(v) => setInfoForm((f) => ({ ...f, apiPassword: String(v) }))}
            placeholder={t["admin.providers.password_placeholder"] ?? "Leave blank to keep current"}
            disabled={!isFull}
          />
          <FormField
            label={t["admin.field.margin"] ?? "Margin (%)"}
            type="number"
            step="0.01"
            value={infoForm.margin}
            onChange={(v) => setInfoForm((f) => ({ ...f, margin: Number(v) }))}
            disabled={!isFull}
          />
          <div className="flex items-center gap-4 pt-2 text-sm text-gray-500">
            <span>
              {t["admin.field.syncStatus"] ?? "Sync Status"}:{" "}
              <strong>{provider.syncStatus ?? "-"}</strong>
            </span>
            <span>
              {t["admin.field.lastSyncedAt"] ?? "Last Synced"}:{" "}
              <strong>
                {provider.lastSyncedAt
                  ? new Date(provider.lastSyncedAt).toLocaleString()
                  : t["admin.label.never"] ?? "Never"}
              </strong>
            </span>
          </div>
          {isFull && (
            <button
              onClick={handleInfoSave}
              disabled={pending}
              className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
            >
              {pending ? "..." : t["admin.action.save"] ?? "Save"}
            </button>
          )}
        </div>
      )}

      {/* ========== HOTEL LINKS TAB ========== */}
      {activeTab === "hotel-links" && (
        <>
          {isFull && (
            <div className="mb-4 flex justify-end">
              <button
                onClick={() => {
                  setHpHotelId(0);
                  setHpModalOpen(true);
                }}
                className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
              >
                {t["admin.providers.assign_hotel"] ?? "Assign Hotel"}
              </button>
            </div>
          )}

          <DataTable
            columns={hpColumns}
            data={hotelProviders}
            keyField="id"
            emptyMessage={t["admin.providers.no_hotels"] ?? "No hotels linked to this provider."}
            actions={
              isFull
                ? (item) => (
                    <button
                      onClick={() => setHpDeleting(item)}
                      className="rounded border border-red-300 px-3 py-1 text-xs font-medium text-red-700 hover:bg-red-50"
                    >
                      {t["admin.action.delete"] ?? "Delete"}
                    </button>
                  )
                : undefined
            }
          />

          <FormModal
            open={hpModalOpen}
            title={t["admin.providers.assign_hotel"] ?? "Assign Hotel"}
            onClose={() => setHpModalOpen(false)}
            onSubmit={handleHpCreate}
            loading={pending}
            saveLabel={t["admin.action.save"] ?? "Save"}
            cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
          >
            <FormField
              label={t["admin.field.hotel"] ?? "Hotel"}
              type="select"
              value={hpHotelId}
              onChange={(v) => setHpHotelId(Number(v))}
              options={hotels.map((h) => ({ value: h.id, label: h.name }))}
              required
            />
          </FormModal>

          <ConfirmDialog
            open={!!hpDeleting}
            title={t["admin.confirm.delete.title"] ?? "Confirm Delete"}
            message={
              (t["admin.confirm.delete.message"] ?? "Are you sure you want to delete \"{name}\"?").replace(
                "{name}",
                hpDeleting ? (hotelMap.get(hpDeleting.hotelId) ?? String(hpDeleting.hotelId)) : ""
              )
            }
            onConfirm={handleHpDelete}
            onCancel={() => setHpDeleting(null)}
            loading={pending}
            confirmLabel={t["admin.action.delete"] ?? "Delete"}
            cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
          />
        </>
      )}

      {/* ========== ROOM TYPES TAB ========== */}
      {activeTab === "room-types" && (
        <>
          {isFull && (
            <div className="mb-4 flex justify-end">
              <button
                onClick={openRtCreate}
                className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
              >
                {t["admin.action.create"] ?? "Create"}
              </button>
            </div>
          )}

          {/* Room types table with expandable board rows */}
          <div className="overflow-hidden rounded-lg border border-gray-200 bg-white">
            <table className="w-full text-left text-sm">
              <thead className="border-b border-gray-200 bg-gray-50 text-xs font-medium uppercase text-gray-500">
                <tr>
                  {rtColumns.map((col) => (
                    <th key={col.key} className={`px-4 py-3 ${col.className ?? ""}`}>
                      {col.header}
                    </th>
                  ))}
                  <th className="px-4 py-3 text-right">
                    {t["admin.field.actions"] ?? "Actions"}
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {rtItems.length === 0 && (
                  <tr>
                    <td
                      colSpan={rtColumns.length + 1}
                      className="px-4 py-8 text-center text-sm text-gray-500"
                    >
                      {t["admin.providers.no_room_types"] ?? "No room types configured."}
                    </td>
                  </tr>
                )}
                {rtItems.map((rt) => {
                  const isExpanded = expandedRtId === rt.id;
                  const rtBoards = boardItems.filter(
                    (b) => b.hotelProviderRoomTypeId === rt.id
                  );
                  return (
                    <RoomTypeRow
                      key={rt.id}
                      rt={rt}
                      rtColumns={rtColumns}
                      isExpanded={isExpanded}
                      boards={rtBoards}
                      boardTypeMap={boardTypeMap}
                      isFull={isFull}
                      pending={pending}
                      t={t}
                      onToggleExpand={() =>
                        setExpandedRtId(isExpanded ? null : rt.id)
                      }
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
          </div>

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
              label={t["admin.field.hotelLink"] ?? "Hotel Link"}
              type="select"
              value={rtForm.hotelProviderId}
              onChange={(v) => setRtForm((f) => ({ ...f, hotelProviderId: Number(v) }))}
              options={hotelProviders.map((hp) => ({
                value: hp.id,
                label: hotelMap.get(hp.hotelId) ?? String(hp.hotelId),
              }))}
              required
            />
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
      )}
    </>
  );
}

// ===================== ROOM TYPE ROW (with expandable boards) =====================

function RoomTypeRow({
  rt,
  rtColumns,
  isExpanded,
  boards,
  boardTypeMap,
  isFull,
  pending,
  t,
  onToggleExpand,
  onEdit,
  onDelete,
  onBoardCreate,
  onBoardEdit,
  onBoardDelete,
}: {
  rt: HotelProviderRoomTypeDto;
  rtColumns: Column<HotelProviderRoomTypeDto>[];
  isExpanded: boolean;
  boards: HotelProviderRoomTypeBoardDto[];
  boardTypeMap: Map<number, string>;
  isFull: boolean;
  pending: boolean;
  t: Record<string, string>;
  onToggleExpand: () => void;
  onEdit: () => void;
  onDelete: () => void;
  onBoardCreate: () => void;
  onBoardEdit: (item: HotelProviderRoomTypeBoardDto) => void;
  onBoardDelete: (item: HotelProviderRoomTypeBoardDto) => void;
}) {
  const colCount = rtColumns.length + 1;

  return (
    <>
      <tr className="hover:bg-gray-50">
        {rtColumns.map((col) => (
          <td key={col.key} className={`px-4 py-3 ${col.className ?? ""}`}>
            {col.render
              ? col.render(rt)
              : String((rt as unknown as Record<string, unknown>)[col.key] ?? "")}
          </td>
        ))}
        <td className="px-4 py-3 text-right">
          <div className="flex items-center justify-end gap-2">
            <button
              onClick={onToggleExpand}
              className="rounded border border-gray-300 px-3 py-1 text-xs font-medium text-gray-700 hover:bg-gray-50"
            >
              {isExpanded
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
      {isExpanded && (
        <tr>
          <td colSpan={colCount} className="bg-gray-50 px-8 py-4">
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
