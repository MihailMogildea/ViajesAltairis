"use client";

import Link from "next/link";
import { useBooking } from "@/context/BookingContext";
import { formatPrice } from "@/lib/utils";
import { useLocale } from "@/context/LocaleContext";

export default function BookingBasket() {
  const { items, subtotal, promoDiscount, userDiscountAmount, taxEstimate, total, removeItem } = useBooking();
  const { locale, currency, t } = useLocale();

  if (items.length === 0) return null;

  return (
    <div className="rounded-xl border border-blue-200 bg-blue-50 p-4">
      <h3 className="font-semibold text-gray-900">{t("client.basket.title")} ({items.length})</h3>
      <div className="mt-3 space-y-2">
        {items.map((item) => (
          <div key={item.id} className="flex items-center justify-between text-sm text-gray-600">
            <span className="truncate pr-2"><span className="font-medium">{item.room_type_name}</span> &middot; {item.hotel_name}</span>
            <span className="flex shrink-0 items-center gap-1">
              <span className="font-medium">{formatPrice(item.line_total, currency.code, locale, currency.exchangeRateToEur)}</span>
              <button
                onClick={() => removeItem(item.id)}
                className="ml-1 rounded p-0.5 text-gray-400 hover:bg-red-50 hover:text-red-500 transition-colors"
                title={t("client.basket.remove")}
              >
                <svg className="h-3.5 w-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
                  <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </span>
          </div>
        ))}
      </div>
      <div className="mt-3 border-t border-blue-200 pt-3">
        {promoDiscount > 0 && (
          <div className="flex justify-between text-sm text-green-600">
            <span>{t("client.basket.discount")}</span>
            <span>-{formatPrice(promoDiscount, currency.code, locale, currency.exchangeRateToEur)}</span>
          </div>
        )}
        {userDiscountAmount > 0 && (
          <div className="flex justify-between text-sm text-green-600">
            <span>{t("client.booking.member_discount")}</span>
            <span>-{formatPrice(userDiscountAmount, currency.code, locale, currency.exchangeRateToEur)}</span>
          </div>
        )}
        {taxEstimate > 0 && (
          <div className="flex justify-between text-sm text-gray-500">
            <span>{t("client.booking.tax")}</span>
            <span>{formatPrice(taxEstimate, currency.code, locale, currency.exchangeRateToEur)}</span>
          </div>
        )}
        <div className="flex justify-between font-semibold text-gray-900">
          <span>{t("client.basket.total")}</span>
          <span>{formatPrice(total, currency.code, locale, currency.exchangeRateToEur)}</span>
        </div>
      </div>
      <Link href="/booking" className="mt-3 block rounded-lg bg-blue-600 py-2 text-center text-sm font-semibold text-white hover:bg-blue-700 transition-colors">
        {t("client.basket.view")}
      </Link>
    </div>
  );
}
