"use client";

import { BarChart } from "@/components/bar-chart";
import type { BookingVolumeDto, BookingsByStatusDto, BookingAverageDto } from "@/types/statistics";

interface BookingsTabProps {
  volume: BookingVolumeDto[];
  byStatus: BookingsByStatusDto[];
  average: BookingAverageDto | null;
  t: Record<string, string>;
}

const STATUS_COLORS: Record<string, string> = {
  confirmed: "bg-green-500",
  pending: "bg-amber-500",
  draft: "bg-blue-500",
  cancelled: "bg-red-500",
  completed: "bg-emerald-500",
  checked_in: "bg-purple-500",
};

export function BookingsTab({ volume, byStatus, average, t }: BookingsTabProps) {
  return (
    <div className="space-y-8">
      <section>
        <h3 className="mb-3 text-lg font-semibold">{t["admin.statistics.volume"] ?? "Booking Volume"}</h3>
        <BarChart
          items={volume.map((v) => ({ label: v.period, value: v.bookingCount }))}
        />
      </section>

      <section>
        <h3 className="mb-3 text-lg font-semibold">{t["admin.statistics.by_status"] ?? "Bookings by Status"}</h3>
        <BarChart
          items={byStatus.map((s) => ({ label: s.statusName, value: s.bookingCount }))}
          color={byStatus.length > 0 ? (STATUS_COLORS[byStatus[0].statusName.toLowerCase()] ?? "bg-blue-500") : "bg-blue-500"}
        />
      </section>

      {average && (
        <section>
          <h3 className="mb-3 text-lg font-semibold">{t["admin.statistics.averages"] ?? "Booking Averages"}</h3>
          <div className="grid gap-4 sm:grid-cols-3">
            <Card
              label={t["admin.statistics.avg_value"] ?? "Avg. Value"}
              value={`€${average.averageValue.toFixed(2)}`}
            />
            <Card
              label={t["admin.statistics.avg_nights"] ?? "Avg. Nights"}
              value={average.averageNights.toFixed(1)}
            />
            <Card
              label={t["admin.statistics.total_bookings"] ?? "Total Bookings"}
              value={String(average.totalBookings)}
            />
          </div>
        </section>
      )}
    </div>
  );
}

function Card({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-lg border border-gray-200 bg-white p-5">
      <p className="text-sm font-medium text-gray-500">{label}</p>
      <p className="mt-1 text-3xl font-semibold text-gray-900">{value}</p>
    </div>
  );
}
