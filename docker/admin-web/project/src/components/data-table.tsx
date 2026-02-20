"use client";

export interface Column<T> {
  key: string;
  header: string;
  render?: (item: T) => React.ReactNode;
  className?: string;
}

interface DataTableProps<T> {
  columns: Column<T>[];
  data: T[];
  keyField: keyof T;
  emptyMessage?: string;
  actions?: (item: T) => React.ReactNode;
}

export function DataTable<T>({
  columns,
  data,
  keyField,
  emptyMessage = "No data.",
  actions,
}: DataTableProps<T>) {
  if (data.length === 0) {
    return (
      <div className="rounded-lg border border-gray-200 bg-white p-8 text-center text-sm text-gray-500">
        {emptyMessage}
      </div>
    );
  }

  return (
    <div className="overflow-hidden rounded-lg border border-gray-200 bg-white">
      <table className="w-full text-left text-sm">
        <thead className="border-b border-gray-200 bg-gray-50 text-xs font-medium uppercase text-gray-500">
          <tr>
            {columns.map((col) => (
              <th key={col.key} className={`px-4 py-3 ${col.className ?? ""}`}>
                {col.header}
              </th>
            ))}
            {actions && <th className="px-4 py-3 text-right">Actions</th>}
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-100">
          {data.map((item) => (
            <tr key={String(item[keyField])} className="hover:bg-gray-50">
              {columns.map((col) => (
                <td key={col.key} className={`px-4 py-3 ${col.className ?? ""}`}>
                  {col.render
                    ? col.render(item)
                    : String((item as Record<string, unknown>)[col.key] ?? "")}
                </td>
              ))}
              {actions && (
                <td className="px-4 py-3 text-right">
                  <div className="flex items-center justify-end gap-2">
                    {actions(item)}
                  </div>
                </td>
              )}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
