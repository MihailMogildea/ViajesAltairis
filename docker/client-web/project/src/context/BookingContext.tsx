"use client";

import { createContext, useContext, useState, useEffect, useMemo, type ReactNode } from "react";
import type { BasketItem, ApiSubmitResponse } from "@/types";
import { calculateNights } from "@/lib/utils";
import { useAuth } from "@/context/AuthContext";
import {
  apiValidatePromoCode,
  apiCreateDraftReservation,
  apiAddReservationLine,
  apiAddReservationGuest,
  apiSubmitReservation,
  apiGetHotelTaxes,
} from "@/lib/api";

interface BookingState {
  items: BasketItem[];
  promoCode: string;
  promoDiscount: number;
  userDiscountAmount: number;
  taxEstimate: number;
  addItem: (item: Omit<BasketItem, "id" | "nights" | "line_total">) => void;
  removeItem: (id: string) => void;
  clearBasket: () => void;
  applyPromoCode: (code: string) => Promise<boolean>;
  submitBooking: (currencyCode: string, guestFirstName: string, guestLastName: string, guestEmail: string, paymentMethodId: number, cardNumber?: string, cardExpiry?: string, cardCvv?: string, cardHolderName?: string) => Promise<ApiSubmitResponse | null>;
  subtotal: number;
  total: number;
}

function estimateTax(items: BasketItem[], discountPct: number): number {
  let tax = 0;
  for (const item of items) {
    if (!item.taxes || item.taxes.length === 0) continue;
    const discountedTotal = item.line_total * (1 - discountPct / 100);
    for (const t of item.taxes) {
      if (t.isPercentage) {
        tax += discountedTotal * (t.rate / 100);
      } else {
        tax += t.rate * item.nights * item.num_rooms;
      }
    }
  }
  return Math.round(tax * 100) / 100;
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
  userDiscountAmount: 0,
  taxEstimate: 0,
  addItem: () => {},
  removeItem: () => {},
  clearBasket: () => {},
  applyPromoCode: async () => false,
  submitBooking: async () => null,
  subtotal: 0,
  total: 0,
});

export function BookingProvider({ children }: { children: ReactNode }) {
  const { user } = useAuth();
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

  // Backfill taxes for items loaded from localStorage without them
  useEffect(() => {
    const missing = items.filter((i) => !i.taxes || i.taxes.length === 0);
    if (missing.length === 0) return;
    const hotelIds = [...new Set(missing.map((i) => i.hotel_id))];
    Promise.all(
      hotelIds.map((hid) =>
        apiGetHotelTaxes(hid)
          .then((taxes) => ({ hid, taxes: taxes.map((tx) => ({ taxTypeName: tx.taxTypeName, rate: tx.rate, isPercentage: tx.isPercentage })) }))
          .catch(() => ({ hid, taxes: [] as { taxTypeName: string; rate: number; isPercentage: boolean }[] }))
      )
    ).then((results) => {
      const taxMap = new Map(results.map((r) => [r.hid, r.taxes]));
      setItems((prev) =>
        prev.map((item) =>
          !item.taxes || item.taxes.length === 0
            ? { ...item, taxes: taxMap.get(item.hotel_id) ?? [] }
            : item
        )
      );
    });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const subtotal = items.reduce((sum, item) => sum + item.line_total, 0);

  // Recalculate discount whenever subtotal changes (fixes stale discount)
  // Backend applies promo % per-line and promo fixed at header — both can coexist
  const promoDiscount = useMemo(() => {
    if (!promoCode) return 0;
    let discount = 0;
    if (promoPercent > 0) discount += subtotal * (promoPercent / 100);
    if (promoFixed > 0) discount += promoFixed;
    // Fallback: check hardcoded codes (only when API didn't set values)
    if (discount === 0) {
      const hc = PROMO_CODES[promoCode];
      if (!hc) return 0;
      discount = hc.type === "percent" ? subtotal * (hc.value / 100) : hc.value;
    }
    return Math.min(discount, subtotal);
  }, [promoCode, promoPercent, promoFixed, subtotal]);

  // User-level discounts (additive %): user discount + business partner + subscription
  const userDiscountPct = (user?.discount ?? 0)
    + (user?.business_partner_discount ?? 0)
    + (user?.subscription_discount ?? 0);
  const userDiscountAmount = subtotal * userDiscountPct / 100;

  // Backend applies all %-based discounts to per-line tax base, but promo fixed amounts
  // are subtracted at header level AFTER tax. Only include actual percentages here.
  const totalDiscountPct = promoPercent + userDiscountPct;

  // Estimate taxes on discounted amounts (matches backend: taxableAmount = subtotal - discountAmount)
  const taxEstimate = useMemo(() => {
    return estimateTax(items, totalDiscountPct);
  }, [items, totalDiscountPct]);

  const total = Math.max(0, subtotal - promoDiscount - userDiscountAmount + taxEstimate);

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
    currencyCode: string,
    guestFirstName: string,
    guestLastName: string,
    _guestEmail: string,
    paymentMethodId: number,
    cardNumber?: string,
    cardExpiry?: string,
    cardCvv?: string,
    cardHolderName?: string
  ): Promise<ApiSubmitResponse | null> {
    try {
      // 1. Create draft reservation
      const reservationId = await apiCreateDraftReservation(
        currencyCode,
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

      // 3. Submit with payment
      const result = await apiSubmitReservation(
        reservationId,
        paymentMethodId,
        cardNumber,
        cardExpiry,
        cardCvv,
        cardHolderName
      );
      return result;
    } catch (err) {
      console.error("[BookingContext] submitBooking failed:", err);
      throw err;
    }
  }

  return (
    <BookingContext.Provider
      value={{ items, promoCode, promoDiscount, userDiscountAmount, taxEstimate, addItem, removeItem, clearBasket, applyPromoCode, submitBooking, subtotal, total }}
    >
      {children}
    </BookingContext.Provider>
  );
}

export function useBooking() {
  return useContext(BookingContext);
}
