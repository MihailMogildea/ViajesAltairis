import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { Breadcrumb } from "@/components/breadcrumb";
import { SchedulerTable } from "./scheduler-table";

interface JobSchedule {
  id: number;
  jobKey: string;
  name: string;
  cronExpression: string;
  enabled: boolean;
  lastExecutedAt: string | null;
  createdAt: string;
  updatedAt: string;
}

export default async function SchedulerPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let schedules: JobSchedule[] = [];
  let error: string | null = null;

  try {
    schedules = await apiFetch<JobSchedule[]>("/api/job-schedules", {
      cache: "no-store",
    });
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load schedules";
  }

  return (
    <div>
      <Breadcrumb
        items={[
          { label: t["admin.section.system"] ?? "System Config", href: "/system" },
          { label: t["admin.scheduler.title"] ?? "Job Scheduler" },
        ]}
      />
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">
          {t["admin.scheduler.title"] ?? "Job Scheduler"}
        </h2>
        <p className="mt-1 text-sm text-gray-500">
          {t["admin.scheduler.desc"] ?? "Manage scheduled jobs, update cron expressions, and trigger jobs manually."}
        </p>
      </div>

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <SchedulerTable schedules={schedules} t={t} />
      )}
    </div>
  );
}
