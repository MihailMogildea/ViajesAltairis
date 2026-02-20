"use client";

import { useEffect } from "react";

interface ToastMessageProps {
  message: string | null;
  onDismiss: () => void;
  duration?: number;
}

export function ToastMessage({ message, onDismiss, duration = 4000 }: ToastMessageProps) {
  useEffect(() => {
    if (!message) return;
    const timer = setTimeout(onDismiss, duration);
    return () => clearTimeout(timer);
  }, [message, onDismiss, duration]);

  if (!message) return null;

  return (
    <p className="mb-4 rounded border border-blue-200 bg-blue-50 px-4 py-2 text-sm text-blue-800">
      {message}
    </p>
  );
}
