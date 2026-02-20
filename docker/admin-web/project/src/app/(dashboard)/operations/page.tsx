import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { SectionHeader } from "@/components/section-header";
import { OperationsTabs } from "./operations-tabs";
import type { HotelDto, HotelBlackoutDto } from "@/types/hotel";
import type { CancellationDto, CancellationPolicyDto } from "@/types/reservation";

export default async function OperationsPage() {
  const session = await getSession();
  const access = session ? getAccessLevel(session.role, "operations") : null;
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  if (!access) {
    return (
      <div>
        <SectionHeader
          title={t["admin.section.operations"] ?? "Operations"}
        />
        <p className="text-sm text-red-600">
          {t["admin.label.no_access"] ?? "You do not have access to this section."}
        </p>
      </div>
    );
  }

  let blackouts: HotelBlackoutDto[] = [];
  let cancellations: CancellationDto[] = [];
  let policies: CancellationPolicyDto[] = [];
  let hotels: HotelDto[] = [];
  let error: string | null = null;

  try {
    [blackouts, cancellations, policies, hotels] = await Promise.all([
      apiFetch<HotelBlackoutDto[]>("/api/HotelBlackouts", { cache: "no-store" }),
      apiFetch<CancellationDto[]>("/api/Cancellations", { cache: "no-store" }),
      apiFetch<CancellationPolicyDto[]>("/api/CancellationPolicies", { cache: "no-store" }),
      apiFetch<HotelDto[]>("/api/Hotels", { cache: "no-store" }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load operations data";
  }

  return (
    <div>
      <SectionHeader
        title={t["admin.section.operations"] ?? "Operations"}
        description={t["admin.section.operations.desc"] ?? "Blackout periods, cancellations, and cancellation policies."}
      />

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <OperationsTabs
          blackouts={blackouts}
          cancellations={cancellations}
          policies={policies}
          hotels={hotels}
          access={access}
          t={t}
        />
      )}
    </div>
  );
}
