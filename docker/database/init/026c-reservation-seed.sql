-- Completed reservation: Juan at Hotel Altairis Palma (5 stars)
-- Booked by agent Pedro, owner Juan (client)
INSERT INTO reservation (id, reservation_code, status_id, booked_by_user_id, owner_user_id, owner_first_name, owner_last_name, owner_email, owner_phone, owner_tax_id, owner_address, owner_city, owner_postal_code, owner_country, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id, created_at) VALUES
(1, 'ALT-2026-000001', 5, 3, 8, 'Juan', 'Martínez', 'client1@example.com', '+34 600 000 005', NULL, 'Calle Aragón 120', 'Barcelona', '08015', 'Spain', 1050.00, 115.00, 157.50, 0.00, 1322.50, 1, 1, '2026-01-08 10:30:00');

-- Completed reservation: Emma at Hotel Sol de Palma (3 stars)
-- Booked by agent Sophie, owner Emma (client)
INSERT INTO reservation (id, reservation_code, status_id, booked_by_user_id, owner_user_id, owner_first_name, owner_last_name, owner_email, owner_phone, owner_tax_id, owner_address, owner_city, owner_postal_code, owner_country, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id, created_at) VALUES
(2, 'ALT-2026-000002', 5, 4, 9, 'Emma', 'Wilson', 'client2@example.com', '+44 700 000 006', NULL, '12 Baker Street', 'London', 'NW1 6XE', 'United Kingdom', 165.00, 22.50, 24.75, 0.00, 212.25, 2, 2, '2026-01-15 14:15:00');

-- Completed reservation: Juan at Hotel Playa de Palma (4 stars)
-- Booked by agent Pedro, owner Juan (client)
INSERT INTO reservation (id, reservation_code, status_id, booked_by_user_id, owner_user_id, owner_first_name, owner_last_name, owner_email, owner_phone, owner_tax_id, owner_address, owner_city, owner_postal_code, owner_country, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id, created_at) VALUES
(3, 'ALT-2026-000003', 5, 3, 8, 'Juan', 'Martínez', 'client1@example.com', '+34 600 000 005', NULL, 'Calle Aragón 120', 'Barcelona', '08015', 'Spain', 680.00, 76.00, 102.00, 0.00, 858.00, 1, 1, '2026-01-22 09:45:00');

-- Completed reservation: Emma at Hotel Casco Antiguo (3 stars)
-- Booked by agent Sophie, owner Emma (client)
INSERT INTO reservation (id, reservation_code, status_id, booked_by_user_id, owner_user_id, owner_first_name, owner_last_name, owner_email, owner_phone, owner_tax_id, owner_address, owner_city, owner_postal_code, owner_country, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id, created_at) VALUES
(4, 'ALT-2026-000004', 5, 4, 9, 'Emma', 'Wilson', 'client2@example.com', '+44 700 000 006', NULL, '12 Baker Street', 'London', 'NW1 6XE', 'United Kingdom', 240.00, 30.00, 36.00, 0.00, 306.00, 2, 2, '2026-01-30 16:00:00');

-- Completed reservation: Roberto García at Hotel Bellver Park (4 stars)
-- Booked by B2B agent Ana (Viajes Sol), walk-in owner
INSERT INTO reservation (id, reservation_code, status_id, booked_by_user_id, owner_user_id, owner_first_name, owner_last_name, owner_email, owner_phone, owner_tax_id, owner_address, owner_city, owner_postal_code, owner_country, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id, created_at) VALUES
(5, 'ALT-2025-000005', 5, 10, NULL, 'Roberto', 'García', 'roberto.garcia@email.com', '+34 655 123 456', NULL, 'Calle Mayor 45', 'Madrid', '28013', 'Spain', 2380.00, 252.00, 357.00, 0.00, 2989.00, 1, 1, '2025-12-05 11:20:00');

-- Completed reservation: Juan at Hotel Marina Palma (5 stars) — used promo code WELCOME10 (10% off)
-- Booked by agent Pedro, owner Juan (client)
INSERT INTO reservation (id, reservation_code, status_id, booked_by_user_id, owner_user_id, owner_first_name, owner_last_name, owner_email, owner_phone, owner_tax_id, owner_address, owner_city, owner_postal_code, owner_country, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id, promo_code_id, created_at) VALUES
(6, 'ALT-2026-000006', 5, 3, 8, 'Juan', 'Martínez', 'client1@example.com', '+34 600 000 005', NULL, 'Calle Aragón 120', 'Barcelona', '08015', 'Spain', 1500.00, 156.00, 225.00, 150.00, 1731.00, 1, 1, 1, '2026-02-05 13:30:00');

