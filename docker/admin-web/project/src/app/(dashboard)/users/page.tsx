import { apiFetch } from "@/lib/api";
import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { getSession } from "@/lib/auth";
import { getAccessLevel } from "@/lib/permissions";
import { UsersTable } from "./users-table";
import type { UserDto, UserTypeDto } from "@/types/user";
import type { LanguageDto, TranslationDto } from "@/types/system";

export default async function UsersPage() {
  const session = await getSession();
  const access = session ? getAccessLevel(session.role, "users") : null;
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  let users: UserDto[] = [];
  let userTypes: UserTypeDto[] = [];
  let languages: LanguageDto[] = [];
  let translations: TranslationDto[] = [];
  let error: string | null = null;

  try {
    [users, userTypes, languages, translations] = await Promise.all([
      apiFetch<UserDto[]>("/api/Users", { cache: "no-store" }),
      apiFetch<UserTypeDto[]>("/api/UserTypes", { cache: "no-store" }),
      apiFetch<LanguageDto[]>("/api/Languages", { cache: "no-store" }),
      apiFetch<TranslationDto[]>("/api/Translations", { cache: "no-store" }),
    ]);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load users";
  }

  const langId = languages.find((l) => l.isoCode === locale)?.id ?? 1;
  const utNames: Record<number, string> = {};
  for (const tr of translations) {
    if (tr.entityType === "user_type" && tr.field === "name" && tr.languageId === langId) {
      utNames[tr.entityId] = tr.value;
    }
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
          utNames={utNames}
          t={t}
          access={access}
        />
      )}
    </div>
  );
}
