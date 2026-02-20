"use client";

import { getHotelAmenities } from "@/data/amenities";
import { useLocale } from "@/context/LocaleContext";

interface FilterSidebarProps {
  stars: number[];
  onStarsChange: (stars: number[]) => void;
  minPrice: number;
  maxPrice: number;
  onMinPriceChange: (val: number) => void;
  onMaxPriceChange: (val: number) => void;
  selectedAmenities: number[];
  onAmenitiesChange: (amenities: number[]) => void;
  onReset: () => void;
}

export default function FilterSidebar({
  stars, onStarsChange,
  minPrice, maxPrice, onMinPriceChange, onMaxPriceChange,
  selectedAmenities, onAmenitiesChange,
  onReset,
}: FilterSidebarProps) {
  const { t } = useLocale();
  const hotelAmenities = getHotelAmenities();

  function toggleStar(s: number) {
    onStarsChange(stars.includes(s) ? stars.filter((x) => x !== s) : [...stars, s]);
  }

  function toggleAmenity(id: number) {
    onAmenitiesChange(
      selectedAmenities.includes(id)
        ? selectedAmenities.filter((x) => x !== id)
        : [...selectedAmenities, id]
    );
  }

  return (
    <aside className="space-y-6">
      <div className="flex items-center justify-between">
        <h3 className="font-semibold text-gray-900">{t("client.filter.filters")}</h3>
        <button onClick={onReset} className="text-sm text-blue-600 hover:underline">{t("client.filter.reset")}</button>
      </div>

      {/* Star rating */}
      <div>
        <h4 className="mb-2 text-sm font-medium text-gray-700">{t("client.filter.star_rating")}</h4>
        <div className="space-y-1">
          {[5, 4, 3].map((s) => (
            <label key={s} className="flex cursor-pointer items-center gap-2 rounded px-2 py-1 hover:bg-gray-50">
              <input
                type="checkbox"
                checked={stars.includes(s)}
                onChange={() => toggleStar(s)}
                className="h-4 w-4 rounded border-gray-300 text-blue-600"
              />
              <span className="text-sm text-gray-700">{"★".repeat(s)}{"☆".repeat(5 - s)}</span>
            </label>
          ))}
        </div>
      </div>

      {/* Price range */}
      <div>
        <h4 className="mb-2 text-sm font-medium text-gray-700">{t("client.filter.price_per_night")}</h4>
        <div className="flex items-center gap-2">
          <input
            type="number"
            min={0}
            value={minPrice || ""}
            onChange={(e) => onMinPriceChange(Number(e.target.value))}
            placeholder={t("client.filter.min")}
            className="w-full rounded border border-gray-300 px-2 py-1.5 text-sm"
          />
          <span className="text-gray-400">-</span>
          <input
            type="number"
            min={0}
            value={maxPrice || ""}
            onChange={(e) => onMaxPriceChange(Number(e.target.value))}
            placeholder={t("client.filter.max")}
            className="w-full rounded border border-gray-300 px-2 py-1.5 text-sm"
          />
        </div>
      </div>

      {/* Amenities */}
      <div>
        <h4 className="mb-2 text-sm font-medium text-gray-700">{t("client.filter.amenities")}</h4>
        <div className="space-y-1">
          {hotelAmenities.map((a) => (
            <label key={a.id} className="flex cursor-pointer items-center gap-2 rounded px-2 py-1 hover:bg-gray-50">
              <input
                type="checkbox"
                checked={selectedAmenities.includes(a.id)}
                onChange={() => toggleAmenity(a.id)}
                className="h-4 w-4 rounded border-gray-300 text-blue-600"
              />
              <span className="text-sm text-gray-700">{a.name}</span>
            </label>
          ))}
        </div>
      </div>
    </aside>
  );
}
