import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { BusinessPartnersTable } from "./business-partners-table";
import type { BusinessPartnerDto } from "@/types/business-partner";

export default async function BusinessPartnersPage() {
  const session = await getSession();
  const access = session
    ? getAccessLevel(session.role, "business-partners")
    : null;
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let partners: BusinessPartnerDto[] = [];
  let error: string | null = null;

  try {
    partners = await apiFetch<BusinessPartnerDto[]>("/api/BusinessPartners", {
      cache: "no-store",
    });
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load business partners";
  }

  return (
    <div>
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">
          {t["admin.section.business_partners"] ?? "Business Partners"}
        </h2>
        <p className="mt-1 text-sm text-gray-500">
          {t["admin.business_partners.desc"] ?? "Manage business partner accounts, discounts, and contact information."}
        </p>
      </div>

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <BusinessPartnersTable partners={partners} t={t} access={access} />
      )}
    </div>
  );
}
