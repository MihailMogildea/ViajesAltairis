"use client";

import { useState } from "react";
import { NotificationLogDto } from "@/types/system";
import { DataTable, Column } from "@/components/data-table";
import { SectionHeader } from "@/components/section-header";

export function NotificationsTable({
  notifications: initial,
  t,
}: {
  notifications: NotificationLogDto[];
  t: Record<string, string>;
}) {
  const [items] = useState(initial);

  const columns: Column<NotificationLogDto>[] = [
    { key: "id", header: "ID", className: "w-20" },
    {
      key: "recipientEmail",
      header: t["admin.label.recipient"] ?? "Recipient",
    },
    {
      key: "subject",
      header: t["admin.label.subject"] ?? "Subject",
    },
    {
      key: "createdAt",
      header: t["admin.label.created_at"] ?? "Created",
      render: (item) => new Date(item.createdAt).toLocaleString(),
    },
  ];

  return (
    <>
      <SectionHeader
        title={t["admin.system.notifications"] ?? "Notification Log"}
        description={t["admin.system.notifications.desc"] ?? "View sent notification history."}
      />

      <DataTable
        columns={columns}
        data={items}
        keyField="id"
        emptyMessage={t["admin.label.no_data"] ?? "No notifications found."}
      />
    </>
  );
}
