"use client";

import Image from "next/image";
import Link from "next/link";
import { useLocale } from "@/context/LocaleContext";

export default function Footer() {
  const { t } = useLocale();

  return (
    <footer className="mt-auto border-t border-gray-200 bg-gray-50">
      <div className="mx-auto max-w-7xl px-4 py-10 sm:px-6">
        <div className="grid gap-8 sm:grid-cols-3">
          <div>
            <div className="flex items-center gap-2">
              <Image src="/logo.jpg" alt="ViajesAltairis" width={28} height={28} className="rounded" />
              <h3 className="text-sm font-semibold text-gray-900">ViajesAltairis</h3>
            </div>
            <p className="mt-2 text-sm text-gray-500">
              {t("client.footer.description")}
            </p>
          </div>
          <div>
            <h3 className="text-sm font-semibold text-gray-900">{t("client.footer.destinations")}</h3>
            <ul className="mt-2 space-y-1 text-sm text-gray-500">
              <li><Link href="/hotels?destination=Mallorca" className="hover:text-gray-700">Mallorca</Link></li>
              <li><Link href="/hotels?destination=Menorca" className="hover:text-gray-700">Menorca</Link></li>
              <li><Link href="/hotels?destination=Ibiza" className="hover:text-gray-700">Ibiza</Link></li>
              <li><Link href="/hotels?destination=Barcelona" className="hover:text-gray-700">Barcelona</Link></li>
              <li><Link href="/hotels?destination=Nice" className="hover:text-gray-700">Nice</Link></li>
            </ul>
          </div>
          <div>
            <h3 className="text-sm font-semibold text-gray-900">{t("client.footer.support")}</h3>
            <ul className="mt-2 space-y-1 text-sm text-gray-500">
              <li>{t("client.footer.email")}</li>
              <li>{t("client.footer.phone")}</li>
              <li>{t("client.footer.hours")}</li>
            </ul>
          </div>
        </div>
        <div className="mt-8 border-t border-gray-200 pt-6 text-center text-xs text-gray-400">
          &copy; {new Date().getFullYear()} {t("client.footer.rights")}
        </div>
      </div>
    </footer>
  );
}
