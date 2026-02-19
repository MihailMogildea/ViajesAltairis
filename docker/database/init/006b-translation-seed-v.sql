-- =====================
-- Board Types
-- =====================

-- room_only
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('board_type', 1, 1, 'Room Only'),
('board_type', 1, 2, 'Solo Alojamiento');

-- bed_and_breakfast
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('board_type', 2, 1, 'Bed & Breakfast'),
('board_type', 2, 2, 'Alojamiento y Desayuno');

-- half_board
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('board_type', 3, 1, 'Half Board'),
('board_type', 3, 2, 'Media Pensión');

-- full_board
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('board_type', 4, 1, 'Full Board'),
('board_type', 4, 2, 'Pensión Completa');

-- all_inclusive
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('board_type', 5, 1, 'All Inclusive'),
('board_type', 5, 2, 'Todo Incluido');
