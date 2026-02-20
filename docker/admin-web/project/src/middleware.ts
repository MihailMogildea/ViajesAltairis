import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";
import { decodeJwt } from "jose";
import { USER_TYPE_ID, sectionFromPath, canAccess } from "./lib/permissions";
import { COOKIE_NAME } from "./lib/auth";

/** Routes that don't require authentication. */
const PUBLIC_PATHS = ["/login", "/unauthorized"];

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  // Allow public paths and Next.js internals
  if (
    PUBLIC_PATHS.some((p) => pathname.startsWith(p)) ||
    pathname.startsWith("/_next") ||
    pathname.startsWith("/favicon")
  ) {
    return NextResponse.next();
  }

  // Check auth cookie
  const token = request.cookies.get(COOKIE_NAME)?.value;
  if (!token) {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  // Decode JWT (no verification — admin-api handles that on API calls)
  try {
    const payload = decodeJwt(token) as { user_type_id?: string; exp?: number };

    // Expired?
    if (payload.exp && payload.exp * 1000 < Date.now()) {
      const response = NextResponse.redirect(new URL("/login", request.url));
      response.cookies.delete(COOKIE_NAME);
      return response;
    }

    // Map to role (JWT claims are strings)
    const userTypeId = payload.user_type_id ? Number(payload.user_type_id) : null;
    const role = userTypeId ? USER_TYPE_ID[userTypeId] : null;
    if (!role) {
      // client or unknown — no admin access
      const response = NextResponse.redirect(new URL("/login", request.url));
      response.cookies.delete(COOKIE_NAME);
      return response;
    }

    // Check section access
    const section = sectionFromPath(pathname);
    if (section && !canAccess(role, section)) {
      return NextResponse.redirect(new URL("/unauthorized", request.url));
    }
  } catch {
    // Malformed token
    const response = NextResponse.redirect(new URL("/login", request.url));
    response.cookies.delete(COOKIE_NAME);
    return response;
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/((?!_next/static|_next/image|favicon.ico).*)"],
};
