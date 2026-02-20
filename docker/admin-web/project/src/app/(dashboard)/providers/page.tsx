import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { ProvidersTable } from "./providers-table";
import type { ProviderDto, ProviderTypeDto } from "@/types/provider";
import type { CurrencyDto } from "@/types/system";

export default async function ProvidersPage() {
  const session = await getSession();
  const access = session
    ? getAccessLevel(session.role, "providers")
    : null;
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let providers: ProviderDto[] = [];
  let providerTypes: ProviderTypeDto[] = [];
  let currencies: CurrencyDto[] = [];
  let error: string | null = null;

  try {
    [providers, providerTypes, currencies] = await Promise.all([
      apiFetch<ProviderDto[]>("/api/Providers", { cache: "no-store" }),
      apiFetch<ProviderTypeDto[]>("/api/ProviderTypes", { cache: "no-store" }),
      apiFetch<CurrencyDto[]>("/api/Currencies", { cache: "no-store" }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load providers";
  }

  return (
    <div>
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">
          {t["admin.section.providers"] ?? "Providers"}
        </h2>
        <p className="mt-1 text-sm text-gray-500">
          {t["admin.providers.desc"] ?? "Manage hotel providers, API connections, and margins."}
        </p>
      </div>

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <ProvidersTable
          providers={providers}
          providerTypes={providerTypes}
          currencies={currencies}
          t={t}
          access={access}
        />
      )}
    </div>
  );
}
