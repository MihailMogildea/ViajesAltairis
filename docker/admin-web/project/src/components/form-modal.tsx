"use client";

import { useEffect, useRef } from "react";

interface FormModalProps {
  open: boolean;
  title: string;
  onClose: () => void;
  onSubmit: () => void;
  loading?: boolean;
  saveLabel?: string;
  cancelLabel?: string;
  children: React.ReactNode;
}

export function FormModal({
  open,
  title,
  onClose,
  onSubmit,
  loading = false,
  saveLabel = "Save",
  cancelLabel = "Cancel",
  children,
}: FormModalProps) {
  const overlayRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!open) return;
    const handler = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    };
    document.addEventListener("keydown", handler);
    return () => document.removeEventListener("keydown", handler);
  }, [open, onClose]);

  if (!open) return null;

  return (
    <div
      ref={overlayRef}
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/40"
      onClick={(e) => {
        if (e.target === overlayRef.current) onClose();
      }}
    >
      <div className="w-full max-w-lg rounded-lg bg-white shadow-lg">
        <div className="border-b border-gray-200 px-6 py-4">
          <h3 className="text-lg font-semibold">{title}</h3>
        </div>
        <div className="max-h-[70vh] overflow-y-auto px-6 py-4 space-y-4">
          {children}
        </div>
        <div className="flex justify-end gap-2 border-t border-gray-200 px-6 py-4">
          <button
            onClick={onClose}
            disabled={loading}
            className="rounded border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
          >
            {cancelLabel}
          </button>
          <button
            onClick={onSubmit}
            disabled={loading}
            className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
          >
            {loading ? "..." : saveLabel}
          </button>
        </div>
      </div>
    </div>
  );
}
