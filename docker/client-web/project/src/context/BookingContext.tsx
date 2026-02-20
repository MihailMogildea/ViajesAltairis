"use client";

import { createContext, useContext, useState, useEffect, useMemo, type ReactNode } from "react";
import type { BasketItem, ApiSubmitResponse } from "@/types";
import { calculateNights } from "@/lib/utils";
import {
  apiValidatePromoCode,
  apiCreateDraftReservation,
  apiAddReservationLine,
  apiAddReservationGuest,
  apiSubmitReservation,
} from "@/lib/api";

interface BookingState {
  items: BasketItem[];
  promoCode: string;
  promoDiscount: number;
  addItem: (item: Omit<BasketItem, "id" | "nights" | "line_total">) => void;
  removeItem: (id: string) => void;
  clearBasket: () => void;
  applyPromoCode: (code: string) => Promise<boolean>;
  submitBooking: (guestFirstName: string, guestLastName: string, guestEmail: string) => Promise<ApiSubmitResponse | null>;
  subtotal: number;
  total: number;
}

const PROMO_CODES: Record<string, { type: "percent" | "fixed"; value: number }> = {
  WELCOME10: { type: "percent", value: 10 },
  SUMMER25: { type: "fixed", value: 25 },
  VIP2026: { type: "percent", value: 15 },
};

const BASKET_KEY = "va_basket";
const PROMO_KEY = "va_promo";

function loadBasket(): BasketItem[] {
  if (typeof window === "undefined") return [];
  try {
    const raw = localStorage.getItem(BASKET_KEY);
    return raw ? JSON.parse(raw) : [];
  } catch { return []; }
}

function loadPromo(): string {
  if (typeof window === "undefined") return "";
  return localStorage.getItem(PROMO_KEY) || "";
}

const BookingContext = createContext<BookingState>({
  items: [],
  promoCode: "",
  promoDiscount: 0,
  addItem: () => {},
  removeItem: () => {},
  clearBasket: () => {},
  applyPromoCode: async () => false,
  submitBooking: async () => null,
  subtotal: 0,
  total: 0,
});

export function BookingProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState<BasketItem[]>(loadBasket);
  const [promoCode, setPromoCode] = useState(loadPromo);
  // Store promo percentage/fixed info so we can recalculate when subtotal changes
  const [promoPercent, setPromoPercent] = useState<number>(0);
  const [promoFixed, setPromoFixed] = useState<number>(0);

  // Persist basket to localStorage
  useEffect(() => {
    localStorage.setItem(BASKET_KEY, JSON.stringify(items));
  }, [items]);

  // Persist promo code to localStorage
  useEffect(() => {
    if (promoCode) {
      localStorage.setItem(PROMO_KEY, promoCode);
    } else {
      localStorage.removeItem(PROMO_KEY);
    }
  }, [promoCode]);

  const subtotal = items.reduce((sum, item) => sum + item.line_total, 0);

  // Recalculate discount whenever subtotal changes (fixes stale discount)
  const promoDiscount = useMemo(() => {
    if (!promoCode) return 0;
    if (promoPercent > 0) return subtotal * (promoPercent / 100);
    if (promoFixed > 0) return Math.min(promoFixed, subtotal);
    // Fallback: check hardcoded codes
    const hc = PROMO_CODES[promoCode];
    if (!hc) return 0;
    return hc.type === "percent" ? subtotal * (hc.value / 100) : Math.min(hc.value, subtotal);
  }, [promoCode, promoPercent, promoFixed, subtotal]);

  const total = Math.max(0, subtotal - promoDiscount);

  function addItem(item: Omit<BasketItem, "id" | "nights" | "line_total">) {
    const nights = calculateNights(item.check_in, item.check_out);
    const line_total = (item.price_per_night + item.board_supplement) * nights * item.num_rooms;
    const newItem: BasketItem = {
      ...item,
      id: crypto.randomUUID(),
      nights,
      line_total,
    };
    setItems((prev) => [...prev, newItem]);
  }

  function removeItem(id: string) {
    setItems((prev) => prev.filter((item) => item.id !== id));
  }

  function clearBasket() {
    setItems([]);
    setPromoCode("");
    setPromoPercent(0);
    setPromoFixed(0);
  }

  async function applyPromoCode(code: string): Promise<boolean> {
    const upper = code.toUpperCase().trim();

    // Try API validation first
    try {
      const result = await apiValidatePromoCode(upper);
      if (result.isValid) {
        setPromoCode(upper);
        setPromoPercent(result.discountPercentage ?? 0);
        setPromoFixed(result.discountAmount ?? 0);
        return true;
      }
      return false;
    } catch {
      // API unavailable — fall back to hardcoded codes
      const hc = PROMO_CODES[upper];
      if (hc) {
        setPromoCode(upper);
        setPromoPercent(hc.type === "percent" ? hc.value : 0);
        setPromoFixed(hc.type === "fixed" ? hc.value : 0);
        return true;
      }
      return false;
    }
  }

  async function submitBooking(
    guestFirstName: string,
    guestLastName: string,
    _guestEmail: string
  ): Promise<ApiSubmitResponse | null> {
    try {
      // 1. Create draft reservation
      const reservationId = await apiCreateDraftReservation(
        "EUR",
        promoCode || undefined
      );

      // 2. Add each basket item as a reservation line + guest
      for (const item of items) {
        const lineId = await apiAddReservationLine(
          reservationId,
          item.room_config_id,
          item.board_type_id,
          item.check_in,
          item.check_out,
          item.guests
        );
        await apiAddReservationGuest(
          reservationId,
          lineId,
          guestFirstName,
          guestLastName
        );
      }

      // 3. Submit
      const result = await apiSubmitReservation(reservationId);
      return result;
    } catch {
      // API unavailable — return null so caller can fall back
      return null;
    }
  }

  return (
    <BookingContext.Provider
      value={{ items, promoCode, promoDiscount, addItem, removeItem, clearBasket, applyPromoCode, submitBooking, subtotal, total }}
    >
      {children}
    </BookingContext.Provider>
  );
}

export function useBooking() {
  return useContext(BookingContext);
}
