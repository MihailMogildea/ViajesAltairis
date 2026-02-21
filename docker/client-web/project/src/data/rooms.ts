import type { RoomConfig, RoomImage, BoardOption } from "@/types";

const ROOM_TYPE_NAMES: Record<number, string> = {
  1: "Single",
  2: "Double",
  3: "Twin",
  4: "Suite",
  5: "Junior Suite",
  6: "Deluxe",
};

// Provider margin by hotel ID (provider_margin + hotel_margin)
// Hotels 1-12: Mallorca 15%, Hotels 13-14: Menorca 15%, Hotels 15-16: Ibiza 15%
// Hotels 17-19: Peninsula 12%, Hotels 20-22: France 10%
function marginFactor(hotelId: number): number {
  if (hotelId <= 12) return 1.15;
  if (hotelId <= 14) return 1.15;
  if (hotelId <= 16) return 1.15;
  if (hotelId <= 19) return 1.12;
  return 1.10;
}

function applyMargin(price: number, hotelId: number): number {
  return Math.round(price * marginFactor(hotelId) * 100) / 100;
}

function boards(star: 3 | 4 | 5, roomTypeId: number, hprtId: number, hotelId: number): BoardOption[] {
  // 3-star: room_only, B&B, half_board
  // 4-star: room_only, B&B, half_board, full_board
  // 5-star: all five
  const base: BoardOption[] = [
    { board_type_id: 1, board_type_name: "Room Only", board_type_code: "room_only", price_supplement: 0 },
  ];
  const supplements = boardPricing[hprtId];
  if (!supplements) return base;
  for (const s of supplements) {
    base.push({ ...s, price_supplement: applyMargin(s.price_supplement, hotelId) });
  }
  return base;
}

// Board pricing by hotel_provider_room_type_id
const boardPricing: Record<number, BoardOption[]> = {};

function addBoards(hprtId: number, entries: [number, string, string, number][]) {
  boardPricing[hprtId] = entries.map(([id, name, code, price]) => ({
    board_type_id: id,
    board_type_name: name,
    board_type_code: code,
    price_supplement: price,
  }));
}

// Hotel Altairis Palma (5 stars) - hprt 1-4
addBoards(1, [[2,"Bed & Breakfast","bed_and_breakfast",30],[3,"Half Board","half_board",55],[4,"Full Board","full_board",85],[5,"All Inclusive","all_inclusive",120]]);
addBoards(2, [[2,"Bed & Breakfast","bed_and_breakfast",30],[3,"Half Board","half_board",55],[4,"Full Board","full_board",85],[5,"All Inclusive","all_inclusive",120]]);
addBoards(3, [[2,"Bed & Breakfast","bed_and_breakfast",35],[3,"Half Board","half_board",60],[4,"Full Board","full_board",90],[5,"All Inclusive","all_inclusive",130]]);
addBoards(4, [[2,"Bed & Breakfast","bed_and_breakfast",35],[3,"Half Board","half_board",60],[4,"Full Board","full_board",90],[5,"All Inclusive","all_inclusive",130]]);

// Sol de Palma (3 stars) - hprt 5-7
addBoards(5, [[2,"Bed & Breakfast","bed_and_breakfast",10],[3,"Half Board","half_board",25]]);
addBoards(6, [[2,"Bed & Breakfast","bed_and_breakfast",10],[3,"Half Board","half_board",25]]);
addBoards(7, [[2,"Bed & Breakfast","bed_and_breakfast",12],[3,"Half Board","half_board",28]]);

// Playa de Palma (4 stars) - hprt 8-10
addBoards(8, [[2,"Bed & Breakfast","bed_and_breakfast",18],[3,"Half Board","half_board",40],[4,"Full Board","full_board",60]]);
addBoards(9, [[2,"Bed & Breakfast","bed_and_breakfast",18],[3,"Half Board","half_board",40],[4,"Full Board","full_board",60]]);
addBoards(10, [[2,"Bed & Breakfast","bed_and_breakfast",22],[3,"Half Board","half_board",45],[4,"Full Board","full_board",65]]);

