"use client";

interface BarChartItem {
  label: string;
  value: number;
  sublabel?: string;
}

interface BarChartProps {
  items: BarChartItem[];
  formatValue?: (value: number) => string;
  color?: string;
}

export function BarChart({
  items,
  formatValue = (v) => v.toLocaleString(),
  color = "bg-blue-500",
}: BarChartProps) {
  if (items.length === 0) {
    return (
      <div className="rounded-lg border border-gray-200 bg-white p-8 text-center text-sm text-gray-500">
        No data.
      </div>
    );
  }

  const max = Math.max(...items.map((i) => i.value), 1);

  return (
    <div className="space-y-2">
      {items.map((item, idx) => (
        <div key={idx} className="flex items-center gap-3">
          <span className="w-36 truncate text-sm text-gray-700" title={item.label}>
            {item.label}
          </span>
          <div className="flex-1 h-6 rounded bg-gray-100">
            <div
              className={`h-6 rounded ${color}`}
              style={{ width: `${(item.value / max) * 100}%`, minWidth: item.value > 0 ? "4px" : "0" }}
            />
          </div>
          <span className="w-32 text-right text-sm font-medium text-gray-700">
            {formatValue(item.value)}
            {item.sublabel && (
              <span className="ml-1 text-xs text-gray-400">({item.sublabel})</span>
            )}
          </span>
        </div>
      ))}
    </div>
  );
}
