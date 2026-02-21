-- Passwords are bcrypt hashes of 'password123' for dev purposes
-- $2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O

-- Admin
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, address, city, postal_code, country, language_id, created_at) VALUES
(1, 'admin@altairis.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Carlos', 'Administrador', '+34 600 000 001', 'Carrer de Velàzquez 10', 'Palma', '07001', 'Spain', 1, '2025-06-01 09:00:00');

-- Manager
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, address, city, postal_code, country, language_id, created_at) VALUES
(2, 'manager@altairis.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'María', 'García', '+34 600 000 002', 'Carrer de Sant Miquel 22', 'Palma', '07002', 'Spain', 2, '2025-06-15 10:00:00');

-- Agents
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, address, city, postal_code, country, language_id, created_at) VALUES
(3, 'agent1@altairis.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Pedro', 'López', '+34 600 000 003', 'Avinguda Jaume III 5', 'Palma', '07012', 'Spain', 2, '2025-07-01 08:30:00'),
(3, 'agent2@altairis.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Sophie', 'Dupont', '+33 600 000 004', 'Rue de France 15', 'Nice', '06000', 'France', 1, '2025-07-15 09:00:00');

-- Hotel Staff - Regional manager (linked to provider)
-- user_id=5, manages all Mallorca hotels via provider_id=1
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, address, city, postal_code, country, language_id, provider_id, created_at) VALUES
(4, 'regional.mallorca@altairis.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Antoni', 'Barceló', '+34 600 000 030', 'Carrer de la Unió 8', 'Palma', '07001', 'Spain', 2, 1, '2025-08-01 08:00:00');

-- Hotel Staff - Hotel directors (linked to specific hotels via user_hotel table)
-- user_id=6, director of Hotel Altairis Palma
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, address, city, postal_code, country, language_id, created_at) VALUES
(4, 'director.palma@altairis.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Marta', 'Serra', '+34 600 000 031', 'Paseo Marítimo 14', 'Palma', '07014', 'Spain', 2, '2025-08-15 09:00:00');

-- user_id=7, director of Hotel Dalt Vila (Ibiza)
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, address, city, postal_code, country, language_id, created_at) VALUES
(4, 'director.ibiza@altairis.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Tomás', 'Tur', '+34 600 000 032', 'Carrer de la Virgen 5', 'Ibiza', '07800', 'Spain', 2, '2025-09-01 10:00:00');

-- Clients (user_type_id=5)
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, address, city, postal_code, country, language_id, created_at) VALUES
(5, 'client1@example.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Juan', 'Martínez', '+34 600 000 005', 'Calle Aragón 120', 'Barcelona', '08015', 'Spain', 2, '2025-10-15 14:30:00'),
(5, 'client2@example.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Emma', 'Wilson', '+44 700 000 006', '12 Baker Street', 'London', 'NW1 6XE', 'United Kingdom', 1, '2025-11-20 16:00:00');

-- Business Partner users (Viajes Sol - partner_id=1)
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, language_id, business_partner_id, created_at) VALUES
(3, 'ana@viajessol.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Ana', 'Fernández', '+34 911 000 010', 2, 1, '2025-09-01 11:00:00'),
(3, 'luis@viajessol.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Luis', 'Romero', '+34 911 000 011', 2, 1, '2025-09-15 12:00:00');

-- Business Partner users (Mediterranean Tours - partner_id=2)
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, language_id, business_partner_id, created_at) VALUES
(3, 'james@medtours.co.uk', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'James', 'Taylor', '+44 207 000 020', 1, 2, '2025-10-01 09:00:00');

-- Additional clients for user growth demo data (user_type_id=5)
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, address, city, postal_code, country, language_id, created_at) VALUES
(5, 'maria.lopez@example.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'María', 'López', '+34 655 100 001', 'Calle Gran Vía 45', 'Madrid', '28013', 'Spain', 2, '2025-10-28 11:30:00'),
(5, 'thomas.brown@example.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Thomas', 'Brown', '+44 777 200 002', '8 Oxford Street', 'London', 'W1D 1BS', 'United Kingdom', 1, '2025-11-10 15:00:00'),
(5, 'laura.chen@example.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Laura', 'Chen', '+1 415 300 003', '500 Market Street', 'San Francisco', '94105', 'United States', 1, '2025-12-05 10:00:00'),
(5, 'andre.petit@example.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'André', 'Petit', '+33 612 400 004', '20 Rue du Faubourg', 'Paris', '75008', 'France', 1, '2025-12-18 14:00:00'),
(5, 'sofia.rossi@example.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Sofia', 'Rossi', '+39 345 500 005', 'Via Roma 12', 'Milan', '20121', 'Italy', 1, '2026-01-03 09:30:00'),
(5, 'hans.muller@example.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Hans', 'Müller', '+49 170 600 006', 'Königstraße 40', 'Stuttgart', '70173', 'Germany', 1, '2026-01-15 13:00:00'),
(5, 'yuki.tanaka@example.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Yuki', 'Tanaka', '+81 90 7000 0007', '1-2-3 Shibuya', 'Tokyo', '150-0002', 'Japan', 1, '2026-01-28 18:00:00'),
(5, 'isabelle.martin@example.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Isabelle', 'Martin', '+33 678 800 008', '5 Promenade des Anglais', 'Nice', '06000', 'France', 1, '2026-02-10 11:00:00');