// Casco Antiguo (3 stars) - hprt 11-13
addBoards(11, [[2,"Bed & Breakfast","bed_and_breakfast",11],[3,"Half Board","half_board",26]]);
addBoards(12, [[2,"Bed & Breakfast","bed_and_breakfast",11],[3,"Half Board","half_board",26]]);
addBoards(13, [[2,"Bed & Breakfast","bed_and_breakfast",13],[3,"Half Board","half_board",30]]);

// Bellver Park (4 stars) - hprt 14-16
addBoards(14, [[2,"Bed & Breakfast","bed_and_breakfast",20],[3,"Half Board","half_board",42],[4,"Full Board","full_board",62]]);
addBoards(15, [[2,"Bed & Breakfast","bed_and_breakfast",20],[3,"Half Board","half_board",42],[4,"Full Board","full_board",62]]);
addBoards(16, [[2,"Bed & Breakfast","bed_and_breakfast",25],[3,"Half Board","half_board",48],[4,"Full Board","full_board",68]]);

// Marina Palma (5 stars) - hprt 17-19
addBoards(17, [[2,"Bed & Breakfast","bed_and_breakfast",28],[3,"Half Board","half_board",50],[4,"Full Board","full_board",78],[5,"All Inclusive","all_inclusive",110]]);
addBoards(18, [[2,"Bed & Breakfast","bed_and_breakfast",32],[3,"Half Board","half_board",55],[4,"Full Board","full_board",85],[5,"All Inclusive","all_inclusive",125]]);
addBoards(19, [[2,"Bed & Breakfast","bed_and_breakfast",35],[3,"Half Board","half_board",58],[4,"Full Board","full_board",88],[5,"All Inclusive","all_inclusive",128]]);

// Catedral Palma (4 stars) - hprt 20-22
addBoards(20, [[2,"Bed & Breakfast","bed_and_breakfast",19],[3,"Half Board","half_board",38],[4,"Full Board","full_board",58]]);
addBoards(21, [[2,"Bed & Breakfast","bed_and_breakfast",19],[3,"Half Board","half_board",38],[4,"Full Board","full_board",58]]);
addBoards(22, [[2,"Bed & Breakfast","bed_and_breakfast",23],[3,"Half Board","half_board",44],[4,"Full Board","full_board",64]]);

// Portixol (3 stars) - hprt 23-25
addBoards(23, [[2,"Bed & Breakfast","bed_and_breakfast",12],[3,"Half Board","half_board",28]]);
addBoards(24, [[2,"Bed & Breakfast","bed_and_breakfast",12],[3,"Half Board","half_board",28]]);
addBoards(25, [[2,"Bed & Breakfast","bed_and_breakfast",14],[3,"Half Board","half_board",32]]);

// Santa Catalina (4 stars) - hprt 26-28
addBoards(26, [[2,"Bed & Breakfast","bed_and_breakfast",20],[3,"Half Board","half_board",40],[4,"Full Board","full_board",60]]);
addBoards(27, [[2,"Bed & Breakfast","bed_and_breakfast",20],[3,"Half Board","half_board",40],[4,"Full Board","full_board",60]]);
addBoards(28, [[2,"Bed & Breakfast","bed_and_breakfast",24],[3,"Half Board","half_board",46],[4,"Full Board","full_board",66]]);

// Son Vida Palace (5 stars) - hprt 29-31
addBoards(29, [[2,"Bed & Breakfast","bed_and_breakfast",32],[3,"Half Board","half_board",58],[4,"Full Board","full_board",88],[5,"All Inclusive","all_inclusive",125]]);
addBoards(30, [[2,"Bed & Breakfast","bed_and_breakfast",35],[3,"Half Board","half_board",60],[4,"Full Board","full_board",90],[5,"All Inclusive","all_inclusive",130]]);
addBoards(31, [[2,"Bed & Breakfast","bed_and_breakfast",35],[3,"Half Board","half_board",60],[4,"Full Board","full_board",90],[5,"All Inclusive","all_inclusive",130]]);

// Bahía Alcúdia (4 stars) - hprt 32-34
addBoards(32, [[2,"Bed & Breakfast","bed_and_breakfast",17],[3,"Half Board","half_board",36],[4,"Full Board","full_board",56]]);
addBoards(33, [[2,"Bed & Breakfast","bed_and_breakfast",17],[3,"Half Board","half_board",36],[4,"Full Board","full_board",56]]);
addBoards(34, [[2,"Bed & Breakfast","bed_and_breakfast",21],[3,"Half Board","half_board",42],[4,"Full Board","full_board",62]]);