-- Completed reservation: Oliver Smith at Hotel Catedral Palma (4 stars)
-- Booked by B2B agent James (Mediterranean Tours), walk-in owner
INSERT INTO reservation (id, reservation_code, status_id, booked_by_user_id, owner_user_id, owner_first_name, owner_last_name, owner_email, owner_phone, owner_tax_id, owner_address, owner_city, owner_postal_code, owner_country, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id, created_at) VALUES
(7, 'ALT-2025-000007', 5, 12, NULL, 'Oliver', 'Smith', 'oliver.smith@email.co.uk', '+44 777 888 999', NULL, '5 High Street', 'Manchester', 'M1 1AD', 'United Kingdom', 815.00, 91.50, 122.25, 0.00, 1028.75, 2, 2, '2025-12-18 15:45:00');

-- Completed reservation: Carmen Ruiz at Hotel Portixol (3 stars)
-- Booked by B2B agent Luis (Viajes Sol), walk-in owner
INSERT INTO reservation (id, reservation_code, status_id, booked_by_user_id, owner_user_id, owner_first_name, owner_last_name, owner_email, owner_phone, owner_tax_id, owner_address, owner_city, owner_postal_code, owner_country, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id, created_at) VALUES
(8, 'ALT-2025-000008', 5, 11, NULL, 'Carmen', 'Ruiz', 'carmen.ruiz@email.com', '+34 666 555 444', NULL, 'Avenida de la Constitución 78', 'Sevilla', '41004', 'Spain', 140.00, 18.00, 21.00, 0.00, 179.00, 1, 1, '2025-12-28 08:15:00');

-- Completed reservation: Emma at Hotel Santa Catalina (4 stars) — used promo code VIP2026 (15% off)
-- Booked by agent Sophie, owner Emma (client)
INSERT INTO reservation (id, reservation_code, status_id, booked_by_user_id, owner_user_id, owner_first_name, owner_last_name, owner_email, owner_phone, owner_tax_id, owner_address, owner_city, owner_postal_code, owner_country, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id, promo_code_id, created_at) VALUES
(9, 'ALT-2026-000009', 5, 4, 9, 'Emma', 'Wilson', 'client2@example.com', '+44 700 000 006', NULL, '12 Baker Street', 'London', 'NW1 6XE', 'United Kingdom', 1000.00, 108.00, 150.00, 150.00, 1108.00, 2, 2, 3, '2026-02-12 12:00:00');

-- Completed reservation: Friedrich Weber at Hotel Son Vida Palace (5 stars)
-- Booked by agent Pedro, walk-in owner
INSERT INTO reservation (id, reservation_code, status_id, booked_by_user_id, owner_user_id, owner_first_name, owner_last_name, owner_email, owner_phone, owner_tax_id, owner_address, owner_city, owner_postal_code, owner_country, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id, created_at) VALUES
(10, 'ALT-2026-000010', 5, 3, NULL, 'Friedrich', 'Weber', 'friedrich.weber@email.de', '+49 170 123 4567', NULL, 'Königstraße 25', 'Stuttgart', '70173', 'Germany', 4080.00, 420.00, 612.00, 0.00, 5112.00, 1, 1, '2026-02-18 17:30:00');

-- Cancelled reservation: Sofia at Hotel Bahía Alcúdia (4 stars)
-- Booked by agent Pedro, owner Sofia (client). Cancelled 5 days later.
INSERT INTO reservation (id, reservation_code, status_id, booked_by_user_id, owner_user_id, owner_first_name, owner_last_name, owner_email, owner_phone, owner_tax_id, owner_address, owner_city, owner_postal_code, owner_country, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id, created_at) VALUES
(11, 'ALT-2026-000011', 6, 3, 17, 'Sofia', 'Rossi', 'sofia.rossi@example.com', '+39 345 500 005', NULL, 'Via Roma 12', 'Milan', '20121', 'Italy', 420.00, 42.00, 63.00, 0.00, 525.00, 1, 1, '2026-01-05 10:00:00');

-- Cancelled reservation: Hans at Hotel Promenade Nice (5 stars)
-- Booked by agent Sophie, owner Hans (client). Late cancellation with penalty.
INSERT INTO reservation (id, reservation_code, status_id, booked_by_user_id, owner_user_id, owner_first_name, owner_last_name, owner_email, owner_phone, owner_tax_id, owner_address, owner_city, owner_postal_code, owner_country, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id, created_at) VALUES
(12, 'ALT-2026-000012', 6, 4, 18, 'Hans', 'Müller', 'hans.muller@example.com', '+49 170 600 006', NULL, 'Königstraße 40', 'Stuttgart', '70173', 'Germany', 360.00, 36.00, 36.00, 0.00, 432.00, 1, 1, '2026-01-20 14:30:00');

-- Update promo code usage counts
UPDATE promo_code SET current_uses = 1 WHERE id = 1;
UPDATE promo_code SET current_uses = 1 WHERE id = 3;
