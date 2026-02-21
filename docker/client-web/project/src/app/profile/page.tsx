"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useAuth } from "@/context/AuthContext";
import { useLocale } from "@/context/LocaleContext";
import { apiGetProfile, apiUpdateProfile, apiChangePassword, apiGetCountries } from "@/lib/api";
import type { ApiProfileResponse, ApiCountry } from "@/types";

export default function ProfilePage() {
  const { user, refreshProfile } = useAuth();
  const { locale, t } = useLocale();

  const [profile, setProfile] = useState<ApiProfileResponse | null>(null);
  const [loading, setLoading] = useState(true);

  // Edit form
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [phone, setPhone] = useState("");
  const [countryId, setCountryId] = useState<number | undefined>(undefined);
  const [countries, setCountries] = useState<ApiCountry[]>([]);
  const [saving, setSaving] = useState(false);
  const [saveMsg, setSaveMsg] = useState<{ text: string; ok: boolean } | null>(null);

  // Password form
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [pwSaving, setPwSaving] = useState(false);
  const [pwMsg, setPwMsg] = useState<{ text: string; ok: boolean } | null>(null);

  useEffect(() => {
    if (!user) return;
    apiGetCountries().then(setCountries).catch(() => {});
    apiGetProfile()
      .then((p) => {
        setProfile(p);
        setFirstName(p.firstName);
        setLastName(p.lastName);
        setPhone(p.phone || "");
      })
      .catch(() => {
        // API unavailable — use local user data
        setFirstName(user.first_name);
        setLastName(user.last_name);
      })
      .finally(() => setLoading(false));
  }, [user, locale]);

  // Match country name from profile to country ID once both are loaded
  useEffect(() => {
    if (profile?.country && countries.length > 0 && countryId === undefined) {
      const match = countries.find((c) => c.name === profile.country);
      if (match) setCountryId(match.id);
    }
  }, [profile, countries, countryId]);

  if (!user) {
    return (
      <div className="mx-auto max-w-lg px-4 py-16 text-center">
        <h1 className="text-2xl font-bold text-gray-900">{t("client.profile.title")}</h1>
        <p className="mt-2 text-gray-500">{t("client.profile.login_message")}</p>
        <Link href="/login" className="mt-4 inline-block rounded-lg bg-blue-600 px-6 py-2.5 text-sm font-semibold text-white hover:bg-blue-700">
          {t("client.common.login")}
        </Link>
      </div>
    );
  }

  async function handleSaveProfile(e: React.FormEvent) {
    e.preventDefault();
    setSaving(true);
    setSaveMsg(null);
    try {
      await apiUpdateProfile({
        firstName,
        lastName,
        phone: phone || undefined,
        countryId,
      });
      await refreshProfile();
      setSaveMsg({ text: t("client.profile.saved"), ok: true });
    } catch {
      setSaveMsg({ text: t("client.profile.save_error"), ok: false });
    } finally {
      setSaving(false);
    }
  }

  async function handleChangePassword(e: React.FormEvent) {
    e.preventDefault();
    setPwMsg(null);
    if (newPassword !== confirmPassword) {
      setPwMsg({ text: t("client.profile.pw_mismatch"), ok: false });
      return;
    }
    if (newPassword.length < 8) {
      setPwMsg({ text: t("client.profile.pw_length"), ok: false });
      return;
    }
    setPwSaving(true);
    try {
      await apiChangePassword(currentPassword, newPassword);
      setCurrentPassword("");
      setNewPassword("");
      setConfirmPassword("");
      setPwMsg({ text: t("client.profile.pw_changed"), ok: true });
    } catch {
      setPwMsg({ text: t("client.profile.pw_error"), ok: false });
    } finally {
      setPwSaving(false);
    }
  }

  if (loading) {
    return (
      <div className="mx-auto max-w-lg px-4 py-8">
        <h1 className="mb-6 text-2xl font-bold text-gray-900">{t("client.profile.title")}</h1>
        <p className="text-gray-400">{t("client.profile.loading")}</p>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-lg px-4 py-8 sm:px-6">
      <h1 className="mb-6 text-2xl font-bold text-gray-900">{t("client.profile.title")}</h1>

      {/* Profile info (read-only) */}
      <div className="rounded-xl border border-gray-200 p-5 mb-6">
        <div className="flex items-center gap-4">
          <div className="flex h-14 w-14 items-center justify-center rounded-full bg-blue-100 text-xl font-semibold text-blue-600">
            {user.first_name[0]}{user.last_name[0]}
          </div>
          <div>
            <p className="text-lg font-semibold text-gray-900">{user.first_name} {user.last_name}</p>
            <p className="text-sm text-gray-500">{user.email}</p>
          </div>
        </div>
        {profile && (
          <div className="mt-4 grid grid-cols-2 gap-3 text-sm">
            {profile.phone && (
              <div>
                <p className="text-xs text-gray-400">{t("client.profile.phone")}</p>
                <p className="text-gray-700">{profile.phone}</p>
              </div>
            )}
            {profile.country && (
              <div>
                <p className="text-xs text-gray-400">{t("client.profile.country")}</p>
                <p className="text-gray-700">{profile.country}</p>
              </div>
            )}
            <div>
              <p className="text-xs text-gray-400">{t("client.profile.language")}</p>
              <p className="text-gray-700">{profile.preferredLanguage}</p>
            </div>
            <div>
              <p className="text-xs text-gray-400">{t("client.profile.currency")}</p>
              <p className="text-gray-700">{profile.preferredCurrency}</p>
            </div>
            {user.discount > 0 && (
              <div>
                <p className="text-xs text-gray-400">{t("client.profile.discount")}</p>
                <p className="text-green-600 font-medium">{user.discount}%</p>
              </div>
            )}
            {user.subscription_type && (
              <div>
                <p className="text-xs text-gray-400">{t("client.profile.subscription")}</p>
                <p className="text-purple-600 font-medium">{user.subscription_type}</p>
              </div>
            )}
          </div>
        )}
      </div>

      {/* Edit profile */}
      <form onSubmit={handleSaveProfile} className="rounded-xl border border-gray-200 p-5 mb-6">
        <h2 className="mb-4 text-lg font-semibold text-gray-900">{t("client.profile.edit_title")}</h2>
        <div className="grid gap-3 sm:grid-cols-2">
          <div>
            <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.profile.first_name")}</label>
            <input
              type="text" required value={firstName} onChange={(e) => setFirstName(e.target.value)}
              className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            />
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.profile.last_name")}</label>
            <input
              type="text" required value={lastName} onChange={(e) => setLastName(e.target.value)}
              className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            />
          </div>
        </div>
        <div className="mt-3">
          <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.profile.phone")}</label>
          <input
            type="tel" value={phone} onChange={(e) => setPhone(e.target.value)}
            className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            placeholder="+34 600 000 000"
          />
        </div>
        {countries.length > 0 && (
          <div className="mt-3">
            <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.profile.country")}</label>
            <select
              value={countryId ?? ""}
              onChange={(e) => setCountryId(e.target.value ? Number(e.target.value) : undefined)}
              className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            >
              <option value="">{t("client.profile.select_country")}</option>
              {countries.map((c) => (
                <option key={c.id} value={c.id}>{c.name}</option>
              ))}
            </select>
          </div>
        )}
        {saveMsg && (
          <p className={`mt-3 text-sm ${saveMsg.ok ? "text-green-600" : "text-red-600"}`}>{saveMsg.text}</p>
        )}
        <button
          type="submit" disabled={saving}
          className="mt-4 rounded-lg bg-blue-600 px-5 py-2 text-sm font-semibold text-white hover:bg-blue-700 transition-colors disabled:opacity-50"
        >
          {saving ? t("client.profile.saving") : t("client.profile.save")}
        </button>
      </form>

      {/* Change password */}
      <form onSubmit={handleChangePassword} className="rounded-xl border border-gray-200 p-5">
        <h2 className="mb-4 text-lg font-semibold text-gray-900">{t("client.profile.change_password")}</h2>
        <div className="space-y-3">
          <div>
            <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.profile.current_password")}</label>
            <input
              type="password" required value={currentPassword} onChange={(e) => setCurrentPassword(e.target.value)}
              className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            />
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.profile.new_password")}</label>
            <input
              type="password" required value={newPassword} onChange={(e) => setNewPassword(e.target.value)}
              minLength={8}
              className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
              placeholder={t("client.register.min_chars")}
            />
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium text-gray-700">{t("client.profile.confirm_password")}</label>
            <input
              type="password" required value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)}
              minLength={8}
              className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            />
          </div>
        </div>
        {pwMsg && (
          <p className={`mt-3 text-sm ${pwMsg.ok ? "text-green-600" : "text-red-600"}`}>{pwMsg.text}</p>
        )}
        <button
          type="submit" disabled={pwSaving}
          className="mt-4 rounded-lg bg-gray-100 px-5 py-2 text-sm font-medium text-gray-700 hover:bg-gray-200 transition-colors disabled:opacity-50"
        >
          {pwSaving ? t("client.profile.changing") : t("client.profile.change_password")}
        </button>
      </form>
    </div>
  );
}
