-- Invoices for completed reservations (one per reservation)
-- period_start/end match check_in/check_out dates from reservation lines

-- Reservation 1: Juan at Altairis Palma (paid)
INSERT INTO invoice (id, invoice_number, status_id, reservation_id, business_partner_id, subtotal, tax_amount, discount_amount, total_amount, period_start, period_end, created_at) VALUES
(1, 'INV-2026-000001', 2, 1, NULL, 1050.00, 115.00, 0.00, 1322.50, '2026-01-10', '2026-01-15', '2026-01-08 11:00:00');

-- Reservation 2: Emma at Sol de Palma (paid)
INSERT INTO invoice (id, invoice_number, status_id, reservation_id, business_partner_id, subtotal, tax_amount, discount_amount, total_amount, period_start, period_end, created_at) VALUES
(2, 'INV-2026-000002', 2, 2, NULL, 165.00, 22.50, 0.00, 212.25, '2026-02-01', '2026-02-04', '2026-01-15 15:00:00');

-- Reservation 3: Juan at Playa de Palma (paid)
INSERT INTO invoice (id, invoice_number, status_id, reservation_id, business_partner_id, subtotal, tax_amount, discount_amount, total_amount, period_start, period_end, created_at) VALUES
(3, 'INV-2026-000003', 2, 3, NULL, 680.00, 76.00, 0.00, 858.00, '2026-02-15', '2026-02-19', '2026-01-22 10:00:00');

-- Reservation 4: Emma at Casco Antiguo (created, not yet paid)
INSERT INTO invoice (id, invoice_number, status_id, reservation_id, business_partner_id, subtotal, tax_amount, discount_amount, total_amount, period_start, period_end, created_at) VALUES
(4, 'INV-2026-000004', 1, 4, NULL, 240.00, 30.00, 0.00, 306.00, '2026-01-20', '2026-01-23', '2026-01-30 16:30:00');

-- Reservation 5: Roberto at Bellver Park (paid, B2B Viajes Sol)
INSERT INTO invoice (id, invoice_number, status_id, reservation_id, business_partner_id, subtotal, tax_amount, discount_amount, total_amount, period_start, period_end, created_at) VALUES
(5, 'INV-2025-000005', 2, 5, 1, 2380.00, 252.00, 0.00, 2989.00, '2025-12-22', '2025-12-29', '2025-12-05 12:00:00');

-- Reservation 6: Juan at Marina Palma (paid, promo WELCOME10)
INSERT INTO invoice (id, invoice_number, status_id, reservation_id, business_partner_id, subtotal, tax_amount, discount_amount, total_amount, period_start, period_end, created_at) VALUES
(6, 'INV-2026-000006', 2, 6, NULL, 1500.00, 156.00, 150.00, 1731.00, '2026-03-01', '2026-03-04', '2026-02-05 14:00:00');

-- Reservation 7: Oliver at Catedral Palma (created, B2B Mediterranean Tours)
INSERT INTO invoice (id, invoice_number, status_id, reservation_id, business_partner_id, subtotal, tax_amount, discount_amount, total_amount, period_start, period_end, created_at) VALUES
(7, 'INV-2025-000007', 1, 7, 2, 815.00, 91.50, 0.00, 1028.75, '2026-01-05', '2026-01-10', '2025-12-18 16:00:00');

-- Reservation 8: Carmen at Portixol (paid, B2B Viajes Sol)
INSERT INTO invoice (id, invoice_number, status_id, reservation_id, business_partner_id, subtotal, tax_amount, discount_amount, total_amount, period_start, period_end, created_at) VALUES
(8, 'INV-2025-000008', 2, 8, 1, 140.00, 18.00, 0.00, 179.00, '2026-02-10', '2026-02-12', '2025-12-28 09:00:00');

-- Reservation 9: Emma at Santa Catalina (created, promo VIP2026)
INSERT INTO invoice (id, invoice_number, status_id, reservation_id, business_partner_id, subtotal, tax_amount, discount_amount, total_amount, period_start, period_end, created_at) VALUES
(9, 'INV-2026-000009', 1, 9, NULL, 1000.00, 108.00, 150.00, 1108.00, '2026-03-10', '2026-03-14', '2026-02-12 12:30:00');

-- Reservation 10: Friedrich at Son Vida Palace (paid)
INSERT INTO invoice (id, invoice_number, status_id, reservation_id, business_partner_id, subtotal, tax_amount, discount_amount, total_amount, period_start, period_end, created_at) VALUES
(10, 'INV-2026-000010', 2, 10, NULL, 4080.00, 420.00, 0.00, 5112.00, '2026-01-25', '2026-01-31', '2026-02-18 18:00:00');
