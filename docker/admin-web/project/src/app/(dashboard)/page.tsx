import { getSession } from "@/lib/auth";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { apiFetch } from "@/lib/api";
import Link from "next/link";
import type { HotelDto } from "@/types/hotel";
import type { ReservationAdminDto } from "@/types/reservation";
import type { ReviewDto } from "@/types/review";
import type { InvoiceDto } from "@/types/invoice";

interface DashboardStats {
  totalHotels: number;
  activeHotels: number;
  pendingReservations: number;
  hiddenReviews: number;
  recentReservations: ReservationAdminDto[];
}

async function loadStats(): Promise<DashboardStats> {
  const [hotels, reservations, reviews] = await Promise.all([
    apiFetch<HotelDto[]>("/api/Hotels", { cache: "no-store" }).catch(() => [] as HotelDto[]),
    apiFetch<ReservationAdminDto[]>("/api/Reservations", { cache: "no-store" }).catch(() => [] as ReservationAdminDto[]),
    apiFetch<ReviewDto[]>("/api/Reviews", { cache: "no-store" }).catch(() => [] as ReviewDto[]),
  ]);

  return {
    totalHotels: hotels.length,
    activeHotels: hotels.filter((h) => h.enabled).length,
    pendingReservations: reservations.filter((r) => r.statusName?.toLowerCase().includes("pending")).length,
    hiddenReviews: reviews.filter((r) => !r.visible).length,
    recentReservations: reservations.slice(0, 5),
  };
}

export default async function DashboardPage() {
  const session = await getSession();
  const locale = await getLocale();
  const t = await loadTranslations(locale);
  const stats = await loadStats();

  const cards = [
    {
      label: t["admin.dashboard.hotels"] ?? "Hotels",
      value: `${stats.activeHotels} / ${stats.totalHotels}`,
      sub: t["admin.dashboard.active_total"] ?? "active / total",
      href: "/hotels",
    },
    {
      label: t["admin.dashboard.pending_reservations"] ?? "Pending Reservations",
      value: String(stats.pendingReservations),
      sub: t["admin.dashboard.awaiting_confirmation"] ?? "awaiting confirmation",
      href: "/reservations",
    },
    {
      label: t["admin.dashboard.hidden_reviews"] ?? "Hidden Reviews",
      value: String(stats.hiddenReviews),
      sub: t["admin.dashboard.pending_moderation"] ?? "pending moderation",
      href: "/reviews",
    },
  ];

  return (
    <div>
      <h2 className="mb-2 text-2xl font-semibold">
        {t["admin.dashboard.title"] ?? "Dashboard"}
      </h2>
      <p className="mb-6 text-sm text-gray-500">
        {t["admin.dashboard.welcome"] ?? "Welcome"}, {session?.name}.{" "}
        {t["admin.dashboard.role_label"] ?? "You are signed in as"}{" "}
        <span className="font-medium">{session?.role}</span>.
      </p>

      <div className="mb-8 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {cards.map((card) => (
          <Link
            key={card.href}
            href={card.href}
            className="rounded-lg border border-gray-200 bg-white p-5 hover:border-blue-300 hover:shadow-sm"
          >
            <p className="text-sm font-medium text-gray-500">{card.label}</p>
            <p className="mt-1 text-3xl font-semibold text-gray-900">{card.value}</p>
            <p className="mt-1 text-xs text-gray-400">{card.sub}</p>
          </Link>
        ))}
      </div>

      <div>
        <h3 className="mb-3 text-lg font-semibold">
          {t["admin.dashboard.recent_reservations"] ?? "Recent Reservations"}
        </h3>
        {stats.recentReservations.length === 0 ? (
          <p className="text-sm text-gray-500">
            {t["admin.label.no_data"] ?? "No data."}
          </p>
        ) : (
          <div className="overflow-hidden rounded-lg border border-gray-200 bg-white">
            <table className="w-full text-left text-sm">
              <thead className="border-b border-gray-200 bg-gray-50 text-xs font-medium uppercase text-gray-500">
                <tr>
                  <th className="px-4 py-3">{t["admin.reservations.code"] ?? "Code"}</th>
                  <th className="px-4 py-3">{t["admin.reservations.status"] ?? "Status"}</th>
                  <th className="px-4 py-3">{t["admin.reservations.owner"] ?? "Owner"}</th>
                  <th className="px-4 py-3">{t["admin.reservations.total"] ?? "Total"}</th>
                  <th className="px-4 py-3">{t["admin.reservations.date"] ?? "Date"}</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {stats.recentReservations.map((r) => (
                  <tr key={r.id} className="hover:bg-gray-50">
                    <td className="px-4 py-3">
                      <Link
                        href={`/reservations/${r.id}`}
                        className="font-medium text-blue-600 hover:underline"
                      >
                        {r.reservationCode}
                      </Link>
                    </td>
                    <td className="px-4 py-3">
                      <span
                        className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${
                          r.statusName?.toLowerCase().includes("confirmed")
                            ? "bg-green-100 text-green-700"
                            : r.statusName?.toLowerCase().includes("pending")
                              ? "bg-amber-100 text-amber-700"
                              : r.statusName?.toLowerCase().includes("draft")
                                ? "bg-blue-100 text-blue-700"
                                : r.statusName?.toLowerCase().includes("cancel")
                                  ? "bg-red-100 text-red-700"
                                  : "bg-gray-100 text-gray-500"
                        }`}
                      >
                        {r.statusName}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-gray-700">
                      {r.ownerFirstName} {r.ownerLastName}
                    </td>
                    <td className="px-4 py-3 text-gray-700">
                      {r.totalPrice.toFixed(2)} {r.currencyCode}
                    </td>
                    <td className="px-4 py-3 text-xs text-gray-500">
                      {new Date(r.createdAt).toLocaleDateString()}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
