-- Juan: 5 nights double room + B&B at Hotel Altairis Palma
-- hprt_id=2 (Altairis Palma double, 180€/night), board_type=2 (B&B, 30€/night)
INSERT INTO reservation_line (id, reservation_id, hotel_provider_room_type_id, board_type_id, check_in_date, check_out_date, num_rooms, num_guests, price_per_night, board_price_per_night, num_nights, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id) VALUES
(1, 1, 2, 2, '2026-01-10', '2026-01-15', 1, 2, 180.00, 30.00, 5, 1050.00, 115.00, 157.50, 0.00, 1322.50, 1, 1);

-- Emma: 3 nights single room only at Hotel Sol de Palma
-- hprt_id=5 (Sol de Palma single, 55€/night), board_type=1 (room only, 0€)
INSERT INTO reservation_line (id, reservation_id, hotel_provider_room_type_id, board_type_id, check_in_date, check_out_date, num_rooms, num_guests, price_per_night, board_price_per_night, num_nights, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id) VALUES
(2, 2, 5, 1, '2026-02-01', '2026-02-04', 1, 1, 55.00, 0.00, 3, 165.00, 22.50, 24.75, 0.00, 212.25, 2, 2);

-- Juan: 4 nights double room + half board at Hotel Playa de Palma
-- hprt_id=9 (Playa de Palma double, 130€/night), board_type=3 (half board, 40€/night)
INSERT INTO reservation_line (id, reservation_id, hotel_provider_room_type_id, board_type_id, check_in_date, check_out_date, num_rooms, num_guests, price_per_night, board_price_per_night, num_nights, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id) VALUES
(3, 3, 9, 3, '2026-02-15', '2026-02-19', 1, 2, 130.00, 40.00, 4, 680.00, 76.00, 102.00, 0.00, 858.00, 1, 1);

-- Emma: 3 nights twin room + B&B at Hotel Casco Antiguo
-- hprt_id=13 (Casco Antiguo twin, 70€/night), board_type=2 (B&B, 10€/night)
INSERT INTO reservation_line (id, reservation_id, hotel_provider_room_type_id, board_type_id, check_in_date, check_out_date, num_rooms, num_guests, price_per_night, board_price_per_night, num_nights, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id) VALUES
(4, 4, 13, 2, '2026-01-20', '2026-01-23', 1, 2, 70.00, 10.00, 3, 240.00, 30.00, 36.00, 0.00, 306.00, 2, 2);

-- Roberto: 7 nights suite + full board at Hotel Bellver Park
-- hprt_id=16 (Bellver Park suite, 280€/night), board_type=4 (full board, 60€/night)
INSERT INTO reservation_line (id, reservation_id, hotel_provider_room_type_id, board_type_id, check_in_date, check_out_date, num_rooms, num_guests, price_per_night, board_price_per_night, num_nights, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id) VALUES
(5, 5, 16, 4, '2025-12-22', '2025-12-29', 1, 2, 280.00, 60.00, 7, 2380.00, 252.00, 357.00, 0.00, 2989.00, 1, 1);

-- Juan: 3 nights suite + all inclusive at Hotel Marina Palma
-- hprt_id=18 (Marina Palma suite, 380€/night), board_type=5 (all inclusive, 120€/night)
INSERT INTO reservation_line (id, reservation_id, hotel_provider_room_type_id, board_type_id, check_in_date, check_out_date, num_rooms, num_guests, price_per_night, board_price_per_night, num_nights, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id) VALUES
(6, 6, 18, 5, '2026-03-01', '2026-03-04', 1, 2, 380.00, 120.00, 3, 1500.00, 156.00, 225.00, 0.00, 1881.00, 1, 1);

-- Oliver: 5 nights double room + B&B at Hotel Catedral Palma
-- hprt_id=21 (Catedral Palma double, 145€/night), board_type=2 (B&B, 18€/night)
INSERT INTO reservation_line (id, reservation_id, hotel_provider_room_type_id, board_type_id, check_in_date, check_out_date, num_rooms, num_guests, price_per_night, board_price_per_night, num_nights, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id) VALUES
(7, 7, 21, 2, '2026-01-05', '2026-01-10', 1, 1, 145.00, 18.00, 5, 815.00, 91.50, 122.25, 0.00, 1028.75, 2, 2);

-- Carmen: 2 nights double room only at Hotel Portixol
-- hprt_id=24 (Portixol double, 70€/night), board_type=1 (room only, 0€)
INSERT INTO reservation_line (id, reservation_id, hotel_provider_room_type_id, board_type_id, check_in_date, check_out_date, num_rooms, num_guests, price_per_night, board_price_per_night, num_nights, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id) VALUES
(8, 8, 24, 1, '2026-02-10', '2026-02-12', 1, 2, 70.00, 0.00, 2, 140.00, 18.00, 21.00, 0.00, 179.00, 1, 1);

-- Emma: 4 nights junior suite + half board at Hotel Santa Catalina
-- hprt_id=28 (Santa Catalina jr_suite, 210€/night), board_type=3 (half board, 40€/night)
INSERT INTO reservation_line (id, reservation_id, hotel_provider_room_type_id, board_type_id, check_in_date, check_out_date, num_rooms, num_guests, price_per_night, board_price_per_night, num_nights, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id) VALUES
(9, 9, 28, 3, '2026-03-10', '2026-03-14', 1, 1, 210.00, 40.00, 4, 1000.00, 108.00, 150.00, 0.00, 1258.00, 2, 2);

-- Friedrich: 6 nights deluxe room + all inclusive at Hotel Son Vida Palace
-- hprt_id=31 (Son Vida Palace deluxe, 550€/night), board_type=5 (all inclusive, 130€/night)
INSERT INTO reservation_line (id, reservation_id, hotel_provider_room_type_id, board_type_id, check_in_date, check_out_date, num_rooms, num_guests, price_per_night, board_price_per_night, num_nights, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id) VALUES
(10, 10, 31, 5, '2026-01-25', '2026-01-31', 1, 2, 550.00, 130.00, 6, 4080.00, 420.00, 612.00, 0.00, 5112.00, 1, 1);
