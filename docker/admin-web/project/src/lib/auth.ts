import { cookies } from "next/headers";
import { decodeJwt } from "jose";
import type { UserRole } from "./permissions";
import { USER_TYPE_ID } from "./permissions";

const COOKIE_NAME = "altairis_token";

export interface Session {
  token: string;
  userId: number;
  email: string;
  name: string;
  role: UserRole;
  /** provider_id if hotel_staff is a regional manager */
  providerId: number | null;
  /** business_partner_id if agent belongs to a partner */
  businessPartnerId: number | null;
}

/**
 * Expected JWT payload from admin-api /auth/login.
 *
 * Contract with admin-api:
 * {
 *   sub: string (user id),
 *   email: string,
 *   name: string,
 *   user_type_id: number,
 *   provider_id: number | null,
 *   business_partner_id: number | null,
 *   exp: number
 * }
 */
interface JwtPayload {
  sub: string;
  email: string;
  name: string;
  user_type_id: string;
  provider_id?: string;
  business_partner_id?: string;
  exp: number;
}

/** Read session from the auth cookie. Returns null if not authenticated. */
export async function getSession(): Promise<Session | null> {
  const cookieStore = await cookies();
  const tokenCookie = cookieStore.get(COOKIE_NAME);
  if (!tokenCookie) return null;

  try {
    return parseToken(tokenCookie.value);
  } catch {
    return null;
  }
}

/** Decode JWT and map to Session (no signature verification — admin-api owns that). */
function parseToken(token: string): Session | null {
  const payload = decodeJwt(token) as unknown as JwtPayload;

  // Reject expired tokens client-side
  if (payload.exp && payload.exp * 1000 < Date.now()) return null;

  const userTypeId = Number(payload.user_type_id);
  const role = USER_TYPE_ID[userTypeId];
  if (!role) return null; // client or unknown role

  return {
    token,
    userId: Number(payload.sub),
    email: payload.email,
    name: payload.name,
    role,
    providerId: payload.provider_id ? Number(payload.provider_id) : null,
    businessPartnerId: payload.business_partner_id ? Number(payload.business_partner_id) : null,
  };
}

/** Store JWT in httpOnly cookie. Called from the login server action. */
export async function setAuthCookie(token: string): Promise<void> {
  const payload = decodeJwt(token) as unknown as JwtPayload;
  const cookieStore = await cookies();

  cookieStore.set(COOKIE_NAME, token, {
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    sameSite: "lax",
    path: "/",
    // Align cookie expiry with JWT expiry
    ...(payload.exp ? { expires: new Date(payload.exp * 1000) } : {}),
  });
}

/** Clear the auth cookie. */
export async function clearAuthCookie(): Promise<void> {
  const cookieStore = await cookies();
  cookieStore.delete(COOKIE_NAME);
}

export { COOKIE_NAME };
