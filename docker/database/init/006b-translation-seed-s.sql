-- =====================
-- Invoice Statuses
-- =====================

-- created
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('invoice_status', 1, 1, 'Created'),
('invoice_status', 1, 2, 'Creada');

-- paid
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('invoice_status', 2, 1, 'Paid'),
('invoice_status', 2, 2, 'Pagada');

-- refunded
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('invoice_status', 3, 1, 'Refunded'),
('invoice_status', 3, 2, 'Reembolsada');
