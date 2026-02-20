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
  createAdminDivision,
  updateAdminDivision,
  deleteAdminDivision,
  toggleAdminDivisionEnabled,
} from "./actions";
import type {
  AdministrativeDivisionDto,
  AdministrativeDivisionTypeDto,
  CountryDto,
} from "@/types/system";

interface Props {
  divisions: AdministrativeDivisionDto[];
  countries: CountryDto[];
  divisionTypes: AdministrativeDivisionTypeDto[];
  t: Record<string, string>;
}

const emptyForm = {
  countryId: 0,
  typeId: 0,
  parentId: "" as string | number,
  name: "",
  level: 1,
};

export function AdminDivisionsTable({
  divisions: initial,
  countries,
  divisionTypes,
  t,
}: Props) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<AdministrativeDivisionDto | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [modalOpen, setModalOpen] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<AdministrativeDivisionDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const dismissMessage = useCallback(() => setMessage(null), []);

  const countryOptions = countries.map((c) => ({
    value: c.id,
    label: `${c.isoCode} - ${c.name}`,
  }));

  const typeOptions = divisionTypes.map((dt) => ({
    value: dt.id,
    label: dt.name,
  }));

  const parentOptions = items.map((d) => ({
    value: d.id,
    label: d.name,
  }));

  function countryLabel(countryId: number) {
    const c = countries.find((x) => x.id === countryId);
    return c ? c.name : String(countryId);
  }

  function typeLabel(typeId: number) {
    const dt = divisionTypes.find((x) => x.id === typeId);
    return dt ? dt.name : String(typeId);
  }

  function parentLabel(parentId: number | null) {
    if (!parentId) return "-";
    const d = items.find((x) => x.id === parentId);
    return d ? d.name : String(parentId);
  }

  function openCreate() {
    setEditing(null);
    setForm(emptyForm);
    setModalOpen(true);
    setMessage(null);
  }

  function openEdit(item: AdministrativeDivisionDto) {
    setEditing(item);
    setForm({
      countryId: item.countryId,
      typeId: item.typeId,
      parentId: item.parentId ?? "",
      name: item.name,
      level: item.level,
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
        countryId: Number(form.countryId),
        typeId: Number(form.typeId),
        parentId: form.parentId ? Number(form.parentId) : null,
        name: form.name,
        level: Number(form.level),
      };
      if (editing) {
        const updated = await updateAdminDivision(editing.id, payload);
        setItems((prev) => prev.map((i) => (i.id === editing.id ? updated : i)));
        setMessage(t["admin.label.updated"] ?? "Updated successfully.");
      } else {
        const created = await createAdminDivision(payload);
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
      await deleteAdminDivision(deleteTarget.id);
      setItems((prev) => prev.filter((i) => i.id !== deleteTarget.id));
      setMessage(t["admin.label.deleted"] ?? "Deleted successfully.");
      setDeleteTarget(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  async function handleToggle(item: AdministrativeDivisionDto) {
    setPending(true);
    setMessage(null);
    try {
      const newEnabled = !item.enabled;
      await toggleAdminDivisionEnabled(item.id, newEnabled);
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

  const columns: Column<AdministrativeDivisionDto>[] = [
    {
      key: "name",
      header: t["admin.label.name"] ?? "Name",
    },
    {
      key: "countryId",
      header: t["admin.admin_divisions.col.country"] ?? "Country",
      render: (item) => countryLabel(item.countryId),
    },
    {
      key: "typeId",
      header: t["admin.admin_divisions.col.type"] ?? "Type",
      render: (item) => typeLabel(item.typeId),
    },
    {
      key: "parentId",
      header: t["admin.admin_divisions.col.parent"] ?? "Parent",
      render: (item) => parentLabel(item.parentId),
    },
    {
      key: "level",
      header: t["admin.admin_divisions.col.level"] ?? "Level",
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
        emptyMessage={t["admin.admin_divisions.empty"] ?? "No divisions found."}
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
        title={editing ? (t["admin.admin_divisions.edit"] ?? "Edit Division") : (t["admin.admin_divisions.add"] ?? "Add Division")}
        onClose={closeModal}
        onSubmit={handleSave}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.admin_divisions.col.country"] ?? "Country"}
          type="select"
          value={form.countryId}
          onChange={(v) => setForm({ ...form, countryId: Number(v) })}
          options={countryOptions}
          required
        />
        <FormField
          label={t["admin.admin_divisions.col.type"] ?? "Type"}
          type="select"
          value={form.typeId}
          onChange={(v) => setForm({ ...form, typeId: Number(v) })}
          options={typeOptions}
          required
        />
        <FormField
          label={t["admin.admin_divisions.col.parent"] ?? "Parent Division"}
          type="select"
          value={form.parentId}
          onChange={(v) => setForm({ ...form, parentId: String(v) })}
          options={parentOptions}
          placeholder={t["admin.label.none"] ?? "None (top level)"}
        />
        <FormField
          label={t["admin.label.name"] ?? "Name"}
          type="text"
          value={form.name}
          onChange={(v) => setForm({ ...form, name: String(v) })}
          required
        />
        <FormField
          label={t["admin.admin_divisions.col.level"] ?? "Level"}
          type="number"
          value={form.level}
          onChange={(v) => setForm({ ...form, level: Number(v) })}
          required
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleteTarget}
        title={t["admin.confirm.delete_title"] ?? "Delete Division"}
        message={t["admin.confirm.delete_message"] ?? "Are you sure you want to delete this division?"}
        onConfirm={handleDelete}
        onCancel={() => setDeleteTarget(null)}
        loading={pending}
        confirmLabel={t["admin.action.delete"] ?? "Delete"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      />
    </>
  );
}
