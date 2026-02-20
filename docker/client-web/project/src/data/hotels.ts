import type { Hotel } from "@/types";
import { getAmenitiesForHotel } from "./amenities";
import { getAverageRating, getReviewsForHotel } from "./reviews";
import { getRoomsForHotel } from "./rooms";

function img(name: string, n: number, color: string): { id: number; hotel_id: number; url: string; alt_text: string; sort_order: number } {
  const text = encodeURIComponent(name);
  return { id: n, hotel_id: 0, url: `https://placehold.co/800x500/${color}/FFFFFF?text=${text}`, alt_text: name, sort_order: n };
}

function hotelImages(id: number, name: string, stars: number): Hotel["images"] {
  const colors = ["2563EB", "0891B2", "059669", "7C3AED", "DC2626", "D97706"];
  const count = stars === 5 ? 4 : stars === 4 ? 3 : 2;
  return Array.from({ length: count }, (_, i) => ({
    ...img(name, i + 1, colors[i % colors.length]),
    id: id * 10 + i,
    hotel_id: id,
  }));
}

function minPrice(hotelId: number): number {
  const rooms = getRoomsForHotel(hotelId);
  if (rooms.length === 0) return 0;
  return Math.min(...rooms.map((r) => r.base_price_per_night));
}

interface HotelDef {
  id: number;
  name: string;
  city_id: number;
  city_name: string;
  region_name: string;
  country_name: string;
  stars: number;
  latitude: number;
  longitude: number;
  check_in_time: string;
  check_out_time: string;
  phone: string;
  email: string;
  website: string;
  margin: number;
  summary: string;
  description: string;
}

