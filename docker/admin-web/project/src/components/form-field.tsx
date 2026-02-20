"use client";

interface Option {
  value: string | number;
  label: string;
}

interface FormFieldProps {
  label: string;
  type?: "text" | "number" | "email" | "date" | "time" | "textarea" | "select" | "checkbox" | "password";
  value: string | number | boolean;
  onChange: (value: string | number | boolean) => void;
  options?: Option[];
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
  step?: string;
}

export function FormField({
  label,
  type = "text",
  value,
  onChange,
  options,
  placeholder,
  required,
  disabled,
  step,
}: FormFieldProps) {
  const inputClass =
    "w-full rounded border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500 disabled:bg-gray-50 disabled:text-gray-500";

  if (type === "checkbox") {
    return (
      <label className="flex items-center gap-2 text-sm">
        <input
          type="checkbox"
          checked={Boolean(value)}
          onChange={(e) => onChange(e.target.checked)}
          disabled={disabled}
          className="rounded border-gray-300"
        />
        {label}
      </label>
    );
  }

  if (type === "select") {
    return (
      <div>
        <label className="mb-1 block text-sm font-medium text-gray-700">{label}</label>
        <select
          value={String(value)}
          onChange={(e) => onChange(e.target.value)}
          disabled={disabled}
          className={inputClass}
        >
          <option value="">{placeholder ?? `Select ${label}...`}</option>
          {options?.map((opt) => (
            <option key={opt.value} value={opt.value}>
              {opt.label}
            </option>
          ))}
        </select>
      </div>
    );
  }

  if (type === "textarea") {
    return (
      <div>
        <label className="mb-1 block text-sm font-medium text-gray-700">{label}</label>
        <textarea
          value={String(value)}
          onChange={(e) => onChange(e.target.value)}
          placeholder={placeholder}
          required={required}
          disabled={disabled}
          rows={3}
          className={inputClass}
        />
      </div>
    );
  }

  return (
    <div>
      <label className="mb-1 block text-sm font-medium text-gray-700">{label}</label>
      <input
        type={type}
        value={type === "number" ? (value as number) : String(value)}
        onChange={(e) =>
          type === "number"
            ? onChange(e.target.valueAsNumber || 0)
            : onChange(e.target.value)
        }
        placeholder={placeholder}
        required={required}
        disabled={disabled}
        step={step}
        className={inputClass}
      />
    </div>
  );
}
