-- hotel_provider IDs match hotel IDs 1:1 in our seed data (all internal for now)
-- Room type IDs: 1=single, 2=double, 3=twin, 4=suite, 5=junior_suite, 6=deluxe

-- =====================
-- Hotel Altairis Palma (hp_id=1, 5 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(1, 1, 1, 20, 120.00, 1, 1),
(1, 2, 2, 30, 180.00, 1, 1),
(1, 4, 3, 10, 350.00, 1, 1),
(1, 6, 2, 5, 450.00, 1, 1);

-- =====================
-- Hotel Sol de Palma (hp_id=2, 3 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(2, 1, 1, 15, 55.00, 1, 1),
(2, 2, 2, 20, 80.00, 1, 1),
(2, 3, 2, 10, 75.00, 1, 1);

-- =====================
-- Hotel Playa de Palma (hp_id=3, 4 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(3, 1, 1, 15, 85.00, 1, 1),
(3, 2, 2, 25, 130.00, 1, 1),
(3, 5, 2, 6, 200.00, 1, 1);

-- =====================
-- Hotel Casco Antiguo (hp_id=4, 3 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(4, 1, 1, 10, 50.00, 1, 1),
(4, 2, 2, 15, 75.00, 1, 1),
(4, 3, 2, 8, 70.00, 1, 1);

-- =====================
-- Hotel Bellver Park (hp_id=5, 4 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(5, 1, 1, 12, 90.00, 1, 1),
(5, 2, 2, 20, 140.00, 1, 1),
(5, 4, 3, 4, 280.00, 1, 1);

-- =====================
-- Hotel Marina Palma (hp_id=6, 5 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(6, 2, 2, 25, 190.00, 1, 1),
(6, 4, 3, 8, 380.00, 1, 1),
(6, 6, 2, 4, 480.00, 1, 1);

-- =====================
-- Hotel Catedral Palma (hp_id=7, 4 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(7, 1, 1, 10, 95.00, 1, 1),
(7, 2, 2, 18, 145.00, 1, 1),
(7, 5, 2, 5, 220.00, 1, 1);

-- =====================
-- Hotel Portixol (hp_id=8, 3 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(8, 1, 1, 8, 45.00, 1, 1),
(8, 2, 2, 12, 70.00, 1, 1),
(8, 3, 2, 6, 65.00, 1, 1);

-- =====================
-- Hotel Santa Catalina (hp_id=9, 4 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(9, 1, 1, 10, 88.00, 1, 1),
(9, 2, 2, 16, 135.00, 1, 1),
(9, 5, 2, 4, 210.00, 1, 1);

-- =====================
-- Hotel Son Vida Palace (hp_id=10, 5 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(10, 2, 2, 20, 220.00, 1, 1),
(10, 4, 3, 10, 420.00, 1, 1),
(10, 6, 2, 5, 550.00, 1, 1);

-- =====================
-- Hotel Bahía Alcúdia (hp_id=11, 4 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(11, 1, 1, 10, 90.00, 1, 1),
(11, 2, 2, 25, 140.00, 1, 1),
(11, 5, 2, 8, 220.00, 1, 1);

-- =====================
-- Hotel Serra de Tramuntana (hp_id=12, 4 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(12, 2, 2, 15, 130.00, 1, 1),
(12, 5, 2, 5, 200.00, 1, 1),
(12, 4, 3, 3, 300.00, 1, 1);

-- =====================
-- Hotel Port Mahón (hp_id=13, 4 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(13, 1, 1, 12, 85.00, 1, 1),
(13, 2, 2, 20, 135.00, 1, 1),
(13, 4, 3, 5, 280.00, 1, 1);

-- =====================
-- Hotel Ciutadella Mar (hp_id=14, 3 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(14, 1, 1, 10, 50.00, 1, 1),
(14, 2, 2, 15, 75.00, 1, 1),
(14, 3, 2, 8, 70.00, 1, 1);

-- =====================
-- Hotel Dalt Vila (hp_id=15, 5 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(15, 2, 2, 20, 200.00, 1, 1),
(15, 4, 3, 8, 400.00, 1, 1),
(15, 6, 2, 4, 500.00, 1, 1);

-- =====================
-- Hotel Santa Eulària Beach (hp_id=16, 4 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(16, 1, 1, 10, 95.00, 1, 1),
(16, 2, 2, 18, 150.00, 1, 1),
(16, 5, 2, 6, 230.00, 1, 1);

-- =====================
-- Hotel Altairis Barcelona (hp_id=17, 5 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(17, 1, 1, 15, 130.00, 1, 1),
(17, 2, 2, 25, 200.00, 1, 1),
(17, 4, 3, 10, 380.00, 1, 1),
(17, 6, 2, 5, 500.00, 1, 1);

-- =====================
-- Hotel Gótico (hp_id=18, 3 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(18, 1, 1, 10, 60.00, 1, 1),
(18, 2, 2, 15, 90.00, 1, 1),
(18, 3, 2, 8, 85.00, 1, 1);

-- =====================
-- Hotel Barceloneta Mar (hp_id=19, 4 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(19, 1, 1, 12, 95.00, 1, 1),
(19, 2, 2, 20, 150.00, 1, 1),
(19, 5, 2, 6, 240.00, 1, 1);

-- =====================
-- Hotel Promenade Nice (hp_id=20, 5 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(20, 1, 1, 15, 150.00, 1, 1),
(20, 2, 2, 25, 230.00, 1, 1),
(20, 4, 3, 8, 420.00, 1, 1),
(20, 6, 2, 4, 550.00, 1, 1);

-- =====================
-- Hotel Vieux Nice (hp_id=21, 3 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(21, 1, 1, 8, 65.00, 1, 1),
(21, 2, 2, 12, 95.00, 1, 1),
(21, 3, 2, 6, 90.00, 1, 1);

-- =====================
-- Hotel Colline de Cimiez (hp_id=22, 4 stars)
-- =====================
INSERT INTO hotel_provider_room_type (hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id) VALUES
(22, 1, 1, 10, 100.00, 1, 1),
(22, 2, 2, 18, 160.00, 1, 1),
(22, 5, 2, 5, 250.00, 1, 1);
