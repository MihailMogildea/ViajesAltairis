-- Transaction fees for each payment

-- Visa payments: 2% processing fee
INSERT INTO payment_transaction_fee (id, payment_transaction_id, fee_type, fee_amount, fee_percentage, fixed_fee_amount, currency_id, description, created_at) VALUES
(1, 1, 'processing', 26.45, 2.0000, NULL, 1, 'Visa processing fee', '2026-01-08 11:05:00'),
(2, 3, 'processing', 17.16, 2.0000, NULL, 1, 'Visa processing fee', '2026-01-22 10:15:00'),
(3, 7, 'processing', 102.24, 2.0000, NULL, 1, 'Visa processing fee', '2026-02-18 18:10:00');

-- Mastercard payment: 2% processing fee (GBP)
INSERT INTO payment_transaction_fee (id, payment_transaction_id, fee_type, fee_amount, fee_percentage, fixed_fee_amount, currency_id, description, created_at) VALUES
(4, 2, 'processing', 4.25, 2.0000, NULL, 2, 'Mastercard processing fee', '2026-01-15 15:10:00');

-- Bank transfer payments: flat €15 fee
INSERT INTO payment_transaction_fee (id, payment_transaction_id, fee_type, fee_amount, fee_percentage, fixed_fee_amount, currency_id, description, created_at) VALUES
(5, 4, 'bank_fee', 15.00, NULL, 15.00, 1, 'Bank transfer fee', '2025-12-10 09:30:00'),
(6, 6, 'bank_fee', 15.00, NULL, 15.00, 1, 'Bank transfer fee', '2025-12-28 09:30:00');

-- PayPal payment: 1.5% processing fee
INSERT INTO payment_transaction_fee (id, payment_transaction_id, fee_type, fee_amount, fee_percentage, fixed_fee_amount, currency_id, description, created_at) VALUES
(7, 5, 'processing', 25.97, 1.5000, NULL, 1, 'PayPal processing fee', '2026-02-05 14:15:00');
