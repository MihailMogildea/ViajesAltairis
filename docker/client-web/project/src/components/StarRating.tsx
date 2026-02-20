"use client";

import { getStarArray } from "@/lib/utils";

export default function StarRating({ stars, size = "md" }: { stars: number; size?: "sm" | "md" | "lg" }) {
  const sizeClass = size === "sm" ? "text-sm" : size === "lg" ? "text-xl" : "text-base";
  return (
    <span className={`inline-flex gap-0.5 ${sizeClass}`} aria-label={`${stars} stars`}>
      {getStarArray(stars).map((filled, i) => (
        <span key={i} className={filled ? "text-yellow-400" : "text-gray-300"}>
          ★
        </span>
      ))}
    </span>
  );
}
