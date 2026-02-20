import type { Hotel, SearchFilters } from "@/types";

export function formatPrice(amount: number, currency = "EUR", locale = "en"): string {
  const intlLocale = locale === "es" ? "es-ES" : "en-US";
  return new Intl.NumberFormat(intlLocale, {
    style: "currency",
    currency,
    minimumFractionDigits: 2,
  }).format(amount);
}

export function formatDate(date: string, locale = "en"): string {
  const intlLocale = locale === "es" ? "es-ES" : "en-US";
  return new Date(date).toLocaleDateString(intlLocale, {
    month: "short",
    day: "numeric",
    year: "numeric",
  });
}

export function formatDateShort(date: string, locale = "en"): string {
  const intlLocale = locale === "es" ? "es-ES" : "en-US";
  return new Date(date).toLocaleDateString(intlLocale, {
    month: "short",
    day: "numeric",
  });
}

export function getStarArray(stars: number): boolean[] {
  return Array.from({ length: 5 }, (_, i) => i < stars);
}

export function calculateNights(checkIn: string, checkOut: string): number {
  const start = new Date(checkIn);
  const end = new Date(checkOut);
  const diff = end.getTime() - start.getTime();
  return Math.max(1, Math.ceil(diff / (1000 * 60 * 60 * 24)));
}

export function filterHotels(hotels: Hotel[], filters: Partial<SearchFilters>): Hotel[] {
  return hotels.filter((hotel) => {
    if (filters.destination) {
      const q = filters.destination.toLowerCase();
      const match =
        hotel.name.toLowerCase().includes(q) ||
        hotel.city_name.toLowerCase().includes(q) ||
        hotel.region_name.toLowerCase().includes(q) ||
        hotel.country_name.toLowerCase().includes(q);
      if (!match) return false;
    }
    if (filters.stars && filters.stars.length > 0) {
      if (!filters.stars.includes(hotel.stars)) return false;
    }
    if (filters.minPrice !== undefined && hotel.min_price < filters.minPrice) return false;
    if (filters.maxPrice !== undefined && hotel.min_price > filters.maxPrice) return false;
    if (filters.amenities && filters.amenities.length > 0) {
      const hotelAmenityIds = hotel.amenities.map((a) => a.id);
      if (!filters.amenities.every((id) => hotelAmenityIds.includes(id))) return false;
    }
    return true;
  });
}

export function sortHotels(hotels: Hotel[], sortBy: string): Hotel[] {
  const sorted = [...hotels];
  switch (sortBy) {
    case "price_asc":
      return sorted.sort((a, b) => a.min_price - b.min_price);
    case "price_desc":
      return sorted.sort((a, b) => b.min_price - a.min_price);
    case "rating":
      return sorted.sort((a, b) => b.avg_rating - a.avg_rating);
    case "stars":
      return sorted.sort((a, b) => b.stars - a.stars);
    default:
      return sorted;
  }
}

export function paginate<T>(items: T[], page: number, perPage: number): { items: T[]; total: number } {
  const start = (page - 1) * perPage;
  return {
    items: items.slice(start, start + perPage),
    total: items.length,
  };
}

export function generateReservationCode(): string {
  const chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
  let code = "VA-";
  for (let i = 0; i < 8; i++) {
    code += chars[Math.floor(Math.random() * chars.length)];
  }
  return code;
}

export function getDefaultCheckIn(): string {
  const d = new Date();
  d.setDate(d.getDate() + 14);
  return d.toISOString().split("T")[0];
}

export function getDefaultCheckOut(): string {
  const d = new Date();
  d.setDate(d.getDate() + 17);
  return d.toISOString().split("T")[0];
}
