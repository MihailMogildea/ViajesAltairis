import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { UsersTable } from "./users-table";
import type { UserDto, UserTypeDto } from "@/types/user";
import type { LanguageDto } from "@/types/system";

export default async function UsersPage() {
  const session = await getSession();
  const access = session ? getAccessLevel(session.role, "users") : null;
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let users: UserDto[] = [];
  let userTypes: UserTypeDto[] = [];
  let languages: LanguageDto[] = [];
  let error: string | null = null;

  try {
    [users, userTypes, languages] = await Promise.all([
      apiFetch<UserDto[]>("/api/Users", { cache: "no-store" }),
      apiFetch<UserTypeDto[]>("/api/UserTypes", { cache: "no-store" }),
      apiFetch<LanguageDto[]>("/api/Languages", { cache: "no-store" }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load users";
  }

  return (
    <div>
      <div className="mb-6">
        <h2 className="text-2xl font-semibold">
          {t["admin.section.users"] ?? "Users"}
        </h2>
        <p className="mt-1 text-sm text-gray-500">
          {t["admin.users.desc"] ?? "Manage user accounts, roles, and permissions."}
        </p>
      </div>

      {error ? (
        <p className="text-sm text-red-600">{error}</p>
      ) : (
        <UsersTable
          users={users}
          userTypes={userTypes}
          languages={languages}
          t={t}
          access={access}
        />
      )}
    </div>
  );
}
