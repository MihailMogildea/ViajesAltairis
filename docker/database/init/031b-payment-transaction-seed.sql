-- Payment transactions for paid invoices (status_id=2 = completed)

-- Invoice 1: Juan, Altairis Palma (visa)
INSERT INTO payment_transaction (id, reservation_id, payment_method_id, transaction_reference, amount, currency_id, exchange_rate_id, status_id, created_at) VALUES
(1, 1, 1, 'TXN-2026-VIS-000001', 1322.50, 1, 1, 2, '2026-01-08 11:05:00');

-- Invoice 2: Emma, Sol de Palma (mastercard, GBP)
INSERT INTO payment_transaction (id, reservation_id, payment_method_id, transaction_reference, amount, currency_id, exchange_rate_id, status_id, created_at) VALUES
(2, 2, 2, 'TXN-2026-MC-000002', 212.25, 2, 2, 2, '2026-01-15 15:10:00');

-- Invoice 3: Juan, Playa de Palma (visa)
INSERT INTO payment_transaction (id, reservation_id, payment_method_id, transaction_reference, amount, currency_id, exchange_rate_id, status_id, created_at) VALUES
(3, 3, 1, 'TXN-2026-VIS-000003', 858.00, 1, 1, 2, '2026-01-22 10:15:00');

-- Invoice 5: Roberto, Bellver Park (bank transfer, B2B)
INSERT INTO payment_transaction (id, reservation_id, payment_method_id, transaction_reference, amount, currency_id, exchange_rate_id, status_id, created_at) VALUES
(4, 5, 3, 'TXN-2025-BT-000004', 2989.00, 1, 1, 2, '2025-12-10 09:30:00');

-- Invoice 6: Juan, Marina Palma (paypal)
INSERT INTO payment_transaction (id, reservation_id, payment_method_id, transaction_reference, amount, currency_id, exchange_rate_id, status_id, created_at) VALUES
(5, 6, 4, 'TXN-2026-PP-000005', 1731.00, 1, 1, 2, '2026-02-05 14:15:00');

-- Invoice 8: Carmen, Portixol (bank transfer, B2B)
INSERT INTO payment_transaction (id, reservation_id, payment_method_id, transaction_reference, amount, currency_id, exchange_rate_id, status_id, created_at) VALUES
(6, 8, 3, 'TXN-2025-BT-000006', 179.00, 1, 1, 2, '2025-12-28 09:30:00');

-- Invoice 10: Friedrich, Son Vida Palace (visa)
INSERT INTO payment_transaction (id, reservation_id, payment_method_id, transaction_reference, amount, currency_id, exchange_rate_id, status_id, created_at) VALUES
(7, 10, 1, 'TXN-2026-VIS-000007', 5112.00, 1, 1, 2, '2026-02-18 18:10:00');
