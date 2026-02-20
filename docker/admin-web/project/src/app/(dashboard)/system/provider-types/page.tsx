import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { Breadcrumb } from "@/components/breadcrumb";
import { ProviderTypesTable } from "./provider-types-table";

interface ProviderTypeDto {
  id: number;
  name: string;
  createdAt: string;
}

export default async function ProviderTypesPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let providerTypes: ProviderTypeDto[] = [];
  let error: string | null = null;

  try {
    providerTypes = await apiFetch<ProviderTypeDto[]>("/api/ProviderTypes", {
      cache: "no-store",
    });
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load provider types";
  }

  return (
    <div>
      <Breadcrumb
        items={[
          { label: t["admin.section.system"] ?? "System Config", href: "/system" },
          { label: t["admin.system.provider_types"] ?? "Provider Types" },
        ]}
      />

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <ProviderTypesTable providerTypes={providerTypes} t={t} />
      )}
    </div>
  );
}
