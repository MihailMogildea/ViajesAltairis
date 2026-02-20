import type { Amenity } from "@/types";

export const amenities: Amenity[] = [
  { id: 1, name: "Pool", category: "hotel", icon: "pool" },
  { id: 2, name: "Gym", category: "hotel", icon: "gym" },
  { id: 3, name: "Spa", category: "hotel", icon: "spa" },
  { id: 4, name: "Restaurant", category: "hotel", icon: "restaurant" },
  { id: 5, name: "Parking", category: "hotel", icon: "parking" },
  { id: 6, name: "Beach Access", category: "hotel", icon: "beach_access" },
  { id: 7, name: "Bar", category: "hotel", icon: "bar" },
  { id: 8, name: "Garden", category: "hotel", icon: "garden" },
  { id: 9, name: "Air Conditioning", category: "room", icon: "air_conditioning" },
  { id: 10, name: "TV", category: "room", icon: "tv" },
  { id: 11, name: "WiFi", category: "room", icon: "wifi" },
  { id: 12, name: "Minibar", category: "room", icon: "minibar" },
  { id: 13, name: "Safe", category: "room", icon: "safe" },
  { id: 14, name: "Balcony", category: "room", icon: "balcony" },
  { id: 15, name: "Jacuzzi", category: "room", icon: "jacuzzi" },
  { id: 16, name: "Room Service", category: "room", icon: "room_service" },
  { id: 17, name: "Hair Dryer", category: "room", icon: "hair_dryer" },
  { id: 18, name: "Coffee Maker", category: "room", icon: "coffee_machine" },
];

export const hotelAmenities: Record<number, number[]> = {
  1: [1, 2, 3, 4, 5, 6, 7, 8],
  2: [4, 7],
  3: [1, 4, 5, 6, 7],
  4: [4, 7],
  5: [1, 2, 4, 5, 8],
  6: [1, 2, 3, 4, 5, 6, 7, 8],
  7: [4, 5, 7],
  8: [4, 6],
  9: [1, 4, 5, 7, 8],
  10: [1, 2, 3, 4, 5, 7, 8],
  11: [1, 4, 5, 6, 7],
  12: [4, 5, 8],
  13: [1, 4, 5, 7],
  14: [4, 7],
  15: [1, 2, 3, 4, 5, 7, 8],
  16: [1, 4, 5, 6, 7],
  17: [1, 2, 3, 4, 5, 7],
  18: [4, 7],
  19: [1, 4, 5, 6, 7],
  20: [1, 2, 3, 4, 5, 6, 7, 8],
  21: [4, 7],
  22: [2, 4, 5, 8],
};

export function getAmenitiesForHotel(hotelId: number): Amenity[] {
  const ids = hotelAmenities[hotelId] || [];
  return amenities.filter((a) => ids.includes(a.id));
}

export function getHotelAmenities(): Amenity[] {
  return amenities.filter((a) => a.category === "hotel");
}
