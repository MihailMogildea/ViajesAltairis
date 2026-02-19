-- =====================
-- Subscription Types
-- =====================

-- basic
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('subscription_type', 1, 1, 'Basic'),
('subscription_type', 1, 2, 'Básico');

-- premium
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('subscription_type', 2, 1, 'Premium'),
('subscription_type', 2, 2, 'Premium');

-- vip
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('subscription_type', 3, 1, 'VIP'),
('subscription_type', 3, 2, 'VIP');
