"use client";

interface ConfirmDialogProps {
  open: boolean;
  title?: string;
  message: string;
  onConfirm: () => void;
  onCancel: () => void;
  loading?: boolean;
  confirmLabel?: string;
  cancelLabel?: string;
}

export function ConfirmDialog({
  open,
  title = "Confirm",
  message,
  onConfirm,
  onCancel,
  loading = false,
  confirmLabel = "Delete",
  cancelLabel = "Cancel",
}: ConfirmDialogProps) {
  if (!open) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <div className="w-full max-w-sm rounded-lg bg-white p-6 shadow-lg">
        <h3 className="mb-2 text-lg font-semibold">{title}</h3>
        <p className="mb-6 text-sm text-gray-600">{message}</p>
        <div className="flex justify-end gap-2">
          <button
            onClick={onCancel}
            disabled={loading}
            className="rounded border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
          >
            {cancelLabel}
          </button>
          <button
            onClick={onConfirm}
            disabled={loading}
            className="rounded bg-red-600 px-4 py-2 text-sm font-medium text-white hover:bg-red-700 disabled:opacity-50"
          >
            {loading ? "..." : confirmLabel}
          </button>
        </div>
      </div>
    </div>
  );
}
