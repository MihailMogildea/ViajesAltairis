import { redirect } from "next/navigation";
import { getSession } from "@/lib/auth";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { Sidebar } from "@/components/sidebar";

export default async function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const session = await getSession();
  if (!session) redirect("/login");

  const locale = await getLocale();
  const translations = await loadTranslations(locale);

  return (
    <div className="flex h-screen">
      <Sidebar
        role={session.role}
        userName={session.name}
        locale={locale}
        t={translations}
      />
      <main className="flex-1 overflow-y-auto p-8">{children}</main>
    </div>
  );
}
