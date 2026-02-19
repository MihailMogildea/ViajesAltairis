-- Seed: hotel_provider_room_type_board
-- Links room types to board (meal) options with addon pricing.
-- Board types: 1=room_only, 2=bed_and_breakfast, 3=half_board, 4=full_board, 5=all_inclusive

-- Altairis Palma (hp_id=1, 5 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(1, 1, 0.00),
(1, 2, 30.00),
(1, 3, 55.00),
(1, 4, 85.00),
(1, 5, 120.00),
(2, 1, 0.00),
(2, 2, 30.00),
(2, 3, 55.00),
(2, 4, 85.00),
(2, 5, 120.00),
(3, 1, 0.00),
(3, 2, 35.00),
(3, 3, 60.00),
(3, 4, 90.00),
(3, 5, 130.00),
(4, 1, 0.00),
(4, 2, 35.00),
(4, 3, 60.00),
(4, 4, 90.00),
(4, 5, 130.00);

-- Sol de Palma (hp_id=2, 3 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(5, 1, 0.00),
(5, 2, 10.00),
(5, 3, 25.00),
(6, 1, 0.00),
(6, 2, 10.00),
(6, 3, 25.00),
(7, 1, 0.00),
(7, 2, 12.00),
(7, 3, 28.00);

-- Playa de Palma (hp_id=3, 4 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(8, 1, 0.00),
(8, 2, 18.00),
(8, 3, 40.00),
(8, 4, 60.00),
(9, 1, 0.00),
(9, 2, 18.00),
(9, 3, 40.00),
(9, 4, 60.00),
(10, 1, 0.00),
(10, 2, 22.00),
(10, 3, 45.00),
(10, 4, 65.00);

-- Casco Antiguo (hp_id=4, 3 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(11, 1, 0.00),
(11, 2, 11.00),
(11, 3, 26.00),
(12, 1, 0.00),
(12, 2, 11.00),
(12, 3, 26.00),
(13, 1, 0.00),
(13, 2, 13.00),
(13, 3, 30.00);

-- Bellver Park (hp_id=5, 4 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(14, 1, 0.00),
(14, 2, 20.00),
(14, 3, 42.00),
(14, 4, 62.00),
(15, 1, 0.00),
(15, 2, 20.00),
(15, 3, 42.00),
(15, 4, 62.00),
(16, 1, 0.00),
(16, 2, 25.00),
(16, 3, 48.00),
(16, 4, 68.00);

-- Marina Palma (hp_id=6, 5 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(17, 1, 0.00),
(17, 2, 28.00),
(17, 3, 50.00),
(17, 4, 78.00),
(17, 5, 110.00),
(18, 1, 0.00),
(18, 2, 32.00),
(18, 3, 55.00),
(18, 4, 85.00),
(18, 5, 125.00),
(19, 1, 0.00),
(19, 2, 35.00),
(19, 3, 58.00),
(19, 4, 88.00),
(19, 5, 128.00);

-- Catedral Palma (hp_id=7, 4 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(20, 1, 0.00),
(20, 2, 19.00),
(20, 3, 38.00),
(20, 4, 58.00),
(21, 1, 0.00),
(21, 2, 19.00),
(21, 3, 38.00),
(21, 4, 58.00),
(22, 1, 0.00),
(22, 2, 23.00),
(22, 3, 44.00),
(22, 4, 64.00);

-- Portixol (hp_id=8, 3 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(23, 1, 0.00),
(23, 2, 12.00),
(23, 3, 28.00),
(24, 1, 0.00),
(24, 2, 12.00),
(24, 3, 28.00),
(25, 1, 0.00),
(25, 2, 14.00),
(25, 3, 32.00);

-- Santa Catalina (hp_id=9, 4 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(26, 1, 0.00),
(26, 2, 20.00),
(26, 3, 40.00),
(26, 4, 60.00),
(27, 1, 0.00),
(27, 2, 20.00),
(27, 3, 40.00),
(27, 4, 60.00),
(28, 1, 0.00),
(28, 2, 24.00),
(28, 3, 46.00),
(28, 4, 66.00);

-- Son Vida Palace (hp_id=10, 5 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(29, 1, 0.00),
(29, 2, 32.00),
(29, 3, 58.00),
(29, 4, 88.00),
(29, 5, 125.00),
(30, 1, 0.00),
(30, 2, 35.00),
(30, 3, 60.00),
(30, 4, 90.00),
(30, 5, 130.00),
(31, 1, 0.00),
(31, 2, 35.00),
(31, 3, 60.00),
(31, 4, 90.00),
(31, 5, 130.00);

-- Bahía Alcúdia (hp_id=11, 4 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(32, 1, 0.00),
(32, 2, 17.00),
(32, 3, 36.00),
(32, 4, 56.00),
(33, 1, 0.00),
(33, 2, 17.00),
(33, 3, 36.00),
(33, 4, 56.00),
(34, 1, 0.00),
(34, 2, 21.00),
(34, 3, 42.00),
(34, 4, 62.00);

-- Serra Tramuntana (hp_id=12, 4 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(35, 1, 0.00),
(35, 2, 22.00),
(35, 3, 44.00),
(35, 4, 64.00),
(36, 1, 0.00),
(36, 2, 24.00),
(36, 3, 48.00),
(36, 4, 68.00),
(37, 1, 0.00),
(37, 2, 25.00),
(37, 3, 50.00),
(37, 4, 70.00);

-- Port Mahón (hp_id=13, 4 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(38, 1, 0.00),
(38, 2, 18.00),
(38, 3, 38.00),
(38, 4, 58.00),
(39, 1, 0.00),
(39, 2, 18.00),
(39, 3, 38.00),
(39, 4, 58.00),
(40, 1, 0.00),
(40, 2, 24.00),
(40, 3, 46.00),
(40, 4, 66.00);

-- Ciutadella Mar (hp_id=14, 3 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(41, 1, 0.00),
(41, 2, 10.00),
(41, 3, 25.00),
(42, 1, 0.00),
(42, 2, 10.00),
(42, 3, 25.00),
(43, 1, 0.00),
(43, 2, 12.00),
(43, 3, 28.00);

-- Dalt Vila (hp_id=15, 5 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(44, 1, 0.00),
(44, 2, 30.00),
(44, 3, 52.00),
(44, 4, 80.00),
(44, 5, 115.00),
(45, 1, 0.00),
(45, 2, 34.00),
(45, 3, 58.00),
(45, 4, 86.00),
(45, 5, 125.00),
(46, 1, 0.00),
(46, 2, 35.00),
(46, 3, 60.00),
(46, 4, 90.00),
(46, 5, 130.00);

-- Santa Eulària (hp_id=16, 4 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(47, 1, 0.00),
(47, 2, 16.00),
(47, 3, 35.00),
(47, 4, 55.00),
(48, 1, 0.00),
(48, 2, 16.00),
(48, 3, 35.00),
(48, 4, 55.00),
(49, 1, 0.00),
(49, 2, 20.00),
(49, 3, 42.00),
(49, 4, 62.00);

-- Altairis Barcelona (hp_id=17, 5 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(50, 1, 0.00),
(50, 2, 28.00),
(50, 3, 52.00),
(50, 4, 82.00),
(50, 5, 118.00),
(51, 1, 0.00),
(51, 2, 28.00),
(51, 3, 52.00),
(51, 4, 82.00),
(51, 5, 118.00),
(52, 1, 0.00),
(52, 2, 33.00),
(52, 3, 58.00),
(52, 4, 88.00),
(52, 5, 128.00),
(53, 1, 0.00),
(53, 2, 35.00),
(53, 3, 60.00),
(53, 4, 90.00),
(53, 5, 130.00);

-- Gótico (hp_id=18, 3 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(54, 1, 0.00),
(54, 2, 12.00),
(54, 3, 28.00),
(55, 1, 0.00),
(55, 2, 12.00),
(55, 3, 28.00),
(56, 1, 0.00),
(56, 2, 14.00),
(56, 3, 32.00);

-- Barceloneta Mar (hp_id=19, 4 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(57, 1, 0.00),
(57, 2, 19.00),
(57, 3, 40.00),
(57, 4, 60.00),
(58, 1, 0.00),
(58, 2, 19.00),
(58, 3, 40.00),
(58, 4, 60.00),
(59, 1, 0.00),
(59, 2, 23.00),
(59, 3, 46.00),
(59, 4, 66.00);

-- Promenade Nice (hp_id=20, 5 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(60, 1, 0.00),
(60, 2, 30.00),
(60, 3, 55.00),
(60, 4, 85.00),
(60, 5, 122.00),
(61, 1, 0.00),
(61, 2, 30.00),
(61, 3, 55.00),
(61, 4, 85.00),
(61, 5, 122.00),
(62, 1, 0.00),
(62, 2, 34.00),
(62, 3, 58.00),
(62, 4, 88.00),
(62, 5, 128.00),
(63, 1, 0.00),
(63, 2, 35.00),
(63, 3, 60.00),
(63, 4, 90.00),
(63, 5, 130.00);

-- Vieux Nice (hp_id=21, 3 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(64, 1, 0.00),
(64, 2, 11.00),
(64, 3, 26.00),
(65, 1, 0.00),
(65, 2, 11.00),
(65, 3, 26.00),
(66, 1, 0.00),
(66, 2, 13.00),
(66, 3, 30.00);

-- Colline Cimiez (hp_id=22, 4 stars)
INSERT INTO hotel_provider_room_type_board (hotel_provider_room_type_id, board_type_id, price_per_night) VALUES
(67, 1, 0.00),
(67, 2, 20.00),
(67, 3, 42.00),
(67, 4, 62.00),
(68, 1, 0.00),
(68, 2, 20.00),
(68, 3, 42.00),
(68, 4, 62.00),
(69, 1, 0.00),
(69, 2, 24.00),
(69, 3, 48.00),
(69, 4, 68.00);
