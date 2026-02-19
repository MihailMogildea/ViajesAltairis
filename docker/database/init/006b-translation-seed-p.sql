-- =====================
-- User Types
-- =====================

-- admin
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('user_type', 1, 1, 'Administrator'),
('user_type', 1, 2, 'Administrador');

-- manager
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('user_type', 2, 1, 'Manager'),
('user_type', 2, 2, 'Gerente');

-- agent
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('user_type', 3, 1, 'Agent'),
('user_type', 3, 2, 'Agente');

-- hotel_staff
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('user_type', 4, 1, 'Hotel Staff'),
('user_type', 4, 2, 'Personal de hotel');

-- client
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('user_type', 5, 1, 'Client'),
('user_type', 5, 2, 'Cliente');
