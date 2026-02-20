"use client";

import { BarChart } from "@/components/bar-chart";
import { DataTable, type Column } from "@/components/data-table";
import type { RevenueByHotelDto, RevenueByProviderDto, RevenuePeriodDto } from "@/types/statistics";

interface RevenueTabProps {
  byHotel: RevenueByHotelDto[];
  byProvider: RevenueByProviderDto[];
  byPeriod: RevenuePeriodDto[];
  t: Record<string, string>;
}

export function RevenueTab({ byHotel, byProvider, byPeriod, t }: RevenueTabProps) {
  const fmtCurrency = (v: number) => `€${v.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;

  const hotelColumns: Column<RevenueByHotelDto>[] = [
    { key: "hotelName", header: t["admin.statistics.hotel"] ?? "Hotel" },
    {
      key: "totalRevenue",
      header: t["admin.statistics.revenue"] ?? "Revenue",
      render: (item) => fmtCurrency(item.totalRevenue),
      className: "text-right",
    },
    {
      key: "reservationCount",
      header: t["admin.statistics.count"] ?? "Count",
      render: (item) => String(item.reservationCount),
      className: "text-right",
    },
  ];

  return (
    <div className="space-y-8">
      <section>
        <h3 className="mb-3 text-lg font-semibold">{t["admin.statistics.by_hotel"] ?? "Revenue by Hotel"}</h3>
        <BarChart
          items={byHotel.map((h) => ({
            label: h.hotelName,
            value: h.totalRevenue,
            sublabel: String(h.reservationCount),
          }))}
          formatValue={fmtCurrency}
        />
        <div className="mt-4">
          <DataTable columns={hotelColumns} data={byHotel} keyField="hotelId" emptyMessage={t["admin.label.no_data"] ?? "No data."} />
        </div>
      </section>

      <section>
        <h3 className="mb-3 text-lg font-semibold">{t["admin.statistics.by_provider"] ?? "Revenue by Provider"}</h3>
        <BarChart
          items={byProvider.map((p) => ({
            label: p.providerName,
            value: p.totalRevenue,
            sublabel: String(p.reservationCount),
          }))}
          formatValue={fmtCurrency}
          color="bg-indigo-500"
        />
      </section>

      <section>
        <h3 className="mb-3 text-lg font-semibold">{t["admin.statistics.by_period"] ?? "Revenue by Period"}</h3>
        <BarChart
          items={byPeriod.map((p) => ({
            label: p.period,
            value: p.totalRevenue,
            sublabel: String(p.reservationCount),
          }))}
          formatValue={fmtCurrency}
          color="bg-emerald-500"
        />
      </section>
    </div>
  );
}
