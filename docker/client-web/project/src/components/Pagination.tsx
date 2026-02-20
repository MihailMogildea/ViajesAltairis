"use client";

import { useLocale } from "@/context/LocaleContext";

interface PaginationProps {
  page: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

export default function Pagination({ page, totalPages, onPageChange }: PaginationProps) {
  const { t } = useLocale();

  if (totalPages <= 1) return null;

  const pages: (number | "...")[] = [];
  for (let i = 1; i <= totalPages; i++) {
    if (i === 1 || i === totalPages || (i >= page - 1 && i <= page + 1)) {
      pages.push(i);
    } else if (pages[pages.length - 1] !== "...") {
      pages.push("...");
    }
  }

  return (
    <nav className="flex items-center justify-center gap-1 pt-6" aria-label="Pagination">
      <button
        onClick={() => onPageChange(page - 1)}
        disabled={page <= 1}
        className="rounded-lg px-3 py-2 text-sm font-medium text-gray-600 hover:bg-gray-100 disabled:opacity-40 disabled:pointer-events-none"
      >
        {t("client.pagination.previous")}
      </button>
      {pages.map((p, i) =>
        p === "..." ? (
          <span key={`ellipsis-${i}`} className="px-2 text-gray-400">...</span>
        ) : (
          <button
            key={p}
            onClick={() => onPageChange(p)}
            className={`min-w-[36px] rounded-lg px-3 py-2 text-sm font-medium ${
              p === page ? "bg-blue-600 text-white" : "text-gray-600 hover:bg-gray-100"
            }`}
          >
            {p}
          </button>
        )
      )}
      <button
        onClick={() => onPageChange(page + 1)}
        disabled={page >= totalPages}
        className="rounded-lg px-3 py-2 text-sm font-medium text-gray-600 hover:bg-gray-100 disabled:opacity-40 disabled:pointer-events-none"
      >
        {t("client.pagination.next")}
      </button>
    </nav>
  );
}
