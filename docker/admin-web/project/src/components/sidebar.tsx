"use client";

import Image from "next/image";
import Link from "next/link";
import { usePathname } from "next/navigation";
import type { UserRole } from "@/lib/permissions";
import type { Locale } from "@/lib/locale";
import { NAV_ITEMS, ROLE_LABELS, canAccess } from "@/lib/permissions";
import { logoutAction } from "@/lib/actions";
import { LocaleSwitcher } from "./locale-switcher";

interface SidebarProps {
  role: UserRole;
  userName: string;
  locale: Locale;
  t: Record<string, string>;
  businessPartnerId?: number | null;
}

export function Sidebar({ role, userName, locale, t, businessPartnerId }: SidebarProps) {
  const pathname = usePathname();
  const visibleItems = NAV_ITEMS.filter((item) => canAccess(role, item.section, businessPartnerId));

  return (
    <aside className="flex w-64 flex-col border-r border-gray-200 bg-white">
      <div className="flex items-center gap-3 p-6">
        <Image src="/logo.jpg" alt="ViajesAltairis" width={40} height={40} className="rounded" />
        <div>
          <h1 className="text-xl font-bold">ViajesAltairis</h1>
          <p className="mt-1 text-xs text-gray-400">
            {t["admin.sidebar.admin"] ?? "Admin"}
          </p>
        </div>
      </div>

      <nav className="flex-1 space-y-1 px-3">
        {visibleItems.map((item) => {
          const active =
            item.href === "/"
              ? pathname === "/"
              : pathname.startsWith(item.href);

          const label = t[`admin.nav.${item.section}`] ?? item.label;

          return (
            <Link
              key={item.section}
              href={item.href}
              className={`block rounded px-3 py-2 text-sm font-medium ${
                active
                  ? "bg-blue-50 text-blue-700"
                  : "text-gray-700 hover:bg-gray-100"
              }`}
            >
              {label}
            </Link>
          );
        })}
      </nav>

      <div className="border-t border-gray-200 p-4">
        <div className="mb-3 flex items-center justify-between">
          <div>
            <p className="truncate text-sm font-medium text-gray-700">
              {userName}
            </p>
            <p className="text-xs text-gray-400">{ROLE_LABELS[role] ?? role}</p>
          </div>
          <LocaleSwitcher current={locale} />
        </div>
        <form action={logoutAction}>
          <button
            type="submit"
            className="text-sm text-gray-500 hover:text-gray-700"
          >
            {t["admin.sidebar.signout"] ?? "Sign out"}
          </button>
        </form>
      </div>
    </aside>
  );
}