// Serra de Tramuntana (4 stars) - hprt 35-37
addBoards(35, [[2,"Bed & Breakfast","bed_and_breakfast",22],[3,"Half Board","half_board",44],[4,"Full Board","full_board",64]]);
addBoards(36, [[2,"Bed & Breakfast","bed_and_breakfast",24],[3,"Half Board","half_board",48],[4,"Full Board","full_board",68]]);
addBoards(37, [[2,"Bed & Breakfast","bed_and_breakfast",25],[3,"Half Board","half_board",50],[4,"Full Board","full_board",70]]);

// Port Mahón (4 stars) - hprt 38-40
addBoards(38, [[2,"Bed & Breakfast","bed_and_breakfast",18],[3,"Half Board","half_board",38],[4,"Full Board","full_board",58]]);
addBoards(39, [[2,"Bed & Breakfast","bed_and_breakfast",18],[3,"Half Board","half_board",38],[4,"Full Board","full_board",58]]);
addBoards(40, [[2,"Bed & Breakfast","bed_and_breakfast",24],[3,"Half Board","half_board",46],[4,"Full Board","full_board",66]]);

// Ciutadella Mar (3 stars) - hprt 41-43
addBoards(41, [[2,"Bed & Breakfast","bed_and_breakfast",10],[3,"Half Board","half_board",25]]);
addBoards(42, [[2,"Bed & Breakfast","bed_and_breakfast",10],[3,"Half Board","half_board",25]]);
addBoards(43, [[2,"Bed & Breakfast","bed_and_breakfast",12],[3,"Half Board","half_board",28]]);

// Dalt Vila (5 stars) - hprt 44-46
addBoards(44, [[2,"Bed & Breakfast","bed_and_breakfast",30],[3,"Half Board","half_board",52],[4,"Full Board","full_board",80],[5,"All Inclusive","all_inclusive",115]]);
addBoards(45, [[2,"Bed & Breakfast","bed_and_breakfast",34],[3,"Half Board","half_board",58],[4,"Full Board","full_board",86],[5,"All Inclusive","all_inclusive",125]]);
addBoards(46, [[2,"Bed & Breakfast","bed_and_breakfast",35],[3,"Half Board","half_board",60],[4,"Full Board","full_board",90],[5,"All Inclusive","all_inclusive",130]]);

// Santa Eulària Beach (4 stars) - hprt 47-49
addBoards(47, [[2,"Bed & Breakfast","bed_and_breakfast",16],[3,"Half Board","half_board",35],[4,"Full Board","full_board",55]]);
addBoards(48, [[2,"Bed & Breakfast","bed_and_breakfast",16],[3,"Half Board","half_board",35],[4,"Full Board","full_board",55]]);
addBoards(49, [[2,"Bed & Breakfast","bed_and_breakfast",20],[3,"Half Board","half_board",42],[4,"Full Board","full_board",62]]);

// Altairis Barcelona (5 stars) - hprt 50-53
addBoards(50, [[2,"Bed & Breakfast","bed_and_breakfast",28],[3,"Half Board","half_board",52],[4,"Full Board","full_board",82],[5,"All Inclusive","all_inclusive",118]]);
addBoards(51, [[2,"Bed & Breakfast","bed_and_breakfast",28],[3,"Half Board","half_board",52],[4,"Full Board","full_board",82],[5,"All Inclusive","all_inclusive",118]]);
addBoards(52, [[2,"Bed & Breakfast","bed_and_breakfast",33],[3,"Half Board","half_board",58],[4,"Full Board","full_board",88],[5,"All Inclusive","all_inclusive",128]]);
addBoards(53, [[2,"Bed & Breakfast","bed_and_breakfast",35],[3,"Half Board","half_board",60],[4,"Full Board","full_board",90],[5,"All Inclusive","all_inclusive",130]]);

