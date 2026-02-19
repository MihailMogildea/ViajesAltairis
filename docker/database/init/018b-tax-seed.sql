-- Spain VAT (IVA) 10% - country level
INSERT INTO tax (tax_type_id, country_id, rate, is_percentage) VALUES
(1, 1, 10.0000, TRUE);

-- Balearic Islands tourist tax - 2.00 per night (division level, division_id=1)
INSERT INTO tax (tax_type_id, administrative_division_id, rate, is_percentage) VALUES
(2, 1, 2.0000, FALSE);

-- France VAT (TVA) 10% - country level
INSERT INTO tax (tax_type_id, country_id, rate, is_percentage) VALUES
(1, 2, 10.0000, TRUE);

-- Nice city tax - 1.50 per night (city level, city_id=32)
INSERT INTO tax (tax_type_id, city_id, rate, is_percentage) VALUES
(3, 32, 1.5000, FALSE);
