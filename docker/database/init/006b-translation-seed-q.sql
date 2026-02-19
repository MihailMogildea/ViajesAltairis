-- =====================
-- Reservation Statuses
-- =====================

-- draft
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('reservation_status', 1, 1, 'Draft'),
('reservation_status', 1, 2, 'Borrador');

-- pending
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('reservation_status', 2, 1, 'Pending'),
('reservation_status', 2, 2, 'Pendiente');

-- confirmed
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('reservation_status', 3, 1, 'Confirmed'),
('reservation_status', 3, 2, 'Confirmada');

-- checked_in
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('reservation_status', 4, 1, 'Checked In'),
('reservation_status', 4, 2, 'Registrado');

-- completed
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('reservation_status', 5, 1, 'Completed'),
('reservation_status', 5, 2, 'Completada');

-- cancelled
INSERT INTO translation (entity_type, entity_id, language_id, value) VALUES
('reservation_status', 6, 1, 'Cancelled'),
('reservation_status', 6, 2, 'Cancelada');