// Gótico (3 stars) - hprt 54-56
addBoards(54, [[2,"Bed & Breakfast","bed_and_breakfast",12],[3,"Half Board","half_board",28]]);
addBoards(55, [[2,"Bed & Breakfast","bed_and_breakfast",12],[3,"Half Board","half_board",28]]);
addBoards(56, [[2,"Bed & Breakfast","bed_and_breakfast",14],[3,"Half Board","half_board",32]]);

// Barceloneta Mar (4 stars) - hprt 57-59
addBoards(57, [[2,"Bed & Breakfast","bed_and_breakfast",19],[3,"Half Board","half_board",40],[4,"Full Board","full_board",60]]);
addBoards(58, [[2,"Bed & Breakfast","bed_and_breakfast",19],[3,"Half Board","half_board",40],[4,"Full Board","full_board",60]]);
addBoards(59, [[2,"Bed & Breakfast","bed_and_breakfast",23],[3,"Half Board","half_board",46],[4,"Full Board","full_board",66]]);

// Promenade Nice (5 stars) - hprt 60-63
addBoards(60, [[2,"Bed & Breakfast","bed_and_breakfast",30],[3,"Half Board","half_board",55],[4,"Full Board","full_board",85],[5,"All Inclusive","all_inclusive",122]]);
addBoards(61, [[2,"Bed & Breakfast","bed_and_breakfast",30],[3,"Half Board","half_board",55],[4,"Full Board","full_board",85],[5,"All Inclusive","all_inclusive",122]]);
addBoards(62, [[2,"Bed & Breakfast","bed_and_breakfast",34],[3,"Half Board","half_board",58],[4,"Full Board","full_board",88],[5,"All Inclusive","all_inclusive",128]]);
addBoards(63, [[2,"Bed & Breakfast","bed_and_breakfast",35],[3,"Half Board","half_board",60],[4,"Full Board","full_board",90],[5,"All Inclusive","all_inclusive",130]]);

// Vieux Nice (3 stars) - hprt 64-66
addBoards(64, [[2,"Bed & Breakfast","bed_and_breakfast",11],[3,"Half Board","half_board",26]]);
addBoards(65, [[2,"Bed & Breakfast","bed_and_breakfast",11],[3,"Half Board","half_board",26]]);
addBoards(66, [[2,"Bed & Breakfast","bed_and_breakfast",13],[3,"Half Board","half_board",30]]);

// Colline de Cimiez (4 stars) - hprt 67-69
addBoards(67, [[2,"Bed & Breakfast","bed_and_breakfast",20],[3,"Half Board","half_board",42],[4,"Full Board","full_board",62]]);
addBoards(68, [[2,"Bed & Breakfast","bed_and_breakfast",20],[3,"Half Board","half_board",42],[4,"Full Board","full_board",62]]);
addBoards(69, [[2,"Bed & Breakfast","bed_and_breakfast",24],[3,"Half Board","half_board",48],[4,"Full Board","full_board",68]]);

function img(hotelName: string, roomType: string, n: number, roomId: number): RoomImage {
  return { id: roomId * 10 + n, room_config_id: roomId, url: `https://picsum.photos/seed/room-${roomId}-${n}/800/600`, alt_text: `${roomType} at ${hotelName}`, sort_order: n };
}

function makeRoom(id: number, hotelId: number, roomTypeId: number, capacity: number, quantity: number, price: number, hotelName: string): RoomConfig {
  const typeName = ROOM_TYPE_NAMES[roomTypeId];
  const numImages = (roomTypeId === 4 || roomTypeId === 6) ? 3 : 2;
  const stars = [5, 3, 4, 3, 4, 5, 4, 3, 4, 5, 4, 4, 4, 3, 5, 4, 5, 3, 4, 5, 3, 4][hotelId - 1] as 3 | 4 | 5;
  return {
    id,
    hotel_id: hotelId,
    room_type_id: roomTypeId,
    room_type_name: typeName,
    capacity,
    quantity,
    base_price_per_night: applyMargin(price, hotelId),
    currency_code: "EUR",
    images: Array.from({ length: numImages }, (_, i) => img(hotelName, typeName, i + 1, id)),
    amenities: [],
    board_options: boards(stars, roomTypeId, id, hotelId),
  };
}

