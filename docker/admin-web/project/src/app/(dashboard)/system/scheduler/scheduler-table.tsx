"use client";

import { useState, useEffect } from "react";
import { triggerJob, updateSchedule } from "./actions";

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

export function SchedulerTable({
  schedules: initial,
  t,
}: {
  schedules: JobSchedule[];
  t: Record<string, string>;
}) {
  const [schedules, setSchedules] = useState(initial);
  const [editingKey, setEditingKey] = useState<string | null>(null);
  const [editCron, setEditCron] = useState("");
  const [pending, setPending] = useState<string | null>(null);
  const [message, setMessage] = useState<string | null>(null);

  useEffect(() => {
    if (!message) return;
    const timer = setTimeout(() => setMessage(null), 4000);
    return () => clearTimeout(timer);
  }, [message]);

  function startEdit(schedule: JobSchedule) {
    setEditingKey(schedule.jobKey);
    setEditCron(schedule.cronExpression);
    setMessage(null);
  }

  function cancelEdit() {
    setEditingKey(null);
    setEditCron("");
  }

  async function handleSave(schedule: JobSchedule) {
    setPending(schedule.jobKey);
    setMessage(null);
    try {
      const updated = await updateSchedule(schedule.jobKey, {
        cronExpression: editCron,
        enabled: schedule.enabled,
      });
      setSchedules((prev) =>
        prev.map((s) => (s.jobKey === schedule.jobKey ? { ...s, ...updated } : s))
      );
      setEditingKey(null);
      setMessage(t["admin.scheduler.updated"] ?? "Schedule updated.");
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Update failed");
    } finally {
      setPending(null);
    }
  }

  async function handleToggle(schedule: JobSchedule) {
    setPending(schedule.jobKey);
    setMessage(null);
    try {
      const updated = await updateSchedule(schedule.jobKey, {
        cronExpression: schedule.cronExpression,
        enabled: !schedule.enabled,
      });
      setSchedules((prev) =>
        prev.map((s) => (s.jobKey === schedule.jobKey ? { ...s, ...updated } : s))
      );
      const label = !schedule.enabled
        ? t["admin.label.enabled"] ?? "Enabled"
        : t["admin.label.disabled"] ?? "Disabled";
      setMessage(`"${schedule.name}" ${label.toLowerCase()}.`);
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Toggle failed");
    } finally {
      setPending(null);
    }
  }

  async function handleTrigger(schedule: JobSchedule) {
    setPending(schedule.jobKey);
    setMessage(null);
    try {
      await triggerJob(schedule.jobKey);
      setMessage(t["admin.scheduler.triggered"] ?? "Job triggered.");
    } catch (e) {
      setMessage(e instanceof Error ? e.message : "Trigger failed");
    } finally {
      setPending(null);
    }
  }

  function formatDate(dateStr: string | null) {
    if (!dateStr) return t["admin.label.never"] ?? "Never";
    return new Date(dateStr).toLocaleString();
  }

  return (
    <>
      {message && (
        <p className="mb-4 rounded border border-blue-200 bg-blue-50 px-4 py-2 text-sm text-blue-800">
          {message}
        </p>
      )}

      <div className="overflow-hidden rounded-lg border border-gray-200 bg-white">
        <table className="w-full text-left text-sm">
          <thead className="border-b border-gray-200 bg-gray-50 text-xs font-medium uppercase text-gray-500">
            <tr>
              <th className="px-4 py-3">{t["admin.scheduler.col.job"] ?? "Job"}</th>
              <th className="px-4 py-3">{t["admin.scheduler.col.cron"] ?? "Cron"}</th>
              <th className="px-4 py-3">{t["admin.scheduler.col.status"] ?? "Status"}</th>
              <th className="px-4 py-3">{t["admin.scheduler.col.last_executed"] ?? "Last Executed"}</th>
              <th className="px-4 py-3 text-right">{t["admin.scheduler.col.actions"] ?? "Actions"}</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {schedules.map((schedule) => {
              const isEditing = editingKey === schedule.jobKey;
              const isPending = pending === schedule.jobKey;

              return (
                <tr key={schedule.jobKey} className="hover:bg-gray-50">
                  <td className="px-4 py-3">
                    <div className="font-medium text-gray-900">
                      {schedule.name}
                    </div>
                    <div className="text-xs text-gray-400">
                      {schedule.jobKey}
                    </div>
                  </td>
                  <td className="px-4 py-3">
                    {isEditing ? (
                      <input
                        type="text"
                        value={editCron}
                        onChange={(e) => setEditCron(e.target.value)}
                        className="w-40 rounded border border-gray-300 px-2 py-1 font-mono text-xs focus:border-blue-500 focus:outline-none"
                      />
                    ) : (
                      <code className="rounded bg-gray-100 px-2 py-0.5 text-xs">
                        {schedule.cronExpression}
                      </code>
                    )}
                  </td>
                  <td className="px-4 py-3">
                    <button
                      onClick={() => handleToggle(schedule)}
                      disabled={isPending}
                      className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${
                        schedule.enabled
                          ? "bg-green-100 text-green-700 hover:bg-green-200"
                          : "bg-gray-100 text-gray-500 hover:bg-gray-200"
                      }`}
                    >
                      {schedule.enabled
                        ? t["admin.label.enabled"] ?? "Enabled"
                        : t["admin.label.disabled"] ?? "Disabled"}
                    </button>
                  </td>
                  <td className="px-4 py-3 text-xs text-gray-500">
                    {formatDate(schedule.lastExecutedAt)}
                  </td>
                  <td className="px-4 py-3 text-right">
                    <div className="flex items-center justify-end gap-2">
                      {isEditing ? (
                        <>
                          <button
                            onClick={() => handleSave(schedule)}
                            disabled={isPending}
                            className="rounded bg-blue-600 px-3 py-1 text-xs font-medium text-white hover:bg-blue-700 disabled:opacity-50"
                          >
                            {t["admin.action.save"] ?? "Save"}
                          </button>
                          <button
                            onClick={cancelEdit}
                            className="rounded border border-gray-300 px-3 py-1 text-xs font-medium text-gray-700 hover:bg-gray-50"
                          >
                            {t["admin.action.cancel"] ?? "Cancel"}
                          </button>
                        </>
                      ) : (
                        <>
                          <button
                            onClick={() => startEdit(schedule)}
                            disabled={isPending}
                            className="rounded border border-gray-300 px-3 py-1 text-xs font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50"
                          >
                            {t["admin.action.edit"] ?? "Edit"}
                          </button>
                          <button
                            onClick={() => handleTrigger(schedule)}
                            disabled={isPending || !schedule.enabled}
                            className="rounded bg-amber-500 px-3 py-1 text-xs font-medium text-white hover:bg-amber-600 disabled:opacity-50"
                            title={
                              !schedule.enabled
                                ? t["admin.scheduler.trigger_disabled"] ?? "Enable the job before triggering"
                                : t["admin.scheduler.trigger_hint"] ?? "Run this job now"
                            }
                          >
                            {isPending ? "..." : t["admin.action.trigger"] ?? "Trigger"}
                          </button>
                        </>
                      )}
                    </div>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </>
  );
}
