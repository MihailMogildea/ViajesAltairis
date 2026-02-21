/* eslint-disable @next/next/no-img-element */
"use client";

import Link from "next/link";
import type { Hotel } from "@/types";
import StarRating from "./StarRating";
import { formatPrice } from "@/lib/utils";
import { useLocale } from "@/context/LocaleContext";

function buildQuery(checkIn?: string, checkOut?: string, guests?: number): string {
  const params = new URLSearchParams();
  if (checkIn) params.set("checkIn", checkIn);
  if (checkOut) params.set("checkOut", checkOut);
  if (guests && guests !== 2) params.set("guests", String(guests));
  const qs = params.toString();
  return qs ? `?${qs}` : "";
}

export default function HotelCard({ hotel, checkIn, checkOut, guests }: { hotel: Hotel; checkIn?: string; checkOut?: string; guests?: number }) {
  const { locale, currency, t } = useLocale();
  const mainImage = hotel.images[0];

  return (
    <Link href={`/hotels/${hotel.id}${buildQuery(checkIn, checkOut, guests)}`} className="group flex flex-col overflow-hidden rounded-xl border border-gray-200 bg-white transition-shadow hover:shadow-lg">
      <div className="aspect-[16/10] overflow-hidden bg-gray-100">
        {mainImage && (
          <img
            src={mainImage.url}
            alt={mainImage.alt_text}
            className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-105"
          />
        )}
      </div>
      <div className="flex flex-1 flex-col p-4">
        <div className="flex items-start justify-between gap-2">
          <div>
            <h3 className="font-semibold text-gray-900 group-hover:text-blue-600 transition-colors">{hotel.name}</h3>
            <p className="text-sm text-gray-500">{hotel.city_name}, {hotel.region_name}</p>
          </div>
          <StarRating stars={hotel.stars} size="sm" />
        </div>
        <p className="mt-2 flex-1 text-sm text-gray-600 line-clamp-2">{hotel.summary}</p>
        <div className="mt-3 flex items-end justify-between border-t border-gray-100 pt-3">
          <div className="flex items-center gap-2">
            {hotel.avg_rating > 0 && (
              <span className="inline-flex items-center gap-1 rounded bg-blue-50 px-2 py-0.5 text-sm font-medium text-blue-700">
                {hotel.avg_rating.toFixed(1)}
              </span>
            )}
            {hotel.review_count > 0 && (
              <span className="text-xs text-gray-400">{hotel.review_count} {hotel.review_count === 1 ? t("client.hotel_card.review") : t("client.hotel_card.reviews")}</span>
            )}
          </div>
          <div className="text-right">
            <span className="text-lg font-bold text-gray-900">{formatPrice(hotel.min_price, currency.code, locale, currency.exchangeRateToEur)}</span>
            <span className="block text-xs text-gray-400">{t("client.hotel_card.per_night")}</span>
          </div>
        </div>
      </div>
    </Link>
  );
}
