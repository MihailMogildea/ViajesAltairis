-- =====================
-- Tax Types
-- =====================

-- vat
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('tax_type', 1, 1, 'VAT'),
('tax_type', 1, 2, 'IVA');

-- tourist_tax
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('tax_type', 2, 1, 'Tourist Tax'),
('tax_type', 2, 2, 'Tasa turística');

-- city_tax
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('tax_type', 3, 1, 'City Tax'),
('tax_type', 3, 2, 'Tasa municipal');
