"use client";

import { useState, useCallback } from "react";
import {
  TaxDto,
  TaxTypeDto,
  CountryDto,
  AdministrativeDivisionDto,
  CityDto,
} from "@/types/system";
import { DataTable, Column } from "@/components/data-table";
import { FormModal } from "@/components/form-modal";
import { FormField } from "@/components/form-field";
import { ConfirmDialog } from "@/components/confirm-dialog";
import { StatusBadge } from "@/components/status-badge";
import { ToastMessage } from "@/components/toast-message";
import { SectionHeader } from "@/components/section-header";
import { createTax, updateTax, deleteTax, toggleTaxEnabled } from "./actions";

interface TaxesTabProps {
  taxes: TaxDto[];
  taxTypes: TaxTypeDto[];
  countries: CountryDto[];
  divisions: AdministrativeDivisionDto[];
  cities: CityDto[];
  t: Record<string, string>;
}

export function TaxesTab({
  taxes: initial,
  taxTypes,
  countries,
  divisions,
  cities,
  t,
}: TaxesTabProps) {
  const [items, setItems] = useState(initial);
  const [editing, setEditing] = useState<TaxDto | null>(null);
  const [deleting, setDeleting] = useState<TaxDto | null>(null);
  const [pending, setPending] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [showCreate, setShowCreate] = useState(false);

  const [formTaxTypeId, setFormTaxTypeId] = useState<number | string>("");
  const [formCountryId, setFormCountryId] = useState<number | string>("");
  const [formDivisionId, setFormDivisionId] = useState<number | string>("");
  const [formCityId, setFormCityId] = useState<number | string>("");
  const [formRate, setFormRate] = useState<number>(0);
  const [formIsPercentage, setFormIsPercentage] = useState(true);

  const dismissMessage = useCallback(() => setMessage(null), []);

  function buildPayload() {
    return {
      taxTypeId: Number(formTaxTypeId),
      countryId: formCountryId ? Number(formCountryId) : null,
      administrativeDivisionId: formDivisionId ? Number(formDivisionId) : null,
      cityId: formCityId ? Number(formCityId) : null,
      rate: formRate,
      isPercentage: formIsPercentage,
    };
  }

  function openCreate() {
    setFormTaxTypeId("");
    setFormCountryId("");
    setFormDivisionId("");
    setFormCityId("");
    setFormRate(0);
    setFormIsPercentage(true);
    setShowCreate(true);
    setMessage(null);
  }

  function openEdit(item: TaxDto) {
    setFormTaxTypeId(item.taxTypeId);
    setFormCountryId(item.countryId ?? "");
    setFormDivisionId(item.administrativeDivisionId ?? "");
    setFormCityId(item.cityId ?? "");
    setFormRate(item.rate);
    setFormIsPercentage(item.isPercentage);
    setEditing(item);
    setMessage(null);
  }

  async function handleSave() {
    setPending(true);
    setMessage(null);
    try {
      const payload = buildPayload();
      if (editing) {
        const updated = await updateTax(editing.id, payload);
        setItems((prev) => prev.map((i) => (i.id === editing.id ? updated : i)));
        setEditing(null);
        setMessage(t["admin.toast.updated"] ?? "Updated successfully.");
      } else {
        const created = await createTax(payload);
        setItems((prev) => [...prev, created]);
        setShowCreate(false);
        setMessage(t["admin.toast.created"] ?? "Created successfully.");
      }
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Save failed");
    } finally {
      setPending(false);
    }
  }

  async function handleDelete() {
    if (!deleting) return;
    setPending(true);
    setMessage(null);
    try {
      await deleteTax(deleting.id);
      setItems((prev) => prev.filter((i) => i.id !== deleting.id));
      setDeleting(null);
      setMessage(t["admin.toast.deleted"] ?? "Deleted successfully.");
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Delete failed");
    } finally {
      setPending(false);
    }
  }

  async function handleToggle(item: TaxDto) {
    setPending(true);
    setMessage(null);
    try {
      const updated = await toggleTaxEnabled(item.id);
      setItems((prev) => prev.map((i) => (i.id === item.id ? updated : i)));
      const label = updated.enabled
        ? t["admin.label.enabled"] ?? "Enabled"
        : t["admin.label.disabled"] ?? "Disabled";
      setMessage(`Tax #${item.id} ${label.toLowerCase()}.`);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Toggle failed");
    } finally {
      setPending(false);
    }
  }

  function lookupName(list: { id: number; name: string }[], id: number | null) {
    if (id === null) return "-";
    return list.find((x) => x.id === id)?.name ?? String(id);
  }

  const columns: Column<TaxDto>[] = [
    { key: "id", header: "ID", className: "w-16" },
    {
      key: "taxTypeId",
      header: t["admin.label.tax_type"] ?? "Tax Type",
      render: (item) => lookupName(taxTypes, item.taxTypeId),
    },
    {
      key: "countryId",
      header: t["admin.label.country"] ?? "Country",
      render: (item) => lookupName(countries, item.countryId),
    },
    {
      key: "rate",
      header: t["admin.label.rate"] ?? "Rate",
      render: (item) =>
        item.isPercentage ? `${item.rate}%` : item.rate.toFixed(2),
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

  const taxTypeOptions = taxTypes.map((tt) => ({ value: tt.id, label: tt.name }));
  const countryOptions = countries.map((c) => ({ value: c.id, label: c.name }));
  const divisionOptions = divisions.map((d) => ({ value: d.id, label: d.name }));
  const cityOptions = cities.map((c) => ({ value: c.id, label: c.name }));

  return (
    <>
      <ToastMessage message={message} onDismiss={dismissMessage} />

      <SectionHeader
        title={t["admin.taxes.taxes"] ?? "Taxes"}
        description={t["admin.taxes.taxes.desc"] ?? "Manage tax rates by type and geography."}
        action={
          <button
            onClick={openCreate}
            className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
          >
            {t["admin.action.create"] ?? "Create"}
          </button>
        }
      />

      <DataTable
        columns={columns}
        data={items}
        keyField="id"
        emptyMessage={t["admin.label.no_data"] ?? "No taxes found."}
        actions={(item) => (
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
        )}
      />

      <FormModal
        open={showCreate || !!editing}
        title={
          editing
            ? t["admin.taxes.edit"] ?? "Edit Tax"
            : t["admin.taxes.create"] ?? "Create Tax"
        }
        onClose={() => { setShowCreate(false); setEditing(null); }}
        onSubmit={handleSave}
        loading={pending}
        saveLabel={t["admin.action.save"] ?? "Save"}
        cancelLabel={t["admin.action.cancel"] ?? "Cancel"}
      >
        <FormField
          label={t["admin.label.tax_type"] ?? "Tax Type"}
          type="select"
          value={formTaxTypeId}
          onChange={(v) => setFormTaxTypeId(v as number)}
          options={taxTypeOptions}
          required
        />
        <FormField
          label={t["admin.label.country"] ?? "Country"}
          type="select"
          value={formCountryId}
          onChange={(v) => setFormCountryId(v as number)}
          options={countryOptions}
          placeholder={t["admin.label.none"] ?? "None (global)"}
        />
        <FormField
          label={t["admin.label.administrative_division"] ?? "Administrative Division"}
          type="select"
          value={formDivisionId}
          onChange={(v) => setFormDivisionId(v as number)}
          options={divisionOptions}
          placeholder={t["admin.label.none"] ?? "None"}
        />
        <FormField
          label={t["admin.label.city"] ?? "City"}
          type="select"
          value={formCityId}
          onChange={(v) => setFormCityId(v as number)}
          options={cityOptions}
          placeholder={t["admin.label.none"] ?? "None"}
        />
        <FormField
          label={t["admin.label.rate"] ?? "Rate"}
          type="number"
          value={formRate}
          onChange={(v) => setFormRate(Number(v))}
          step="0.01"
          required
        />
        <FormField
          label={t["admin.label.is_percentage"] ?? "Is percentage"}
          type="checkbox"
          value={formIsPercentage}
          onChange={(v) => setFormIsPercentage(Boolean(v))}
        />
      </FormModal>

      <ConfirmDialog
        open={!!deleting}
        title={t["admin.confirm.delete_title"] ?? "Confirm Delete"}
        message={
          (t["admin.confirm.delete_message"] ?? 'Delete tax #{id}?').replace(
            "{id}",
            String(deleting?.id ?? "")
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
