import type { Region } from "@/types";

export const regions: Region[] = [
  {
    id: 3,
    name: "Mallorca",
    country: "Spain",
    cities: [
      { id: 1, name: "Palma", administrative_division_id: 3 },
      { id: 2, name: "Calvià", administrative_division_id: 3 },
      { id: 3, name: "Manacor", administrative_division_id: 3 },
      { id: 4, name: "Alcúdia", administrative_division_id: 3 },
      { id: 5, name: "Sóller", administrative_division_id: 3 },
      { id: 6, name: "Pollença", administrative_division_id: 3 },
      { id: 7, name: "Inca", administrative_division_id: 3 },
      { id: 8, name: "Artà", administrative_division_id: 3 },
      { id: 9, name: "Felanitx", administrative_division_id: 3 },
      { id: 10, name: "Valldemossa", administrative_division_id: 3 },
    ],
  },
  {
    id: 4,
    name: "Menorca",
    country: "Spain",
    cities: [
      { id: 11, name: "Mahón", administrative_division_id: 4 },
      { id: 12, name: "Ciutadella", administrative_division_id: 4 },
    ],
  },
  {
    id: 5,
    name: "Ibiza",
    country: "Spain",
    cities: [
      { id: 21, name: "Ibiza", administrative_division_id: 5 },
      { id: 22, name: "Santa Eulària des Riu", administrative_division_id: 5 },
    ],
  },
  {
    id: 6,
    name: "Barcelona",
    country: "Spain",
    cities: [{ id: 31, name: "Barcelona", administrative_division_id: 6 }],
  },
  {
    id: 8,
    name: "Côte d'Azur",
    country: "France",
    cities: [{ id: 32, name: "Nice", administrative_division_id: 8 }],
  },
];

export function getCityName(cityId: number): string {
  for (const region of regions) {
    const city = region.cities.find((c) => c.id === cityId);
    if (city) return city.name;
  }
  return "Unknown";
}

export function getRegionForCity(cityId: number): Region | undefined {
  return regions.find((r) => r.cities.some((c) => c.id === cityId));
}

export function getAllDestinations(): string[] {
  const destinations: string[] = [];
  for (const region of regions) {
    destinations.push(region.name);
    for (const city of region.cities) {
      destinations.push(city.name);
    }
  }
  return destinations;
}
