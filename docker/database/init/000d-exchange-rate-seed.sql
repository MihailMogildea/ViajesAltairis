-- EUR to EUR is always 1
INSERT INTO exchange_rate (currency_id, rate_to_eur, valid_from, valid_to) VALUES
(1, 1.000000, '2025-01-01 00:00:00', '2099-12-31 23:59:59');

-- GBP to EUR (approximate rate)
INSERT INTO exchange_rate (currency_id, rate_to_eur, valid_from, valid_to) VALUES
(2, 1.170000, '2025-01-01 00:00:00', '2025-12-31 23:59:59');
