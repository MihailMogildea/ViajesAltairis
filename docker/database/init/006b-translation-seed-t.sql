-- =====================
-- Payment Methods
-- =====================

-- visa
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('payment_method', 1, 1, 'Visa'),
('payment_method', 1, 2, 'Visa');

-- mastercard
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('payment_method', 2, 1, 'Mastercard'),
('payment_method', 2, 2, 'Mastercard');

-- bank_transfer
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('payment_method', 3, 1, 'Bank Transfer'),
('payment_method', 3, 2, 'Transferencia bancaria');

-- paypal
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('payment_method', 4, 1, 'PayPal'),
('payment_method', 4, 2, 'PayPal');
