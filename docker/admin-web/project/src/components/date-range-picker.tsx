"use client";

interface DateRangePickerProps {
  from: string;
  to: string;
  onChange: (from: string, to: string) => void;
  t: Record<string, string>;
}

function formatDate(date: Date): string {
  return date.toISOString().split("T")[0];
}

export function DateRangePicker({ from, to, onChange, t }: DateRangePickerProps) {
  const today = new Date();

  const presets = [
    {
      label: t["admin.statistics.last_30_days"] ?? "30d",
      getRange: () => {
        const d = new Date(today);
        d.setDate(d.getDate() - 30);
        return { from: formatDate(d), to: formatDate(today) };
      },
    },
    {
      label: t["admin.statistics.last_90_days"] ?? "90d",
      getRange: () => {
        const d = new Date(today);
        d.setDate(d.getDate() - 90);
        return { from: formatDate(d), to: formatDate(today) };
      },
    },
    {
      label: t["admin.statistics.this_year"] ?? "Year",
      getRange: () => ({
        from: `${today.getFullYear()}-01-01`,
        to: formatDate(today),
      }),
    },
  ];

  return (
    <div className="mb-6 flex flex-wrap items-end gap-4">
      <div>
        <label className="mb-1 block text-sm font-medium text-gray-700">
          {t["admin.statistics.date_from"] ?? "From"}
        </label>
        <input
          type="date"
          value={from}
          onChange={(e) => onChange(e.target.value, to)}
          className="rounded border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
        />
      </div>
      <div>
        <label className="mb-1 block text-sm font-medium text-gray-700">
          {t["admin.statistics.date_to"] ?? "To"}
        </label>
        <input
          type="date"
          value={to}
          onChange={(e) => onChange(from, e.target.value)}
          className="rounded border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
        />
      </div>
      <div className="flex gap-2">
        {presets.map((preset) => (
          <button
            key={preset.label}
            onClick={() => {
              const range = preset.getRange();
              onChange(range.from, range.to);
            }}
            className="rounded border border-gray-300 bg-white px-3 py-2 text-sm font-medium text-gray-600 hover:bg-gray-50"
          >
            {preset.label}
          </button>
        ))}
      </div>
    </div>
  );
}
