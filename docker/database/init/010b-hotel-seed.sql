-- =====================
-- Mallorca hotels (cities 1-10)
-- =====================

-- Palma (city_id = 1)
INSERT INTO hotel (city_id, name, stars, address, email, phone, check_in_time, check_out_time, latitude, longitude) VALUES
(1, 'Hotel Altairis Palma', 5, 'Paseo Marítimo 12', 'palma@altairis.com', '+34 971 000 001', '15:00:00', '11:00:00', 39.5696000, 2.6502000),
(1, 'Hotel Sol de Palma', 3, 'Carrer dels Apuntadors 8', 'solpalma@altairis.com', '+34 971 000 002', '14:00:00', '12:00:00', 39.5678000, 2.6489000),
(1, 'Hotel Playa de Palma', 4, 'Carrer de la Sirena 22', 'playa@altairis.com', '+34 971 000 010', '15:00:00', '11:00:00', 39.5245000, 2.7420000),
(1, 'Hotel Casco Antiguo', 3, 'Carrer de Sant Jaume 9', 'casco@altairis.com', '+34 971 000 011', '14:00:00', '12:00:00', 39.5710000, 2.6495000),
(1, 'Hotel Bellver Park', 4, 'Carrer de Bellver 3', 'bellver@altairis.com', '+34 971 000 012', '15:00:00', '11:00:00', 39.5635000, 2.6230000),
(1, 'Hotel Marina Palma', 5, 'Avinguda Gabriel Roca 18', 'marina@altairis.com', '+34 971 000 013', '16:00:00', '11:00:00', 39.5720000, 2.6380000),
(1, 'Hotel Catedral Palma', 4, 'Carrer del Palau Reial 1', 'catedral@altairis.com', '+34 971 000 014', '15:00:00', '12:00:00', 39.5680000, 2.6480000),
(1, 'Hotel Portixol', 3, 'Carrer de Portixol 6', 'portixol@altairis.com', '+34 971 000 015', '14:00:00', '11:00:00', 39.5610000, 2.6650000),
(1, 'Hotel Santa Catalina', 4, 'Carrer de Sant Magí 14', 'santacatalina@altairis.com', '+34 971 000 016', '15:00:00', '11:00:00', 39.5730000, 2.6350000),
(1, 'Hotel Son Vida Palace', 5, 'Urbanització Son Vida 1', 'sonvida@altairis.com', '+34 971 000 017', '15:00:00', '12:00:00', 39.5890000, 2.6180000);

-- Alcúdia (city_id = 4)
INSERT INTO hotel (city_id, name, stars, address, email, phone, check_in_time, check_out_time, latitude, longitude) VALUES
(4, 'Hotel Bahía Alcúdia', 4, 'Avinguda de la Platja 5', 'alcudia@altairis.com', '+34 971 000 003', '15:00:00', '11:00:00', 39.8530000, 3.1210000);

-- Sóller (city_id = 5)
INSERT INTO hotel (city_id, name, stars, address, email, phone, check_in_time, check_out_time, latitude, longitude) VALUES
(5, 'Hotel Serra de Tramuntana', 4, 'Carrer de Sa Lluna 15', 'soller@altairis.com', '+34 971 000 004', '15:00:00', '11:00:00', 39.7667000, 2.7150000);

-- =====================
-- Menorca hotels (cities 11-20)
-- =====================

-- Mahón (city_id = 11)
INSERT INTO hotel (city_id, name, stars, address, email, phone, check_in_time, check_out_time, latitude, longitude) VALUES
(11, 'Hotel Port Mahón', 4, 'Passeig Marítim 3', 'mahon@altairis.com', '+34 971 000 005', '15:00:00', '11:00:00', 39.8886000, 4.2658000);

-- Ciutadella (city_id = 12)
INSERT INTO hotel (city_id, name, stars, address, email, phone, check_in_time, check_out_time, latitude, longitude) VALUES
(12, 'Hotel Ciutadella Mar', 3, 'Plaça des Born 7', 'ciutadella@altairis.com', '+34 971 000 006', '14:00:00', '12:00:00', 40.0011000, 3.8374000);

-- =====================
-- Ibiza hotels (cities 21-30)
-- =====================

-- Ibiza (city_id = 21)
INSERT INTO hotel (city_id, name, stars, address, email, phone, check_in_time, check_out_time, latitude, longitude) VALUES
(21, 'Hotel Dalt Vila', 5, 'Carrer de Sa Carrossa 2', 'ibiza@altairis.com', '+34 971 000 007', '15:00:00', '11:00:00', 38.9067000, 1.4363000);

-- Santa Eulària des Riu (city_id = 22)
INSERT INTO hotel (city_id, name, stars, address, email, phone, check_in_time, check_out_time, latitude, longitude) VALUES
(22, 'Hotel Santa Eulària Beach', 4, 'Passeig de s''Alamera 10', 'santaeularia@altairis.com', '+34 971 000 008', '15:00:00', '11:00:00', 38.9847000, 1.5339000);

-- =====================
-- Barcelona hotels (city_id = 31)
-- =====================

INSERT INTO hotel (city_id, name, stars, address, email, phone, check_in_time, check_out_time, latitude, longitude) VALUES
(31, 'Hotel Altairis Barcelona', 5, 'Passeig de Gràcia 45', 'barcelona@altairis.com', '+34 933 000 001', '15:00:00', '11:00:00', 41.3925000, 2.1648000),
(31, 'Hotel Gótico', 3, 'Carrer d''Avinyó 16', 'gotico@altairis.com', '+34 933 000 002', '14:00:00', '12:00:00', 41.3800000, 2.1770000),
(31, 'Hotel Barceloneta Mar', 4, 'Passeig Marítim 28', 'barceloneta@altairis.com', '+34 933 000 003', '15:00:00', '11:00:00', 41.3780000, 2.1920000);

-- =====================
-- Nice hotels (city_id = 32)
-- =====================

INSERT INTO hotel (city_id, name, stars, address, email, phone, check_in_time, check_out_time, latitude, longitude) VALUES
(32, 'Hotel Promenade Nice', 5, 'Promenade des Anglais 37', 'nice@altairis.com', '+33 493 000 001', '15:00:00', '11:00:00', 43.6953000, 7.2650000),
(32, 'Hotel Vieux Nice', 3, 'Rue de la Préfecture 12', 'vieuxnice@altairis.com', '+33 493 000 002', '14:00:00', '12:00:00', 43.6960000, 7.2750000),
(32, 'Hotel Colline de Cimiez', 4, 'Boulevard de Cimiez 8', 'cimiez@altairis.com', '+33 493 000 003', '15:00:00', '11:00:00', 43.7150000, 7.2750000);

-- Hotel IDs: 1=Altairis Palma, 2=Sol de Palma, 3=Playa de Palma, 4=Casco Antiguo,
-- 5=Bellver Park, 6=Marina Palma, 7=Catedral Palma, 8=Portixol,
-- 9=Santa Catalina, 10=Son Vida Palace, 11=Bahía Alcúdia, 12=Serra de Tramuntana,
-- 13=Port Mahón, 14=Ciutadella Mar, 15=Dalt Vila, 16=Santa Eulària Beach,
-- 17=Altairis Barcelona, 18=Gótico, 19=Barceloneta Mar,
-- 20=Promenade Nice, 21=Vieux Nice, 22=Colline de Cimiez