const hotelDefs: HotelDef[] = [
  { id: 1, name: "Hotel Altairis Palma", city_id: 1, city_name: "Palma", region_name: "Mallorca", country_name: "Spain", stars: 5, latitude: 39.5696, longitude: 2.6502, check_in_time: "15:00", check_out_time: "11:00", phone: "+34 971 000 001", email: "palma@altairis.com", website: "altairis.com", margin: 0, summary: "Luxury 5-star hotel on Palma's iconic Paseo Marítimo with stunning bay views.", description: "Experience the pinnacle of Mediterranean luxury at Hotel Altairis Palma. Set along the prestigious Paseo Marítimo, our flagship property offers panoramic views of the Bay of Palma, world-class dining, a full-service spa, and elegantly appointed rooms that blend modern comfort with Mallorcan charm." },
  { id: 2, name: "Hotel Sol de Palma", city_id: 1, city_name: "Palma", region_name: "Mallorca", country_name: "Spain", stars: 3, latitude: 39.5678, longitude: 2.6489, check_in_time: "14:00", check_out_time: "12:00", phone: "+34 971 000 002", email: "solpalma@altairis.com", website: "altairis.com", margin: 0, summary: "Charming budget-friendly hotel in the heart of Palma's old town.", description: "Hotel Sol de Palma offers comfortable, affordable accommodation in a prime central location. Steps from Palma's famous La Lonja neighbourhood, our cosy rooms provide everything you need for an enjoyable city break without breaking the bank." },
  { id: 3, name: "Hotel Playa de Palma", city_id: 1, city_name: "Palma", region_name: "Mallorca", country_name: "Spain", stars: 4, latitude: 39.5245, longitude: 2.742, check_in_time: "15:00", check_out_time: "11:00", phone: "+34 971 000 010", email: "playa@altairis.com", website: "altairis.com", margin: 0, summary: "Beachfront 4-star hotel with pool and direct beach access.", description: "Located right on Playa de Palma's golden sands, this modern hotel is perfect for beach lovers. Enjoy direct beach access, a sparkling pool area, and excellent half-board dining options. Ideal for families and couples seeking sun, sea, and relaxation." },
  { id: 4, name: "Hotel Casco Antiguo", city_id: 1, city_name: "Palma", region_name: "Mallorca", country_name: "Spain", stars: 3, latitude: 39.571, longitude: 2.6495, check_in_time: "14:00", check_out_time: "12:00", phone: "+34 971 000 011", email: "casco@altairis.com", website: "altairis.com", margin: 0, summary: "Budget hotel in Palma's historic quarter, walking distance to the cathedral.", description: "Nestled in the winding streets of Palma's Casco Antiguo, this charming hotel puts you at the doorstep of the city's most iconic landmarks. The cathedral, Royal Palace, and Arab Baths are all within a short walk. A great base for budget-conscious travellers." },
  { id: 5, name: "Hotel Bellver Park", city_id: 1, city_name: "Palma", region_name: "Mallorca", country_name: "Spain", stars: 4, latitude: 39.5635, longitude: 2.623, check_in_time: "15:00", check_out_time: "11:00", phone: "+34 971 000 012", email: "bellver@altairis.com", website: "altairis.com", margin: 0, summary: "Elegant 4-star hotel near Bellver Castle with beautiful parkland views.", description: "Set in a tranquil area near the historic Bellver Castle, this elegant hotel offers a peaceful retreat with stunning views over lush parkland. The on-site restaurant, pool, and gym make it perfect for both relaxation and active holidays." },
  { id: 6, name: "Hotel Marina Palma", city_id: 1, city_name: "Palma", region_name: "Mallorca", country_name: "Spain", stars: 5, latitude: 39.572, longitude: 2.638, check_in_time: "16:00", check_out_time: "11:00", phone: "+34 971 000 013", email: "marina@altairis.com", website: "altairis.com", margin: 0, summary: "Premium 5-star marina hotel with world-class spa and Mediterranean cuisine.", description: "Overlooking Palma's glamorous marina, Hotel Marina Palma is a sanctuary of luxury. Our world-class spa, Michelin-inspired restaurant, and beautifully designed suites offer an unforgettable experience. Watch superyachts glide by from your private balcony." },
  { id: 7, name: "Hotel Catedral Palma", city_id: 1, city_name: "Palma", region_name: "Mallorca", country_name: "Spain", stars: 4, latitude: 39.568, longitude: 2.648, check_in_time: "15:00", check_out_time: "12:00", phone: "+34 971 000 014", email: "catedral@altairis.com", website: "altairis.com", margin: 0, summary: "Boutique 4-star hotel with cathedral views in the historic centre.", description: "Wake up to breathtaking views of Palma Cathedral from this boutique hotel in the heart of the historic centre. Combining old-world charm with modern comforts, Hotel Catedral Palma is ideal for culture lovers and city explorers." },
  { id: 8, name: "Hotel Portixol", city_id: 1, city_name: "Palma", region_name: "Mallorca", country_name: "Spain", stars: 3, latitude: 39.561, longitude: 2.665, check_in_time: "14:00", check_out_time: "11:00", phone: "+34 971 000 015", email: "portixol@altairis.com", website: "altairis.com", margin: 0, summary: "Seaside budget hotel in the charming Portixol fishing village.", description: "Located in the picturesque former fishing village of Portixol, this affordable hotel offers a relaxed seaside atmosphere. Enjoy morning walks along the promenade and fresh seafood at the local restaurants just steps away." },
  { id: 9, name: "Hotel Santa Catalina", city_id: 1, city_name: "Palma", region_name: "Mallorca", country_name: "Spain", stars: 4, latitude: 39.573, longitude: 2.635, check_in_time: "15:00", check_out_time: "11:00", phone: "+34 971 000 016", email: "santacatalina@altairis.com", website: "altairis.com", margin: 0, summary: "Trendy 4-star hotel in Palma's coolest neighbourhood.", description: "In the heart of Santa Catalina, Palma's trendiest neighbourhood, this stylish hotel is surrounded by artisan cafés, boutique shops, and the famous Mercat de Santa Catalina. Modern rooms with rooftop views make this the perfect urban retreat." },
  { id: 10, name: "Hotel Son Vida Palace", city_id: 1, city_name: "Palma", region_name: "Mallorca", country_name: "Spain", stars: 5, latitude: 39.589, longitude: 2.618, check_in_time: "15:00", check_out_time: "12:00", phone: "+34 971 000 017", email: "sonvida@altairis.com", website: "altairis.com", margin: 0, summary: "Palatial 5-star estate with golf course and panoramic bay views.", description: "Perched on a hilltop overlooking the Bay of Palma, Hotel Son Vida Palace is the island's most prestigious address. This palatial estate features an 18-hole golf course, Michelin-starred dining, a world-class spa, and rooms of extraordinary opulence." },
  { id: 11, name: "Hotel Bahía Alcúdia", city_id: 4, city_name: "Alcúdia", region_name: "Mallorca", country_name: "Spain", stars: 4, latitude: 39.853, longitude: 3.121, check_in_time: "15:00", check_out_time: "11:00", phone: "+34 971 000 003", email: "alcudia@altairis.com", website: "altairis.com", margin: 0, summary: "Family-friendly 4-star resort on Alcúdia's famous beach.", description: "Enjoy the crystal-clear waters of Alcúdia Bay from this family-friendly resort. With a kids' club, multiple pools, and direct beach access, it's the perfect destination for families seeking fun in the sun on Mallorca's beautiful north coast." },
  { id: 12, name: "Hotel Serra de Tramuntana", city_id: 5, city_name: "Sóller", region_name: "Mallorca", country_name: "Spain", stars: 4, latitude: 39.7667, longitude: 2.715, check_in_time: "15:00", check_out_time: "11:00", phone: "+34 971 000 004", email: "soller@altairis.com", website: "altairis.com", margin: 0, summary: "Mountain retreat in the stunning Tramuntana range, near Sóller.", description: "Nestled in the UNESCO-protected Serra de Tramuntana mountains, this boutique hotel offers a peaceful escape surrounded by orange groves and dramatic peaks. Take the historic wooden tram to Port de Sóller and explore one of Mallorca's most charming villages." },
  { id: 13, name: "Hotel Port Mahón", city_id: 11, city_name: "Mahón", region_name: "Menorca", country_name: "Spain", stars: 4, latitude: 39.8886, longitude: 4.2658, check_in_time: "15:00", check_out_time: "11:00", phone: "+34 971 000 005", email: "mahon@altairis.com", website: "altairis.com", margin: 0, summary: "Harbourfront 4-star hotel overlooking Mahón's natural port.", description: "Overlooking Europe's second-largest natural harbour, Hotel Port Mahón offers stunning waterfront views and easy access to Menorca's capital. Explore the charming streets, sample local gin, and discover the island's rich British heritage." },
  { id: 14, name: "Hotel Ciutadella Mar", city_id: 12, city_name: "Ciutadella", region_name: "Menorca", country_name: "Spain", stars: 3, latitude: 40.0011, longitude: 3.8374, check_in_time: "14:00", check_out_time: "12:00", phone: "+34 971 000 006", email: "ciutadella@altairis.com", website: "altairis.com", margin: 0, summary: "Affordable hotel in Ciutadella's historic Plaça des Born.", description: "Stay in the heart of Ciutadella, Menorca's former capital, at this welcoming budget hotel. Located on the magnificent Plaça des Born, you're steps from Gothic churches, artisan shops, and the picturesque old port." },
  { id: 15, name: "Hotel Dalt Vila", city_id: 21, city_name: "Ibiza", region_name: "Ibiza", country_name: "Spain", stars: 5, latitude: 38.9067, longitude: 1.4363, check_in_time: "15:00", check_out_time: "11:00", phone: "+34 971 000 007", email: "ibiza@altairis.com", website: "altairis.com", margin: 0, summary: "Exclusive 5-star hotel in Ibiza's UNESCO World Heritage fortress.", description: "Located within the ancient walls of Dalt Vila, Ibiza's UNESCO-listed fortress, this exclusive hotel combines centuries of history with contemporary luxury. Enjoy sunset cocktails with panoramic views over the Mediterranean and the old town's labyrinthine streets." },
  { id: 16, name: "Hotel Santa Eulària Beach", city_id: 22, city_name: "Santa Eulària des Riu", region_name: "Ibiza", country_name: "Spain", stars: 4, latitude: 38.9847, longitude: 1.5339, check_in_time: "15:00", check_out_time: "11:00", phone: "+34 971 000 008", email: "santaeularia@altairis.com", website: "altairis.com", margin: 0, summary: "Relaxed 4-star beachfront hotel in family-friendly Santa Eulària.", description: "On the beautiful promenade of Santa Eulària des Riu, this relaxed beachfront hotel is perfect for those who prefer Ibiza's quieter side. Pristine beaches, excellent restaurants, and a charming marina are all at your doorstep." },
  { id: 17, name: "Hotel Altairis Barcelona", city_id: 31, city_name: "Barcelona", region_name: "Barcelona", country_name: "Spain", stars: 5, latitude: 41.3925, longitude: 2.1648, check_in_time: "15:00", check_out_time: "11:00", phone: "+34 933 000 001", email: "barcelona@altairis.com", website: "altairis.com", margin: 0, summary: "Flagship 5-star hotel on Passeig de Gràcia, near Gaudí's masterpieces.", description: "Our Barcelona flagship occupies a prime position on Passeig de Gràcia, steps from Casa Batlló and La Pedrera. This architectural gem features a rooftop pool with views of the Sagrada Família, a celebrated restaurant, and impeccably designed rooms." },
  { id: 18, name: "Hotel Gótico", city_id: 31, city_name: "Barcelona", region_name: "Barcelona", country_name: "Spain", stars: 3, latitude: 41.38, longitude: 2.177, check_in_time: "14:00", check_out_time: "12:00", phone: "+34 933 000 002", email: "gotico@altairis.com", website: "altairis.com", margin: 0, summary: "Budget-friendly hotel in Barcelona's atmospheric Gothic Quarter.", description: "Immerse yourself in Barcelona's medieval heart at Hotel Gótico. Located on a quiet street in the Gothic Quarter, you're moments from the cathedral, Plaça Reial, and Las Ramblas. Clean, comfortable rooms at an unbeatable price for this central location." },
  { id: 19, name: "Hotel Barceloneta Mar", city_id: 31, city_name: "Barcelona", region_name: "Barcelona", country_name: "Spain", stars: 4, latitude: 41.378, longitude: 2.192, check_in_time: "15:00", check_out_time: "11:00", phone: "+34 933 000 003", email: "barceloneta@altairis.com", website: "altairis.com", margin: 0, summary: "Seafront 4-star hotel in Barcelona's vibrant Barceloneta beach district.", description: "Feel the sand between your toes at Hotel Barceloneta Mar. This modern seafront hotel offers direct beach access, sea-view rooms, and proximity to Barcelona's best chiringuitos and seafood restaurants. The perfect blend of city and beach." },
  { id: 20, name: "Hotel Promenade Nice", city_id: 32, city_name: "Nice", region_name: "Côte d'Azur", country_name: "France", stars: 5, latitude: 43.6953, longitude: 7.265, check_in_time: "15:00", check_out_time: "11:00", phone: "+33 493 000 001", email: "nice@altairis.com", website: "altairis.com", margin: 0, summary: "Iconic 5-star hotel on the Promenade des Anglais with Riviera views.", description: "On the legendary Promenade des Anglais, Hotel Promenade Nice epitomises French Riviera elegance. Azure sea views, a rooftop infinity pool, Michelin-starred cuisine, and palatial rooms make this the quintessential Côte d'Azur experience." },
  { id: 21, name: "Hotel Vieux Nice", city_id: 32, city_name: "Nice", region_name: "Côte d'Azur", country_name: "France", stars: 3, latitude: 43.696, longitude: 7.275, check_in_time: "14:00", check_out_time: "12:00", phone: "+33 493 000 002", email: "vieuxnice@altairis.com", website: "altairis.com", margin: 0, summary: "Charming budget hotel in the colourful streets of Old Nice.", description: "Discover the charm of Vieux Nice from this affordable hotel in the heart of the old town. Surrounded by bustling markets, Baroque churches, and authentic Niçois restaurants, it's the perfect base for exploring the city on foot." },
  { id: 22, name: "Hotel Colline de Cimiez", city_id: 32, city_name: "Nice", region_name: "Côte d'Azur", country_name: "France", stars: 4, latitude: 43.715, longitude: 7.275, check_in_time: "15:00", check_out_time: "11:00", phone: "+33 493 000 003", email: "cimiez@altairis.com", website: "altairis.com", margin: 0, summary: "Refined 4-star hotel in Nice's elegant Cimiez hilltop quarter.", description: "In the exclusive Cimiez quarter, home to the Matisse Museum and Roman ruins, this refined hotel offers a serene escape above the city. Beautiful gardens, a gourmet restaurant, and spacious rooms with views over Nice and the Mediterranean." },
];

export const hotels: Hotel[] = hotelDefs.map((h) => ({
  ...h,
  enabled: true,
  images: hotelImages(h.id, h.name, h.stars),
  amenities: getAmenitiesForHotel(h.id),
  min_price: minPrice(h.id),
  avg_rating: getAverageRating(h.id),
  review_count: getReviewsForHotel(h.id).length,
}));

export function getHotelById(id: number): Hotel | undefined {
  return hotels.find((h) => h.id === id);
}
