"use client";

import { useState } from "react";
import {
  createSeasonalMargin,
  updateSeasonalMargin,
  deleteSeasonalMargin,
} from "./actions";
import { DataTable, type Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import type { SeasonalMarginDto } from "@/types/pricing";
import type { AdministrativeDivisionDto } from "@/types/system";
import type { AccessLevel } from "@/lib/permissions";

interface FormState {
  administrativeDivisionId: string;
  startMonthDay: string;
  endMonthDay: string;
  margin: number;
}

const emptyForm: FormState = {
  administrativeDivisionId: "",
  startMonthDay: "",
  endMonthDay: "",
  margin: 0,
};

export function SeasonalMarginsTab({
  seasonalMargins: initial,
  divisions,
  access,
  t,
}: {
  seasonalMargins: SeasonalMarginDto[];
  divisions: AdministrativeDivisionDto[];
  access: AccessLevel | null;
  t: Record<string, string>;
}) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<SeasonalMarginDto | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [form, setForm] = useState<FormState>(emptyForm);
  const [deleting, setDeleting] = useState<SeasonalMarginDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const isFull = access === "full";

  const divisionMap = new Map(divisions.map((d) => [d.id, d.name]));
  const divisionOptions = divisions.map((d) => ({
    value: d.id,
    label: d.name,
  }));

  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    setModalOpen(true);
  }

  function openEdit(item: SeasonalMarginDto) {
    setEditing(item);
    setForm({
      administrativeDivisionId: String(item.administrativeDivisionId),
      startMonthDay: item.startMonthDay,
      endMonthDay: item.endMonthDay,
      margin: item.margin,
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
        administrativeDivisionId: Number(form.administrativeDivisionId),
        startMonthDay: form.startMonthDay,
        endMonthDay: form.endMonthDay,
        margin: form.margin,
      };
      if (editing) {
        const updated = await updateSeasonalMargin(editing.id, payload);
        setItems((prev) =>
          prev.map((i) => (i.id === editing.id ? { ...i, ...updated } : i))
        );
        setMessage(t["admin.message.updated"] ?? "Updated successfully.");
      } else {
        const created = await createSeasonalMargin(payload);
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
      await deleteSeasonalMargin(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      setMessage(t["admin.message.deleted"] ?? "Deleted successfully.");
      setDeleting(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  const columns: Column<SeasonalMarginDto>[] = [
    {
      key: "administrativeDivisionId",
      header: t["admin.field.division"] ?? "Division",
      render: (item) => divisionMap.get(item.administrativeDivisionId) ?? String(item.administrativeDivisionId),
    },
    {
      key: "startMonthDay",
      header: t["admin.field.startMonthDay"] ?? "Start (MM-DD)",
    },
    {
      key: "endMonthDay",
      header: t["admin.field.endMonthDay"] ?? "End (MM-DD)",
    },
    {
      key: "margin",
      header: t["admin.field.margin"] ?? "Margin",
      render: (item) => `${item.margin}%`,
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
        emptyMessage={t["admin.pricing.seasonal_margins.empty"] ?? "No seasonal margins found."}
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
            ? t["admin.pricing.seasonal_margins.edit"] ?? "Edit Seasonal Margin"
            : t["admin.pricing.seasonal_margins.create"] ?? "Create Seasonal Margin"
        }
        onClose={closeModal}
        onSubmit={handleSubmit}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.field.division"] ?? "Division"}
          type="select"
          value={form.administrativeDivisionId}
          onChange={(v) =>
            setForm((f) => ({ ...f, administrativeDivisionId: String(v) }))
          }
          options={divisionOptions}
          required
        />
        <FormField
          label={t["admin.field.startMonthDay"] ?? "Start (MM-DD)"}
          value={form.startMonthDay}
          onChange={(v) => setForm((f) => ({ ...f, startMonthDay: String(v) }))}
          placeholder="01-15"
          required
        />
        <FormField
          label={t["admin.field.endMonthDay"] ?? "End (MM-DD)"}
          value={form.endMonthDay}
          onChange={(v) => setForm((f) => ({ ...f, endMonthDay: String(v) }))}
          placeholder="03-31"
          required
        />
        <FormField
          label={t["admin.field.margin"] ?? "Margin (%)"}
          type="number"
          value={form.margin}
          onChange={(v) => setForm((f) => ({ ...f, margin: Number(v) }))}
          step="0.01"
          required
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleting}
        title={t["admin.confirm.delete.title"] ?? "Confirm Delete"}
        message={
          (t["admin.confirm.delete.message"] ?? "Are you sure you want to delete this seasonal margin?")
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
