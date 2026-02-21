-- =====================
-- Payment Transaction Statuses
-- =====================
-- language_id: 1=en, 2=es

INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
-- pending
('payment_transaction_status', 1, 'name', 1, 'Pending'),
('payment_transaction_status', 1, 'name', 2, 'Pendiente'),
-- completed
('payment_transaction_status', 2, 'name', 1, 'Completed'),
('payment_transaction_status', 2, 'name', 2, 'Completado'),
-- cancelled
('payment_transaction_status', 3, 'name', 1, 'Cancelled'),
('payment_transaction_status', 3, 'name', 2, 'Cancelado'),
-- refunded
('payment_transaction_status', 4, 'name', 1, 'Refunded'),
('payment_transaction_status', 4, 'name', 2, 'Reembolsado');
