-- Amenity IDs: 1=pool, 2=gym, 3=spa, 4=restaurant, 5=parking, 6=beach_access, 7=bar, 8=garden

-- 5-star hotels get everything
-- Hotel Altairis Palma (id=1)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), (1, 7), (1, 8);

-- Hotel Marina Palma (id=6)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(6, 1), (6, 2), (6, 3), (6, 4), (6, 5), (6, 6), (6, 7), (6, 8);

-- Hotel Son Vida Palace (id=10)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(10, 1), (10, 2), (10, 3), (10, 4), (10, 5), (10, 7), (10, 8);

-- Hotel Dalt Vila (id=15)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(15, 1), (15, 2), (15, 3), (15, 4), (15, 5), (15, 7), (15, 8);

-- Hotel Altairis Barcelona (id=17)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(17, 1), (17, 2), (17, 3), (17, 4), (17, 5), (17, 7);

-- Hotel Promenade Nice (id=20)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(20, 1), (20, 2), (20, 3), (20, 4), (20, 5), (20, 6), (20, 7), (20, 8);

-- 4-star hotels get most
-- Hotel Playa de Palma (id=3)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(3, 1), (3, 4), (3, 5), (3, 6), (3, 7);

-- Hotel Bellver Park (id=5)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(5, 1), (5, 2), (5, 4), (5, 5), (5, 8);

-- Hotel Catedral Palma (id=7)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(7, 4), (7, 5), (7, 7);

-- Hotel Santa Catalina (id=9)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(9, 1), (9, 4), (9, 5), (9, 7), (9, 8);

-- Hotel Bahía Alcúdia (id=11)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(11, 1), (11, 4), (11, 5), (11, 6), (11, 7);

-- Hotel Serra de Tramuntana (id=12)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(12, 4), (12, 5), (12, 8);

-- Hotel Port Mahón (id=13)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(13, 1), (13, 4), (13, 5), (13, 7);

-- Hotel Santa Eulària Beach (id=16)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(16, 1), (16, 4), (16, 5), (16, 6), (16, 7);

-- Hotel Barceloneta Mar (id=19)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(19, 1), (19, 4), (19, 5), (19, 6), (19, 7);

-- Hotel Colline de Cimiez (id=22)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(22, 2), (22, 4), (22, 5), (22, 8);

-- 3-star hotels get basics
-- Hotel Sol de Palma (id=2)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(2, 4), (2, 7);

-- Hotel Casco Antiguo (id=4)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(4, 4), (4, 7);

-- Hotel Portixol (id=8)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(8, 4), (8, 6);

-- Hotel Ciutadella Mar (id=14)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(14, 4), (14, 7);

-- Hotel Gótico (id=18)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(18, 4), (18, 7);

-- Hotel Vieux Nice (id=21)
INSERT INTO hotel_amenity (hotel_id, amenity_id) VALUES
(21, 4), (21, 7);
