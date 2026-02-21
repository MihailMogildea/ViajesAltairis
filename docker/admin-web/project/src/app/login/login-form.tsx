"use client";

import Image from "next/image";
import { useActionState } from "react";
import { loginAction } from "@/lib/actions";

export function LoginForm({ t }: { t: Record<string, string> }) {
  const [state, formAction, pending] = useActionState(loginAction, null);

  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-50">
      <div className="w-full max-w-sm rounded-lg border border-gray-200 bg-white p-8 shadow-sm">
        <div className="mb-4 flex justify-center">
          <Image src="/logo.jpg" alt="ViajesAltairis" width={80} height={80} className="rounded" />
        </div>
        <h1 className="mb-6 text-center text-2xl font-bold text-gray-900">
          {t["admin.login.title"] ?? "ViajesAltairis"}
        </h1>
        <p className="mb-6 text-center text-sm text-gray-500">
          {t["admin.login.subtitle"] ?? "Admin Dashboard"}
        </p>

        <form action={formAction} className="space-y-4">
          <div>
            <label htmlFor="email" className="mb-1 block text-sm font-medium text-gray-700">
              {t["admin.login.email"] ?? "Email"}
            </label>
            <input
              id="email"
              name="email"
              type="email"
              required
              autoComplete="email"
              className="w-full rounded border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            />
          </div>

          <div>
            <label htmlFor="password" className="mb-1 block text-sm font-medium text-gray-700">
              {t["admin.login.password"] ?? "Password"}
            </label>
            <input
              id="password"
              name="password"
              type="password"
              required
              autoComplete="current-password"
              className="w-full rounded border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            />
          </div>

          {state?.error && (
            <p className="text-sm text-red-600">{state.error}</p>
          )}

          <button
            type="submit"
            disabled={pending}
            className="w-full rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
          >
            {pending
              ? t["admin.login.submitting"] ?? "Signing in..."
              : t["admin.login.submit"] ?? "Sign in"}
          </button>
        </form>
      </div>
    </div>
  );
}
