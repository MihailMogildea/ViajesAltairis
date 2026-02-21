-- Sofia cancelled 5 days after booking (free cancellation, no penalty)
INSERT INTO cancellation (id, reservation_id, cancelled_by_user_id, reason, penalty_percentage, penalty_amount, refund_amount, currency_id, created_at) VALUES
(1, 11, 17, 'Change of travel plans due to family emergency.', 0.00, 0.00, 525.00, 1, '2026-01-10 09:30:00');

-- Hans cancelled late (25% penalty, within 48h of check-in)
INSERT INTO cancellation (id, reservation_id, cancelled_by_user_id, reason, penalty_percentage, penalty_amount, refund_amount, currency_id, created_at) VALUES
(2, 12, 18, 'Flight cancelled, unable to travel.', 25.00, 108.00, 324.00, 1, '2026-01-25 16:00:00');
