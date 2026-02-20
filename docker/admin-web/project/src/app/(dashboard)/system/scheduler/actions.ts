"use server";

import { apiFetch } from "@/lib/api";

interface UpdateSchedulePayload {
  cronExpression: string;
  enabled: boolean;
}

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

export async function updateSchedule(
  jobKey: string,
  payload: UpdateSchedulePayload
): Promise<JobSchedule> {
  return apiFetch<JobSchedule>(`/api/job-schedules/${jobKey}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export async function triggerJob(jobKey: string): Promise<void> {
  await apiFetch(`/api/job-schedules/${jobKey}/trigger`, {
    method: "POST",
  });
}
