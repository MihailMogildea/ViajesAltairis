import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { Breadcrumb } from "@/components/breadcrumb";
import { UserDetail } from "./user-detail";
import type { UserDto, UserTypeDto, UserHotelDto, UserSubscriptionDto } from "@/types/user";
import type { LanguageDto } from "@/types/system";
import type { HotelDto } from "@/types/hotel";
import type { SubscriptionTypeDto } from "@/types/subscription";
import type { BusinessPartnerDto } from "@/types/business-partner";
import type { ProviderDto } from "@/types/provider";

export default async function UserDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const session = await getSession();
  const access = session ? getAccessLevel(session.role, "users") : null;
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let user: UserDto | null = null;
  let userTypes: UserTypeDto[] = [];
  let languages: LanguageDto[] = [];
  let hotels: HotelDto[] = [];
  let userHotels: UserHotelDto[] = [];
  let subscriptionTypes: SubscriptionTypeDto[] = [];
  let userSubscriptions: UserSubscriptionDto[] = [];
  let businessPartners: BusinessPartnerDto[] = [];
  let providers: ProviderDto[] = [];
  let error: string | null = null;

  try {
    [
      user,
      userTypes,
      languages,
      hotels,
      userHotels,
      subscriptionTypes,
      userSubscriptions,
      businessPartners,
      providers,
    ] = await Promise.all([
      apiFetch<UserDto>(`/api/Users/${id}`, { cache: "no-store" }),
      apiFetch<UserTypeDto[]>("/api/UserTypes", { cache: "no-store" }),
      apiFetch<LanguageDto[]>("/api/Languages", { cache: "no-store" }),
      apiFetch<HotelDto[]>("/api/Hotels", { cache: "no-store" }),
      apiFetch<UserHotelDto[]>(`/api/UserHotels?userId=${id}`, { cache: "no-store" }),
      apiFetch<SubscriptionTypeDto[]>("/api/SubscriptionTypes", { cache: "no-store" }),
      apiFetch<UserSubscriptionDto[]>(`/api/UserSubscriptions?userId=${id}`, { cache: "no-store" }),
      apiFetch<BusinessPartnerDto[]>("/api/BusinessPartners", { cache: "no-store" }),
      apiFetch<ProviderDto[]>("/api/Providers", { cache: "no-store" }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load user data";
  }

  return (
    <div>
      <Breadcrumb
        items={[
          { label: t["admin.section.users"] ?? "Users", href: "/users" },
          { label: user?.email ?? `User #${id}` },
        ]}
      />
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">
          {user ? `${user.firstName} ${user.lastName}` : t["admin.users.detail"] ?? "User Detail"}
        </h2>
        <p className="mt-1 text-sm text-gray-500">{user?.email ?? ""}</p>
      </div>

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : user ? (
        <UserDetail
          user={user}
          userTypes={userTypes}
          languages={languages}
          hotels={hotels}
          userHotels={userHotels}
          subscriptionTypes={subscriptionTypes}
          userSubscriptions={userSubscriptions}
          businessPartners={businessPartners}
          providers={providers}
          access={access}
          t={t}
        />
      ) : (
        <p className="text-sm text-red-600">{t["admin.users.not_found"] ?? "User not found."}</p>
      )}
    </div>
  );
}
