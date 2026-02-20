"use server";

import { redirect } from "next/navigation";
import { setAuthCookie, clearAuthCookie } from "./auth";
import { setLocale } from "./locale-server";
import type { Locale } from "./locale";
import { apiLogin } from "./api";

export async function loginAction(
  _prevState: { error: string } | null,
  formData: FormData
): Promise<{ error: string } | null> {
  const email = formData.get("email") as string;
  const password = formData.get("password") as string;

  if (!email || !password) {
    return { error: "Email and password are required" };
  }

  try {
    const token = await apiLogin(email, password);
    await setAuthCookie(token);
  } catch (e) {
    return { error: e instanceof Error ? e.message : "Login failed" };
  }

  redirect("/");
}

export async function logoutAction(): Promise<void> {
  await clearAuthCookie();
  redirect("/login");
}

export async function switchLocaleAction(locale: Locale): Promise<void> {
  await setLocale(locale);
}
