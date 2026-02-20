import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { Breadcrumb } from "@/components/breadcrumb";
import { NotificationLogDto } from "@/types/system";
import { NotificationsTable } from "./notifications-table";

export default async function NotificationsPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let notifications: NotificationLogDto[] = [];
  let error: string | null = null;

  try {
    notifications = await apiFetch<NotificationLogDto[]>("/api/NotificationLogs", {
      cache: "no-store",
    });
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load notifications";
  }

  return (
    <div>
      <Breadcrumb
        items={[
          { label: t["admin.section.system"] ?? "System Config", href: "/system" },
          { label: t["admin.system.notifications"] ?? "Notification Log" },
        ]}
      />

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <NotificationsTable notifications={notifications} t={t} />
      )}
    </div>
  );
}
