import Link from "next/link";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";

export default async function UnauthorizedPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-50">
      <div className="text-center">
        <h1 className="mb-2 text-4xl font-bold text-gray-900">
          {t["admin.unauthorized.title"] ?? "403"}
        </h1>
        <p className="mb-6 text-gray-500">
          {t["admin.unauthorized.message"] ?? "You don't have access to this section."}
        </p>
        <Link
          href="/"
          className="text-sm font-medium text-blue-600 hover:text-blue-700"
        >
          {t["admin.unauthorized.back"] ?? "Back to Dashboard"}
        </Link>
      </div>
    </div>
  );
}
