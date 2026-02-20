import Link from "next/link";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";

const systemSections = [
  { key: "scheduler", href: "/system/scheduler" },
  { key: "languages", href: "/system/languages" },
  { key: "currencies", href: "/system/currencies" },
  { key: "exchange_rates", href: "/system/exchange-rates" },
  { key: "countries", href: "/system/countries" },
  { key: "admin_division_types", href: "/system/admin-division-types" },
  { key: "admin_divisions", href: "/system/admin-divisions" },
  { key: "cities", href: "/system/cities" },
  { key: "taxes", href: "/system/taxes" },
  { key: "email_templates", href: "/system/email-templates" },
  { key: "translations", href: "/system/translations" },
  { key: "provider_types", href: "/system/provider-types" },
  { key: "notifications", href: "/system/notifications" },
];

export default async function SystemPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  return (
    <div>
      <h2 className="mb-6 text-2xl font-semibold">
        {t["admin.section.system"] ?? "System Configuration"}
      </h2>
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {systemSections.map((s) => (
          <Link
            key={s.href}
            href={s.href}
            className="rounded-lg border border-gray-200 bg-white p-5 hover:border-blue-300 hover:shadow-sm"
          >
            <h3 className="mb-1 text-sm font-semibold text-gray-900">
              {t[`admin.system.${s.key}`] ?? s.key}
            </h3>
            <p className="text-xs text-gray-500">
              {t[`admin.system.${s.key}.desc`] ?? ""}
            </p>
          </Link>
        ))}
      </div>
    </div>
  );
}
