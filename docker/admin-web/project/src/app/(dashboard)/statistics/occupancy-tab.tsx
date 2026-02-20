"use client";

import type { OccupancyDto } from "@/types/statistics";

interface OccupancyTabProps {
  occupancy: OccupancyDto[];
  t: Record<string, string>;
}

export function OccupancyTab({ occupancy, t }: OccupancyTabProps) {
  if (occupancy.length === 0) {
    return (
      <div className="rounded-lg border border-gray-200 bg-white p-8 text-center text-sm text-gray-500">
        {t["admin.label.no_data"] ?? "No data."}
      </div>
    );
  }

  return (
    <div className="overflow-hidden rounded-lg border border-gray-200 bg-white">
      <table className="w-full text-left text-sm">
        <thead className="border-b border-gray-200 bg-gray-50 text-xs font-medium uppercase text-gray-500">
          <tr>
            <th className="px-4 py-3">{t["admin.statistics.hotel"] ?? "Hotel"}</th>
            <th className="px-4 py-3 text-right">{t["admin.statistics.booked_nights"] ?? "Booked Nights"}</th>
            <th className="px-4 py-3 text-right">{t["admin.statistics.total_nights"] ?? "Total Nights"}</th>
            <th className="px-4 py-3">{t["admin.statistics.occupancy_rate"] ?? "Occupancy Rate"}</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-100">
          {occupancy.map((item) => (
            <tr key={item.hotelId} className="hover:bg-gray-50">
              <td className="px-4 py-3 font-medium text-gray-700">{item.hotelName}</td>
              <td className="px-4 py-3 text-right text-gray-700">{item.bookedRoomNights.toLocaleString()}</td>
              <td className="px-4 py-3 text-right text-gray-700">{item.totalRoomNights.toLocaleString()}</td>
              <td className="px-4 py-3">
                <div className="flex items-center gap-3">
                  <div className="flex-1 h-4 rounded bg-gray-100">
                    <div
                      className="h-4 rounded bg-teal-500"
                      style={{ width: `${Math.min(item.occupancyRate, 100)}%` }}
                    />
                  </div>
                  <span className="w-14 text-right text-sm font-medium text-gray-700">
                    {item.occupancyRate.toFixed(1)}%
                  </span>
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
