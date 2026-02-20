"use client";

type Variant = "enabled" | "disabled" | "success" | "warning" | "danger" | "info";

const styles: Record<Variant, string> = {
  enabled: "bg-green-100 text-green-700",
  disabled: "bg-gray-100 text-gray-500",
  success: "bg-green-100 text-green-700",
  warning: "bg-amber-100 text-amber-700",
  danger: "bg-red-100 text-red-700",
  info: "bg-blue-100 text-blue-700",
};

interface StatusBadgeProps {
  variant: Variant;
  children: React.ReactNode;
  onClick?: () => void;
  disabled?: boolean;
}

export function StatusBadge({ variant, children, onClick, disabled }: StatusBadgeProps) {
  const base = `inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${styles[variant]}`;

  if (onClick) {
    return (
      <button onClick={onClick} disabled={disabled} className={`${base} hover:opacity-80`}>
        {children}
      </button>
    );
  }

  return <span className={base}>{children}</span>;
}
