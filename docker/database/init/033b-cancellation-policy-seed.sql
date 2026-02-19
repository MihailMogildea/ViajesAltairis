-- 5-star hotels: 72h free cancellation, 50% penalty
INSERT INTO cancellation_policy (hotel_id, free_cancellation_hours, penalty_percentage) VALUES
(1, 72, 50.00),
(6, 72, 50.00),
(10, 72, 50.00),
(15, 72, 50.00),
(17, 72, 50.00),
(20, 72, 50.00);

-- 4-star hotels: 48h free cancellation, 100% penalty
INSERT INTO cancellation_policy (hotel_id, free_cancellation_hours, penalty_percentage) VALUES
(3, 48, 100.00),
(5, 48, 100.00),
(7, 48, 100.00),
(9, 48, 100.00),
(11, 48, 100.00),
(12, 48, 100.00),
(13, 48, 100.00),
(16, 48, 100.00),
(19, 48, 100.00),
(22, 48, 100.00);

-- 3-star hotels: 24h free cancellation, 100% penalty
INSERT INTO cancellation_policy (hotel_id, free_cancellation_hours, penalty_percentage) VALUES
(2, 24, 100.00),
(4, 24, 100.00),
(8, 24, 100.00),
(14, 24, 100.00),
(18, 24, 100.00),
(21, 24, 100.00);
