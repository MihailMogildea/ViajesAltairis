"use client";

import { useState, useMemo } from "react";
import { DataTable, type Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import { createBlackout, updateBlackout, deleteBlackout } from "./actions";
import type { HotelDto, HotelBlackoutDto, CreateHotelBlackoutRequest } from "@/types/hotel";
import type { AccessLevel } from "@/lib/permissions";

interface BlackoutsTabProps {
  blackouts: HotelBlackoutDto[];
  hotels: HotelDto[];
  access: AccessLevel;
  t: Record<string, string>;
}

const emptyForm: CreateHotelBlackoutRequest = {
  hotelId: 0,
  startDate: "",
  endDate: "",
  reason: null,
};

export function BlackoutsTab({ blackouts: initial, hotels, access, t }: BlackoutsTabProps) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<HotelBlackoutDto | null>(null);
  const [formOpen, setFormOpen] = useState(false);
  const [form, setForm] = useState<CreateHotelBlackoutRequest>(emptyForm);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<HotelBlackoutDto | null>(null);

  const hotelMap = useMemo(() => {
    const map: Record<number, string> = {};
    for (const h of hotels) map[h.id] = h.name;
    return map;
  }, [hotels]);

  const hotelOptions = useMemo(
    () => hotels.map((h) => ({ value: h.id, label: h.name })),
    [hotels]
  );

  const canEdit = access === "full";

  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    setFormOpen(true);
    setMessage(null);
  }

  function openEdit(item: HotelBlackoutDto) {
    setEditing(item);
    setForm({
      hotelId: item.hotelId,
      startDate: item.startDate,
      endDate: item.endDate,
      reason: item.reason,
    });
    setFormOpen(true);
    setMessage(null);
  }

  function closeForm() {
    setFormOpen(false);
    setEditing(null);
  }

  async function handleSubmit() {
    setPending(true);
    setMessage(null);
    try {
      if (editing) {
        const updated = await updateBlackout(editing.id, form);
        setItems((prev) => prev.map((i) => (i.id === editing.id ? updated : i)));
        setMessage(t["admin.operations.blackout_updated"] ?? "Blackout updated.");
      } else {
        const created = await createBlackout(form);
        setItems((prev) => [...prev, created]);
        setMessage(t["admin.operations.blackout_created"] ?? "Blackout created.");
      }
      closeForm();
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Operation failed");
    } finally {
      setPending(false);
    }
  }

  async function handleDelete() {
    if (!deleteTarget) return;
    setPending(true);
    setMessage(null);
    try {
      await deleteBlackout(deleteTarget.id);
      setItems((prev) => prev.filter((i) => i.id !== deleteTarget.id));
      setMessage(t["admin.operations.blackout_deleted"] ?? "Blackout deleted.");
      setDeleteTarget(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  const columns: Column<HotelBlackoutDto>[] = [
    {
      key: "hotelId",
      header: t["admin.label.hotel"] ?? "Hotel",
      render: (item) => hotelMap[item.hotelId] ?? `#${item.hotelId}`,
    },
    {
      key: "startDate",
      header: t["admin.label.start_date"] ?? "Start Date",
    },
    {
      key: "endDate",
      header: t["admin.label.end_date"] ?? "End Date",
    },
    {
      key: "reason",
      header: t["admin.label.reason"] ?? "Reason",
      render: (item) => item.reason ?? "-",
    },
  ];

  return (
    <>
      <ToastMessage message={message} onDismiss={() => setMessage(null)} />

      <SectionHeader
        title={t["admin.operations.tab.blackouts"] ?? "Blackouts"}
        description={t["admin.operations.blackouts.desc"] ?? "Hotel blackout periods where booking is unavailable."}
        action={
          canEdit ? (
            <button
              onClick={openCreate}
              className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
            >
              {t["admin.action.add"] ?? "Add"} {t["admin.label.blackout"] ?? "Blackout"}
            </button>
          ) : undefined
        }
      />

      <DataTable
        columns={columns}
        data={items}
        keyField="id"
        emptyMessage={t["admin.operations.no_blackouts"] ?? "No blackout periods found."}
        actions={
          canEdit
            ? (item) => (
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
              )
            : undefined
        }
      />

      <FormModal
        open={formOpen}
        title={
          editing
            ? t["admin.operations.edit_blackout"] ?? "Edit Blackout"
            : t["admin.operations.add_blackout"] ?? "Add Blackout"
        }
        onClose={closeForm}
        onSubmit={handleSubmit}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.label.hotel"] ?? "Hotel"}
          type="select"
          value={form.hotelId || ""}
          onChange={(v) => setForm({ ...form, hotelId: Number(v) })}
          options={hotelOptions}
        />
        <FormField
          label={t["admin.label.start_date"] ?? "Start Date"}
          type="date"
          value={form.startDate}
          onChange={(v) => setForm({ ...form, startDate: String(v) })}
        />
        <FormField
          label={t["admin.label.end_date"] ?? "End Date"}
          type="date"
          value={form.endDate}
          onChange={(v) => setForm({ ...form, endDate: String(v) })}
        />
        <FormField
          label={t["admin.label.reason"] ?? "Reason"}
          type="textarea"
          value={form.reason ?? ""}
          onChange={(v) => setForm({ ...form, reason: String(v) || null })}
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleteTarget}
        title={t["admin.action.confirm_delete"] ?? "Confirm Delete"}
        message={
          t["admin.operations.confirm_delete_blackout"] ??
          "Are you sure you want to delete this blackout period?"
        }
        onConfirm={handleDelete}
        onCancel={() => setDeleteTarget(null)}
        loading={pending}
        confirmLabel={t["admin.action.delete"] ?? "Delete"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      />
    </>
  );
}
