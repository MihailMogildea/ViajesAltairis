import { getLocale } from "@/lib/locale-server";
import { loadTranslations } from "@/lib/translations";
import { LoginForm } from "./login-form";

export default async function LoginPage() {
  const locale = await getLocale();
  const t = await loadTranslations(locale);

  return <LoginForm t={t} />;
}
