import type { Amenity } from "@/types";

const iconMap: Record<string, string> = {
  wifi: "📶",
  pool: "🏊",
  spa: "💆",
  gym: "🏋️",
  restaurant: "🍽️",
  bar: "🍸",
  parking: "🅿️",
  concierge: "🛎️",
  air_conditioning: "❄️",
  minibar: "🧊",
  safe: "🔒",
  tv: "📺",
  balcony: "🌅",
  sea_view: "🌊",
  bathrobe: "🧖",
  coffee_machine: "☕",
  room_service: "🛏️",
  jacuzzi: "🛁",
};

export default function AmenityBadge({ amenity }: { amenity: Amenity }) {
  return (
    <span className="inline-flex items-center gap-1.5 rounded-full bg-gray-100 px-3 py-1 text-sm text-gray-700">
      <span>{iconMap[amenity.icon] || "✦"}</span>
      <span>{amenity.name}</span>
    </span>
  );
}