export const roomConfigs: RoomConfig[] = [
  // Hotel Altairis Palma (id=1, 5 stars)
  makeRoom(1, 1, 1, 1, 20, 120, "Hotel Altairis Palma"),
  makeRoom(2, 1, 2, 2, 30, 180, "Hotel Altairis Palma"),
  makeRoom(3, 1, 4, 3, 10, 350, "Hotel Altairis Palma"),
  makeRoom(4, 1, 6, 2, 5, 450, "Hotel Altairis Palma"),
  // Hotel Sol de Palma (id=2, 3 stars)
  makeRoom(5, 2, 1, 1, 15, 55, "Hotel Sol de Palma"),
  makeRoom(6, 2, 2, 2, 20, 80, "Hotel Sol de Palma"),
  makeRoom(7, 2, 3, 2, 10, 75, "Hotel Sol de Palma"),
  // Hotel Playa de Palma (id=3, 4 stars)
  makeRoom(8, 3, 1, 1, 15, 85, "Hotel Playa de Palma"),
  makeRoom(9, 3, 2, 2, 25, 130, "Hotel Playa de Palma"),
  makeRoom(10, 3, 5, 2, 6, 200, "Hotel Playa de Palma"),
  // Hotel Casco Antiguo (id=4, 3 stars)
  makeRoom(11, 4, 1, 1, 10, 50, "Hotel Casco Antiguo"),
  makeRoom(12, 4, 2, 2, 15, 75, "Hotel Casco Antiguo"),
  makeRoom(13, 4, 3, 2, 8, 70, "Hotel Casco Antiguo"),
  // Hotel Bellver Park (id=5, 4 stars)
  makeRoom(14, 5, 1, 1, 12, 90, "Hotel Bellver Park"),
  makeRoom(15, 5, 2, 2, 20, 140, "Hotel Bellver Park"),
  makeRoom(16, 5, 4, 3, 4, 280, "Hotel Bellver Park"),
  // Hotel Marina Palma (id=6, 5 stars)
  makeRoom(17, 6, 2, 2, 25, 190, "Hotel Marina Palma"),
  makeRoom(18, 6, 4, 3, 8, 380, "Hotel Marina Palma"),
  makeRoom(19, 6, 6, 2, 4, 480, "Hotel Marina Palma"),
  // Hotel Catedral Palma (id=7, 4 stars)
  makeRoom(20, 7, 1, 1, 10, 95, "Hotel Catedral Palma"),
  makeRoom(21, 7, 2, 2, 18, 145, "Hotel Catedral Palma"),
  makeRoom(22, 7, 5, 2, 5, 220, "Hotel Catedral Palma"),
  // Hotel Portixol (id=8, 3 stars)
  makeRoom(23, 8, 1, 1, 8, 45, "Hotel Portixol"),
  makeRoom(24, 8, 2, 2, 12, 70, "Hotel Portixol"),
  makeRoom(25, 8, 3, 2, 6, 65, "Hotel Portixol"),
  // Hotel Santa Catalina (id=9, 4 stars)
  makeRoom(26, 9, 1, 1, 10, 88, "Hotel Santa Catalina"),
  makeRoom(27, 9, 2, 2, 16, 135, "Hotel Santa Catalina"),
  makeRoom(28, 9, 5, 2, 4, 210, "Hotel Santa Catalina"),
  // Hotel Son Vida Palace (id=10, 5 stars)
  makeRoom(29, 10, 2, 2, 20, 220, "Hotel Son Vida Palace"),
  makeRoom(30, 10, 4, 3, 10, 420, "Hotel Son Vida Palace"),
  makeRoom(31, 10, 6, 2, 5, 550, "Hotel Son Vida Palace"),
  // Hotel Bahía Alcúdia (id=11, 4 stars)
  makeRoom(32, 11, 1, 1, 10, 90, "Hotel Bahía Alcúdia"),
  makeRoom(33, 11, 2, 2, 25, 140, "Hotel Bahía Alcúdia"),
  makeRoom(34, 11, 5, 2, 8, 220, "Hotel Bahía Alcúdia"),
  // Hotel Serra de Tramuntana (id=12, 4 stars)
  makeRoom(35, 12, 2, 2, 15, 130, "Hotel Serra de Tramuntana"),
  makeRoom(36, 12, 5, 2, 5, 200, "Hotel Serra de Tramuntana"),
  makeRoom(37, 12, 4, 3, 3, 300, "Hotel Serra de Tramuntana"),
  // Hotel Port Mahón (id=13, 4 stars)
  makeRoom(38, 13, 1, 1, 12, 85, "Hotel Port Mahón"),
  makeRoom(39, 13, 2, 2, 20, 135, "Hotel Port Mahón"),
  makeRoom(40, 13, 4, 3, 5, 280, "Hotel Port Mahón"),
  // Hotel Ciutadella Mar (id=14, 3 stars)
  makeRoom(41, 14, 1, 1, 10, 50, "Hotel Ciutadella Mar"),
  makeRoom(42, 14, 2, 2, 15, 75, "Hotel Ciutadella Mar"),
  makeRoom(43, 14, 3, 2, 8, 70, "Hotel Ciutadella Mar"),
  // Hotel Dalt Vila (id=15, 5 stars)
  makeRoom(44, 15, 2, 2, 20, 200, "Hotel Dalt Vila"),
  makeRoom(45, 15, 4, 3, 8, 400, "Hotel Dalt Vila"),
  makeRoom(46, 15, 6, 2, 4, 500, "Hotel Dalt Vila"),
  // Hotel Santa Eulària Beach (id=16, 4 stars)
  makeRoom(47, 16, 1, 1, 10, 95, "Hotel Santa Eulària Beach"),
  makeRoom(48, 16, 2, 2, 18, 150, "Hotel Santa Eulària Beach"),
  makeRoom(49, 16, 5, 2, 6, 230, "Hotel Santa Eulària Beach"),
  // Hotel Altairis Barcelona (id=17, 5 stars)
  makeRoom(50, 17, 1, 1, 15, 130, "Hotel Altairis Barcelona"),
  makeRoom(51, 17, 2, 2, 25, 200, "Hotel Altairis Barcelona"),
  makeRoom(52, 17, 4, 3, 10, 380, "Hotel Altairis Barcelona"),
  makeRoom(53, 17, 6, 2, 5, 500, "Hotel Altairis Barcelona"),
  // Hotel Gótico (id=18, 3 stars)
  makeRoom(54, 18, 1, 1, 10, 60, "Hotel Gótico"),
  makeRoom(55, 18, 2, 2, 15, 90, "Hotel Gótico"),
  makeRoom(56, 18, 3, 2, 8, 85, "Hotel Gótico"),
  // Hotel Barceloneta Mar (id=19, 4 stars)
  makeRoom(57, 19, 1, 1, 12, 95, "Hotel Barceloneta Mar"),
  makeRoom(58, 19, 2, 2, 20, 150, "Hotel Barceloneta Mar"),
  makeRoom(59, 19, 5, 2, 6, 240, "Hotel Barceloneta Mar"),
  // Hotel Promenade Nice (id=20, 5 stars)
  makeRoom(60, 20, 1, 1, 15, 150, "Hotel Promenade Nice"),
  makeRoom(61, 20, 2, 2, 25, 230, "Hotel Promenade Nice"),
  makeRoom(62, 20, 4, 3, 8, 420, "Hotel Promenade Nice"),
  makeRoom(63, 20, 6, 2, 4, 550, "Hotel Promenade Nice"),
  // Hotel Vieux Nice (id=21, 3 stars)
  makeRoom(64, 21, 1, 1, 8, 65, "Hotel Vieux Nice"),
  makeRoom(65, 21, 2, 2, 12, 95, "Hotel Vieux Nice"),
  makeRoom(66, 21, 3, 2, 6, 90, "Hotel Vieux Nice"),
  // Hotel Colline de Cimiez (id=22, 4 stars)
  makeRoom(67, 22, 1, 1, 10, 100, "Hotel Colline de Cimiez"),
  makeRoom(68, 22, 2, 2, 18, 160, "Hotel Colline de Cimiez"),
  makeRoom(69, 22, 5, 2, 5, 250, "Hotel Colline de Cimiez"),
];

export function getRoomsForHotel(hotelId: number): RoomConfig[] {
  return roomConfigs.filter((r) => r.hotel_id === hotelId);
}
