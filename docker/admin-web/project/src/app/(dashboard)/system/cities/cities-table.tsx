"use client";

import { useState, useCallback } from "react";
import { DataTable, type Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import { StatusBadge } from "@/components/status-badge";
import {
  createCity,
  updateCity,
  deleteCity,
  toggleCityEnabled,
} from "./actions";
import type { CityDto, AdministrativeDivisionDto } from "@/types/system";

interface Props {
  cities: CityDto[];
  divisions: AdministrativeDivisionDto[];
  t: Record<string, string>;
}

const emptyForm = { administrativeDivisionId: 0, name: "" };

export function CitiesTable({ cities: initial, divisions, t }: Props) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<CityDto | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [modalOpen, setModalOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<CityDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const dismissMessage = useCallback(() => setMessage(null), []);

  const divisionOptions = divisions.map((d) => ({
    value: d.id,
    label: d.name,
  }));

  function divisionLabel(divisionId: number) {
    const d = divisions.find((x) => x.id === divisionId);
    return d ? d.name : String(divisionId);
  }

  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    setModalOpen(true);
    setMessage(null);
  }

  function openEdit(item: CityDto) {
    setEditing(item);
    setForm({
      administrativeDivisionId: item.administrativeDivisionId,
      name: item.name,
    });
    setModalOpen(true);
    setMessage(null);
  }

  function closeModal() {
    setModalOpen(false);
    setEditing(null);
  }

  async function handleSave() {
    setPending(true);
    setMessage(null);
    try {
      const payload = {
        administrativeDivisionId: Number(form.administrativeDivisionId),
        name: form.name,
      };
      if (editing) {
        const updated = await updateCity(editing.id, payload);
        setItems((prev) => prev.map((i) => (i.id === editing.id ? updated : i)));
        setMessage(t["admin.label.updated"] ?? "Updated successfully.");
      } else {
        const created = await createCity(payload);
        setItems((prev) => [...prev, created]);
        setMessage(t["admin.label.created"] ?? "Created successfully.");
      }
      closeModal();
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Save failed");
    } finally {
      setPending(false);
    }
  }

  async function handleDelete() {
    if (!deleteTarget) return;
    setPending(true);
    setMessage(null);
    try {
      await deleteCity(deleteTarget.id);
      setItems((prev) => prev.filter((i) => i.id !== deleteTarget.id));
      setMessage(t["admin.label.deleted"] ?? "Deleted successfully.");
      setDeleteTarget(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  async function handleToggle(item: CityDto) {
    setPending(true);
    setMessage(null);
    try {
      const newEnabled = !item.enabled;
      await toggleCityEnabled(item.id, newEnabled);
      setItems((prev) =>
        prev.map((i) => (i.id === item.id ? { ...i, enabled: newEnabled } : i))
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

  const columns: Column<CityDto>[] = [
    {
      key: "name",
      header: t["admin.label.name"] ?? "Name",
    },
    {
      key: "administrativeDivisionId",
      header: t["admin.cities.col.division"] ?? "Admin Division",
      render: (item) => divisionLabel(item.administrativeDivisionId),
    },
    {
      key: "enabled",
      header: t["admin.label.status"] ?? "Status",
      render: (item) => (
        <StatusBadge
          variant={item.enabled ? "enabled" : "disabled"}
          onClick={() => handleToggle(item)}
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
      header: t["admin.label.created_at"] ?? "Created",
      render: (item) => new Date(item.createdAt).toLocaleDateString(),
    },
  ];

  return (
    <>
      <ToastMessage message={message} onDismiss={dismissMessage} />

      <SectionHeader
        title=""
        action={
          <button
            onClick={openCreate}
            className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
          >
            {t["admin.action.add"] ?? "Add"}
          </button>
        }
      />

      <DataTable
        columns={columns}
        data={items}
        keyField="id"
        emptyMessage={t["admin.cities.empty"] ?? "No cities found."}
        actions={(item) => (
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
        )}
      />

      <FormModal
        open={modalOpen}
        title={editing ? (t["admin.cities.edit"] ?? "Edit City") : (t["admin.cities.add"] ?? "Add City")}
        onClose={closeModal}
        onSubmit={handleSave}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.cities.col.division"] ?? "Administrative Division"}
          type="select"
          value={form.administrativeDivisionId}
          onChange={(v) => setForm({ ...form, administrativeDivisionId: Number(v) })}
          options={divisionOptions}
          required
        />
        <FormField
          label={t["admin.label.name"] ?? "Name"}
          type="text"
          value={form.name}
          onChange={(v) => setForm({ ...form, name: String(v) })}
          required
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleteTarget}
        title={t["admin.confirm.delete_title"] ?? "Delete City"}
        message={t["admin.confirm.delete_message"] ?? "Are you sure you want to delete this city?"}
        onConfirm={handleDelete}
        onCancel={() => setDeleteTarget(null)}
        loading={pending}
        confirmLabel={t["admin.action.delete"] ?? "Delete"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      />
    </>
  );
}
