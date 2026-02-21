import { getSession, clearAuthCookie } from "./auth";
import { getLocale } from "./locale-server";

/** Base URL for admin-api. Inside Docker network it resolves to the service name. */
const API_BASE_URL =
  process.env.NEXT_PUBLIC_ADMIN_API_URL ?? "http://admin-api:8080";

export async function apiFetch<T>(
  path: string,
  init?: RequestInit
): Promise<T> {
  const session = await getSession();
  const locale = await getLocale();

  const res = await fetch(`${API_BASE_URL}${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      "Accept-Language": locale,
      ...(session ? { Authorization: `Bearer ${session.token}` } : {}),
      ...init?.headers,
    },
  });

  if (res.status === 401) {
    await clearAuthCookie();
    throw new Error("Session expired. Please sign in again.");
  }

  if (res.status === 403) {
    throw new Error("Access denied.");
  }

  if (!res.ok) {
    throw new Error(`API error: ${res.status} ${res.statusText}`);
  }

  if (res.status === 204) {
    return undefined as T;
  }

  return res.json() as Promise<T>;
}

/**
 * Login against admin-api. Returns the raw JWT string on success.
 * Throws on invalid credentials or network error.
 */
export async function apiLogin(
  email: string,
  password: string
): Promise<string> {
  const res = await fetch(`${API_BASE_URL}/api/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password }),
  });

  if (!res.ok) {
    if (res.status === 401) throw new Error("Invalid email or password");
    throw new Error(`Login failed: ${res.status}`);
  }

  const data = (await res.json()) as { token: string };
  return data.token;
}
