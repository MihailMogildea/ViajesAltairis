"use client";

import { useState } from "react";
import { updateInvoiceStatus } from "./actions";
import { DataTable, type Column } from "@/components/data-table";
import { ToastMessage } from "@/components/toast-message";
import type { AccessLevel } from "@/lib/permissions";
import type { InvoiceDto, InvoiceStatusDto } from "@/types/invoice";

interface InvoicesTabProps {
  invoices: InvoiceDto[];
  statuses: InvoiceStatusDto[];
  isNames: Record<number, string>;
  access: AccessLevel;
  t: Record<string, string>;
}

export function InvoicesTab({
  invoices: initial,
  statuses,
  isNames,
  access,
  t,
}: InvoicesTabProps) {
  const [items, setItems] = useState(initial);
  const [pending, setPending] = useState<number | null>(null);
  const [message, setMessage] = useState<string | null>(null);

  const statusMap = new Map(statuses.map((s) => [s.id, s.name]));

  async function handleStatusChange(invoice: InvoiceDto, newStatusId: number) {
    if (newStatusId === invoice.statusId) return;
    setPending(invoice.id);
    setMessage(null);
    try {
      await updateInvoiceStatus(invoice.id, newStatusId);
      setItems((prev) =>
        prev.map((i) =>
          i.id === invoice.id ? { ...i, statusId: newStatusId } : i
        )
      );
      setMessage(t["admin.message.updated"] ?? "Updated successfully.");
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Status update failed");
    } finally {
      setPending(null);
    }
  }

  const columns: Column<InvoiceDto>[] = [
    {
      key: "invoiceNumber",
      header: t["admin.field.invoiceNumber"] ?? "Invoice #",
      render: (item) => (
        <span className="font-medium text-gray-900">{item.invoiceNumber}</span>
      ),
    },
    {
      key: "statusId",
      header: t["admin.field.status"] ?? "Status",
      render: (item) => (
        <span className="text-sm">{isNames[item.statusId] ?? statusMap.get(item.statusId) ?? item.statusId}</span>
      ),
    },
    {
      key: "reservationId",
      header: t["admin.field.reservationId"] ?? "Reservation",
    },
    {
      key: "totalAmount",
      header: t["admin.field.totalAmount"] ?? "Total",
      render: (item) => (
        <span className="font-medium">{item.totalAmount.toFixed(2)}</span>
      ),
      className: "text-right",
    },
    {
      key: "periodStart",
      header: t["admin.field.periodStart"] ?? "Period Start",
      render: (item) => (
        <span className="text-xs text-gray-500">
          {new Date(item.periodStart).toLocaleDateString()}
        </span>
      ),
    },
    {
      key: "periodEnd",
      header: t["admin.field.periodEnd"] ?? "Period End",
      render: (item) => (
        <span className="text-xs text-gray-500">
          {new Date(item.periodEnd).toLocaleDateString()}
        </span>
      ),
    },
  ];

  const canChangeStatus = access === "full";

  return (
    <>
      <ToastMessage message={message} onDismiss={() => setMessage(null)} />

      <DataTable
        columns={columns}
        data={items}
        keyField="id"
        emptyMessage={t["admin.financial.invoices.empty"] ?? "No invoices found."}
        actions={
          canChangeStatus
            ? (item) => (
                <select
                  value={item.statusId}
                  onChange={(e) =>
                    handleStatusChange(item, Number(e.target.value))
                  }
                  disabled={pending === item.id}
                  className="rounded border border-gray-300 px-2 py-1 text-xs focus:border-blue-500 focus:outline-none disabled:opacity-50"
                >
                  {statuses.map((s) => (
                    <option key={s.id} value={s.id}>
                      {isNames[s.id] ?? s.name}
                    </option>
                  ))}
                </select>
              )
            : undefined
        }
      />
    </>
  );
}
