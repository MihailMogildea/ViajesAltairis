import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { AuditTable } from "./audit-table";
import type { AuditLogDto } from "@/types/audit";

export default async function AuditPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let logs: AuditLogDto[] = [];
  let error: string | null = null;

  try {
    logs = await apiFetch<AuditLogDto[]>("/api/AuditLogs", {
      cache: "no-store",
    });
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load audit logs";
  }

  return (
    <div>
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">
          {t["admin.section.audit"] ?? "Audit Log"}
        </h2>
        <p className="mt-1 text-sm text-gray-500">
          {t["admin.audit.desc"] ?? "View a history of all changes made across the platform."}
        </p>
      </div>

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <AuditTable logs={logs} t={t} />
      )}
    </div>
  );
}
