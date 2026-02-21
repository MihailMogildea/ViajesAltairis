"use client";

import { createContext, useContext, useState, useEffect, type ReactNode } from "react";
import type { User } from "@/types";
import { findUserByEmail } from "@/data/users";
import { apiLogin, apiRegister, apiGetProfile, setToken, getToken, clearToken, onAuthError } from "@/lib/api";

interface AuthState {
  user: User | null;
  login: (email: string, password: string) => Promise<boolean>;
  register: (params: { email: string; password: string; firstName: string; lastName: string; phone?: string }) => Promise<string | null>;
  logout: () => void;
  refreshProfile: () => Promise<void>;
}

const AuthContext = createContext<AuthState>({
  user: null,
  login: async () => false,
  register: async () => "Unavailable",
  logout: () => {},
  refreshProfile: async () => {},
});

const PROFILE_KEY = "va_profile";
const MOCK_PASSWORD = "password123";

function mapProfileToUser(p: { id: number; email: string; firstName: string; lastName: string; discount: number; subscriptionType?: string; subscriptionDiscount?: number }): User {
  return {
    id: p.id,
    email: p.email,
    first_name: p.firstName,
    last_name: p.lastName,
    user_type: "client",
    discount: p.discount,
    subscription_type: p.subscriptionType,
    subscription_discount: p.subscriptionDiscount,
  };
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);

  // Handle 401 token expiry — clear auth state
  useEffect(() => {
    const unsubscribe = onAuthError(() => {
      setUser(null);
      clearToken();
      localStorage.removeItem(PROFILE_KEY);
    });
    return unsubscribe;
  }, []);

  useEffect(() => {
    const token = getToken();
    if (token) {
      apiGetProfile()
        .then((profile) => {
          const u = mapProfileToUser(profile);
          setUser(u);
          localStorage.setItem(PROFILE_KEY, JSON.stringify(u));
        })
        .catch(() => {
          // API unavailable — restore from cached profile
          const cached = localStorage.getItem(PROFILE_KEY);
          if (cached) {
            try { setUser(JSON.parse(cached)); } catch { /* ignore */ }
          }
        });
    } else {
      // No token (mock login) — restore from cached profile
      const cached = localStorage.getItem(PROFILE_KEY);
      if (cached) {
        try { setUser(JSON.parse(cached)); } catch { /* ignore */ }
      }
    }
  }, []);

  async function login(email: string, password: string): Promise<boolean> {
    try {
      const res = await apiLogin(email, password);
      setToken(res.token);
      try {
        const profile = await apiGetProfile();
        const u = mapProfileToUser(profile);
        setUser(u);
        localStorage.setItem(PROFILE_KEY, JSON.stringify(u));
      } catch {
        // Token works but profile fetch failed — use mock data as fallback
        const mock = findUserByEmail(email);
        if (mock) {
          setUser(mock);
          localStorage.setItem(PROFILE_KEY, JSON.stringify(mock));
        }
      }
      return true;
    } catch {
      // API unavailable — fall back to mock data but VERIFY password
      if (password !== MOCK_PASSWORD) return false;
      const found = findUserByEmail(email);
      if (!found) return false;
      setUser(found);
      localStorage.setItem(PROFILE_KEY, JSON.stringify(found));
      return true;
    }
  }

  /** Returns null on success, error message on failure */
  async function register(params: {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
    phone?: string;
  }): Promise<string | null> {
    try {
      const res = await apiRegister({
        ...params,
        preferredLanguageId: 1, // English
        preferredCurrencyId: 1, // EUR
      });
      setToken(res.token);
      try {
        const profile = await apiGetProfile();
        const u = mapProfileToUser(profile);
        setUser(u);
        localStorage.setItem(PROFILE_KEY, JSON.stringify(u));
      } catch {
        // Token works but profile failed — build user from registration data
        const u: User = {
          id: res.userId,
          email: params.email,
          first_name: params.firstName,
          last_name: params.lastName,
          user_type: "client",
          discount: 0,
        };
        setUser(u);
        localStorage.setItem(PROFILE_KEY, JSON.stringify(u));
      }
      return null;
    } catch (err) {
      return err instanceof Error ? err.message : "Registration failed";
    }
  }

  async function refreshProfile() {
    try {
      const profile = await apiGetProfile();
      const u = mapProfileToUser(profile);
      setUser(u);
      localStorage.setItem(PROFILE_KEY, JSON.stringify(u));
    } catch { /* ignore */ }
  }

  function logout() {
    setUser(null);
    clearToken();
    localStorage.removeItem(PROFILE_KEY);
  }

  return (
    <AuthContext.Provider value={{ user, login, register, logout, refreshProfile }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
