"use client";

import { useState } from "react";
import { DataTable, type Column } from "@/components/data-table";
import { SectionHeader } from "@/components/section-header";
import type { CancellationDto } from "@/types/reservation";

interface CancellationsTabProps {
  cancellations: CancellationDto[];
  t: Record<string, string>;
}

export function CancellationsTab({ cancellations: initial, t }: CancellationsTabProps) {
  const [items] = useState(initial);

  function formatDate(dateStr: string) {
    return new Date(dateStr).toLocaleString();
  }

  function formatCurrency(amount: number) {
    return amount.toFixed(2);
  }

  const columns: Column<CancellationDto>[] = [
    {
      key: "reservationId",
      header: t["admin.label.reservation_id"] ?? "Reservation",
      render: (item) => `#${item.reservationId}`,
    },
    {
      key: "cancelledByUserId",
      header: t["admin.label.cancelled_by"] ?? "Cancelled By",
      render: (item) => `User #${item.cancelledByUserId}`,
    },
    {
      key: "reason",
      header: t["admin.label.reason"] ?? "Reason",
      render: (item) => item.reason ?? "-",
    },
    {
      key: "penaltyPercentage",
      header: t["admin.label.penalty_pct"] ?? "Penalty %",
      render: (item) => `${item.penaltyPercentage}%`,
      className: "text-right",
    },
    {
      key: "penaltyAmount",
      header: t["admin.label.penalty_amount"] ?? "Penalty",
      render: (item) => formatCurrency(item.penaltyAmount),
      className: "text-right",
    },
    {
      key: "refundAmount",
      header: t["admin.label.refund_amount"] ?? "Refund",
      render: (item) => formatCurrency(item.refundAmount),
      className: "text-right",
    },
    {
      key: "createdAt",
      header: t["admin.label.date"] ?? "Date",
      render: (item) => formatDate(item.createdAt),
    },
  ];

  return (
    <>
      <SectionHeader
        title={t["admin.operations.tab.cancellations"] ?? "Cancellations"}
        description={t["admin.operations.cancellations.desc"] ?? "Read-only log of reservation cancellations."}
      />

      <DataTable
        columns={columns}
        data={items}
        keyField="id"
        emptyMessage={t["admin.operations.no_cancellations"] ?? "No cancellations found."}
      />
    </>
  );
}
