"use client";

import { Fragment, useState } from "react";
import type { AuditLogDto } from "@/types/audit";

function formatJson(value: string | null): string {
  if (!value) return "\u2014";
  try {
    return JSON.stringify(JSON.parse(value), null, 2);
  } catch {
    return value;
  }
}

export function AuditTable({
  logs,
  t,
}: {
  logs: AuditLogDto[];
  t: Record<string, string>;
}) {
  const [expandedId, setExpandedId] = useState<number | null>(null);

  function toggleExpand(id: number) {
    setExpandedId((prev) => (prev === id ? null : id));
  }

  if (logs.length === 0) {
    return (
      <div className="rounded-lg border border-gray-200 bg-white p-8 text-center text-sm text-gray-500">
        {t["admin.audit.empty"] ?? "No audit log entries found."}
      </div>
    );
  }

  return (
    <div className="overflow-hidden rounded-lg border border-gray-200 bg-white">
      <table className="w-full text-left text-sm">
        <thead className="border-b border-gray-200 bg-gray-50 text-xs font-medium uppercase text-gray-500">
          <tr>
            <th className="w-10 px-4 py-3"></th>
            <th className="px-4 py-3">{t["admin.field.id"] ?? "ID"}</th>
            <th className="px-4 py-3">{t["admin.field.entityType"] ?? "Entity Type"}</th>
            <th className="px-4 py-3">{t["admin.field.entityId"] ?? "Entity ID"}</th>
            <th className="px-4 py-3">{t["admin.field.action"] ?? "Action"}</th>
            <th className="px-4 py-3">{t["admin.field.userEmail"] ?? "User"}</th>
            <th className="px-4 py-3">{t["admin.field.createdAt"] ?? "Created"}</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-100">
          {logs.map((log) => {
            const isExpanded = expandedId === log.id;
            const hasDetails = log.oldValues || log.newValues;

            return (
              <Fragment key={log.id}>
                <tr className="hover:bg-gray-50">
                  <td className="px-4 py-3">
                    {hasDetails && (
                      <button
                        onClick={() => toggleExpand(log.id)}
                        className="rounded p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-600"
                        title={isExpanded ? "Collapse" : "Expand"}
                      >
                        <svg
                          className={`h-4 w-4 transition-transform ${isExpanded ? "rotate-90" : ""}`}
                          fill="none"
                          viewBox="0 0 24 24"
                          stroke="currentColor"
                          strokeWidth={2}
                        >
                          <path strokeLinecap="round" strokeLinejoin="round" d="M9 5l7 7-7 7" />
                        </svg>
                      </button>
                    )}
                  </td>
                  <td className="px-4 py-3 text-gray-500">{log.id}</td>
                  <td className="px-4 py-3">
                    <code className="rounded bg-gray-100 px-2 py-0.5 text-xs">
                      {log.entityType}
                    </code>
                  </td>
                  <td className="px-4 py-3 text-gray-500">{log.entityId}</td>
                  <td className="px-4 py-3">
                    <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${
                      log.action === "Create"
                        ? "bg-green-100 text-green-700"
                        : log.action === "Delete"
                          ? "bg-red-100 text-red-700"
                          : "bg-blue-100 text-blue-700"
                    }`}>
                      {log.action}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-gray-500">{log.userEmail ?? "\u2014"}</td>
                  <td className="px-4 py-3 text-xs text-gray-500">
                    {new Date(log.createdAt).toLocaleString()}
                  </td>
                </tr>
                {isExpanded && hasDetails && (
                  <tr>
                    <td colSpan={7} className="bg-gray-50 px-4 py-4">
                      <div className="grid grid-cols-2 gap-4">
                        <div>
                          <p className="mb-1 text-xs font-medium uppercase text-gray-500">
                            {t["admin.audit.oldValues"] ?? "Old Values"}
                          </p>
                          <pre className="max-h-60 overflow-auto rounded border border-gray-200 bg-white p-3 text-xs text-gray-700">
                            {formatJson(log.oldValues)}
                          </pre>
                        </div>
                        <div>
                          <p className="mb-1 text-xs font-medium uppercase text-gray-500">
                            {t["admin.audit.newValues"] ?? "New Values"}
                          </p>
                          <pre className="max-h-60 overflow-auto rounded border border-gray-200 bg-white p-3 text-xs text-gray-700">
                            {formatJson(log.newValues)}
                          </pre>
                        </div>
                      </div>
                    </td>
                  </tr>
                )}
              </Fragment>
            );
          })}
        </tbody>
      </table>
    </div>
  );
}
