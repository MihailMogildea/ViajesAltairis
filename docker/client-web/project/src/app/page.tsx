"use client";

import SearchBar from "@/components/SearchBar";
import DestinationCard from "@/components/DestinationCard";
import HotelCard from "@/components/HotelCard";
import { hotels } from "@/data/hotels";
import { useLocale } from "@/context/LocaleContext";

const destinations = [
  { name: "Mallorca", country: "Spain", hotelCount: 12 },
  { name: "Barcelona", country: "Spain", hotelCount: 3 },
  { name: "Ibiza", country: "Spain", hotelCount: 2 },
  { name: "Menorca", country: "Spain", hotelCount: 2 },
  { name: "Côte d'Azur", country: "France", hotelCount: 3 },
];

const popularHotels = hotels
  .filter((h) => h.avg_rating > 0)
  .sort((a, b) => b.avg_rating - a.avg_rating)
  .slice(0, 4);

export default function HomePage() {
  const { t } = useLocale();

  return (
    <div>
      {/* Hero */}
      <section className="relative bg-gradient-to-br from-blue-600 to-blue-800 px-4 py-16 sm:py-24">
        <div className="mx-auto max-w-7xl">
          <div className="mb-8 text-center">
            <h1 className="text-3xl font-bold text-white sm:text-5xl">
              {t("client.home.hero_title")}
            </h1>
            <p className="mt-3 text-lg text-blue-100">
              {t("client.home.hero_subtitle")}
            </p>
          </div>
          <div className="rounded-xl bg-white p-4 shadow-xl sm:p-6">
            <SearchBar />
          </div>
        </div>
      </section>

      {/* Featured destinations */}
      <section className="px-4 py-12 sm:px-6">
        <div className="mx-auto max-w-7xl">
          <h2 className="mb-6 text-2xl font-bold text-gray-900">{t("client.home.popular_destinations")}</h2>
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-5">
            {destinations.map((d) => (
              <DestinationCard key={d.name} {...d} />
            ))}
          </div>
        </div>
      </section>

      {/* Popular hotels */}
      <section className="bg-gray-50 px-4 py-12 sm:px-6">
        <div className="mx-auto max-w-7xl">
          <h2 className="mb-6 text-2xl font-bold text-gray-900">{t("client.home.top_rated")}</h2>
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-4">
            {popularHotels.map((hotel) => (
              <HotelCard key={hotel.id} hotel={hotel} />
            ))}
          </div>
        </div>
      </section>

      {/* Why book with us */}
      <section className="px-4 py-12 sm:px-6">
        <div className="mx-auto max-w-7xl">
        <h2 className="mb-8 text-center text-2xl font-bold text-gray-900">{t("client.home.why_book")}</h2>
        <div className="grid gap-8 sm:grid-cols-3">
          <div className="text-center">
            <div className="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-blue-100 text-2xl">
              <span>&#9733;</span>
            </div>
            <h3 className="font-semibold text-gray-900">{t("client.home.best_price")}</h3>
            <p className="mt-1 text-sm text-gray-500">{t("client.home.best_price_desc")}</p>
          </div>
          <div className="text-center">
            <div className="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-blue-100 text-2xl">
              <span>&#128274;</span>
            </div>
            <h3 className="font-semibold text-gray-900">{t("client.home.secure_booking")}</h3>
            <p className="mt-1 text-sm text-gray-500">{t("client.home.secure_booking_desc")}</p>
          </div>
          <div className="text-center">
            <div className="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-blue-100 text-2xl">
              <span>&#128222;</span>
            </div>
            <h3 className="font-semibold text-gray-900">{t("client.home.support_24")}</h3>
            <p className="mt-1 text-sm text-gray-500">{t("client.home.support_24_desc")}</p>
          </div>
        </div>
        </div>
      </section>
    </div>
  );
}
