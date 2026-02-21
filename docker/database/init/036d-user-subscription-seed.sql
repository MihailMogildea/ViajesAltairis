-- Client subscriptions
-- Juan (id=8): Premium plan since Nov 2025
INSERT INTO user_subscription (id, user_id, subscription_type_id, start_date, end_date, active, created_at) VALUES
(1, 8, 2, '2025-11-01', NULL, 1, '2025-11-01 10:00:00');

-- Emma (id=9): Basic plan since Dec 2025
INSERT INTO user_subscription (id, user_id, subscription_type_id, start_date, end_date, active, created_at) VALUES
(2, 9, 1, '2025-12-15', NULL, 1, '2025-12-15 14:00:00');

-- Sofia (id=17): VIP plan since Jan 2026
INSERT INTO user_subscription (id, user_id, subscription_type_id, start_date, end_date, active, created_at) VALUES
(3, 17, 3, '2026-01-05', NULL, 1, '2026-01-05 09:00:00');

-- Thomas (id=14): Basic plan, started Nov 2025, cancelled Dec 2025
INSERT INTO user_subscription (id, user_id, subscription_type_id, start_date, end_date, active, created_at) VALUES
(4, 14, 1, '2025-11-15', '2025-12-15', 0, '2025-11-15 16:00:00');

-- Hans (id=18): Premium plan since Jan 2026
INSERT INTO user_subscription (id, user_id, subscription_type_id, start_date, end_date, active, created_at) VALUES
(5, 18, 2, '2026-01-20', NULL, 1, '2026-01-20 11:00:00');
