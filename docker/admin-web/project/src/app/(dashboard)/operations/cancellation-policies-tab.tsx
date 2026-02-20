"use client";

import { useState, useMemo } from "react";
import { DataTable, type Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { ToastMessage } from "@/components/toast-message";
import { StatusBadge } from "@/components/status-badge";
import { SectionHeader } from "@/components/section-header";
import {
  createCancellationPolicy,
  updateCancellationPolicy,
  deleteCancellationPolicy,
  toggleCancellationPolicyEnabled,
} from "./actions";
import type { HotelDto } from "@/types/hotel";
import type { CancellationPolicyDto } from "@/types/reservation";
import type { AccessLevel } from "@/lib/permissions";

interface CancellationPoliciesTabProps {
  policies: CancellationPolicyDto[];
  hotels: HotelDto[];
  access: AccessLevel;
  t: Record<string, string>;
}

interface PolicyForm {
  hotelId: number;
  freeCancellationHours: number;
  penaltyPercentage: number;
}

const emptyForm: PolicyForm = {
  hotelId: 0,
  freeCancellationHours: 0,
  penaltyPercentage: 0,
};

export function CancellationPoliciesTab({
  policies: initial,
  hotels,
  access,
  t,
}: CancellationPoliciesTabProps) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<CancellationPolicyDto | null>(null);
  const [formOpen, setFormOpen] = useState(false);
  const [form, setForm] = useState<PolicyForm>(emptyForm);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<CancellationPolicyDto | null>(null);

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

  function openEdit(item: CancellationPolicyDto) {
    setEditing(item);
    setForm({
      hotelId: item.hotelId,
      freeCancellationHours: item.freeCancellationHours,
      penaltyPercentage: item.penaltyPercentage,
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
        const updated = await updateCancellationPolicy(editing.id, form);
        setItems((prev) => prev.map((i) => (i.id === editing.id ? updated : i)));
        setMessage(t["admin.operations.policy_updated"] ?? "Policy updated.");
      } else {
        const created = await createCancellationPolicy(form);
        setItems((prev) => [...prev, created]);
        setMessage(t["admin.operations.policy_created"] ?? "Policy created.");
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
      await deleteCancellationPolicy(deleteTarget.id);
      setItems((prev) => prev.filter((i) => i.id !== deleteTarget.id));
      setMessage(t["admin.operations.policy_deleted"] ?? "Policy deleted.");
      setDeleteTarget(null);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  async function handleToggle(item: CancellationPolicyDto) {
    setPending(true);
    setMessage(null);
    try {
      const updated = await toggleCancellationPolicyEnabled(item.id);
      setItems((prev) => prev.map((i) => (i.id === item.id ? updated : i)));
      const label = updated.enabled
        ? t["admin.label.enabled"] ?? "Enabled"
        : t["admin.label.disabled"] ?? "Disabled";
      setMessage(`${hotelMap[item.hotelId] ?? "Policy"} ${label.toLowerCase()}.`);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Toggle failed");
    } finally {
      setPending(false);
    }
  }

  const columns: Column<CancellationPolicyDto>[] = [
    {
      key: "hotelId",
      header: t["admin.label.hotel"] ?? "Hotel",
      render: (item) => hotelMap[item.hotelId] ?? `#${item.hotelId}`,
    },
    {
      key: "freeCancellationHours",
      header: t["admin.label.free_cancellation_hours"] ?? "Free Cancel (hrs)",
      className: "text-right",
    },
    {
      key: "penaltyPercentage",
      header: t["admin.label.penalty_pct"] ?? "Penalty %",
      render: (item) => `${item.penaltyPercentage}%`,
      className: "text-right",
    },
    {
      key: "enabled",
      header: t["admin.label.status"] ?? "Status",
      render: (item) => (
        <StatusBadge
          variant={item.enabled ? "enabled" : "disabled"}
          onClick={canEdit ? () => handleToggle(item) : undefined}
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

      <SectionHeader
        title={t["admin.operations.tab.policies"] ?? "Cancellation Policies"}
        description={t["admin.operations.policies.desc"] ?? "Per-hotel cancellation terms and penalty rules."}
        action={
          canEdit ? (
            <button
              onClick={openCreate}
              className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
            >
              {t["admin.action.add"] ?? "Add"} {t["admin.label.policy"] ?? "Policy"}
            </button>
          ) : undefined
        }
      />

      <DataTable
        columns={columns}
        data={items}
        keyField="id"
        emptyMessage={t["admin.operations.no_policies"] ?? "No cancellation policies found."}
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
            ? t["admin.operations.edit_policy"] ?? "Edit Cancellation Policy"
            : t["admin.operations.add_policy"] ?? "Add Cancellation Policy"
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
          label={t["admin.label.free_cancellation_hours"] ?? "Free Cancellation Hours"}
          type="number"
          value={form.freeCancellationHours}
          onChange={(v) => setForm({ ...form, freeCancellationHours: Number(v) })}
        />
        <FormField
          label={t["admin.label.penalty_pct"] ?? "Penalty Percentage"}
          type="number"
          value={form.penaltyPercentage}
          onChange={(v) => setForm({ ...form, penaltyPercentage: Number(v) })}
          step="0.01"
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleteTarget}
        title={t["admin.action.confirm_delete"] ?? "Confirm Delete"}
        message={
          t["admin.operations.confirm_delete_policy"] ??
          "Are you sure you want to delete this cancellation policy?"
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
