/* eslint-disable @next/next/no-img-element */
"use client";

import Link from "next/link";
import { useLocale } from "@/context/LocaleContext";

interface DestinationCardProps {
  name: string;
  country: string;
  hotelCount: number;
}

export default function DestinationCard({ name, country, hotelCount }: DestinationCardProps) {
  const { t } = useLocale();
  const seed = name.toLowerCase().replace(/[^a-z0-9]+/g, "-");
  return (
    <Link
      href={`/hotels?destination=${encodeURIComponent(name)}`}
      className="group relative overflow-hidden rounded-xl"
    >
      <div className="aspect-[4/3] w-full">
        <img
          src={`https://picsum.photos/seed/dest-${seed}/400/300`}
          alt={name}
          className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-105"
        />
      </div>
      <div className="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent" />
      <div className="absolute bottom-0 left-0 p-4">
        <h3 className="text-lg font-bold text-white">{name}</h3>
        <p className="text-sm text-white/80">{country} &middot; {hotelCount} {hotelCount === 1 ? t("client.destination.hotel") : t("client.destination.hotels")}</p>
      </div>
    </Link>
  );
}
