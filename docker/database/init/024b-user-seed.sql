-- Passwords are bcrypt hashes of 'password123' for dev purposes
-- $2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O

-- Admin
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, address, city, postal_code, country, language_id) VALUES
(1, 'admin@altairis.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Carlos', 'Administrador', '+34 600 000 001', 'Carrer de Velàzquez 10', 'Palma', '07001', 'Spain', 1);

-- Manager
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, address, city, postal_code, country, language_id) VALUES
(2, 'manager@altairis.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'María', 'García', '+34 600 000 002', 'Carrer de Sant Miquel 22', 'Palma', '07002', 'Spain', 2);

-- Agents
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, address, city, postal_code, country, language_id) VALUES
(3, 'agent1@altairis.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Pedro', 'López', '+34 600 000 003', 'Avinguda Jaume III 5', 'Palma', '07012', 'Spain', 2),
(3, 'agent2@altairis.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Sophie', 'Dupont', '+33 600 000 004', 'Rue de France 15', 'Nice', '06000', 'France', 1);

-- Hotel Staff - Regional manager (linked to provider)
-- user_id=5, manages all Mallorca hotels via provider_id=1
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, address, city, postal_code, country, language_id, provider_id) VALUES
(4, 'regional.mallorca@altairis.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Antoni', 'Barceló', '+34 600 000 030', 'Carrer de la Unió 8', 'Palma', '07001', 'Spain', 2, 1);

-- Hotel Staff - Hotel directors (linked to specific hotels via user_hotel table)
-- user_id=6, director of Hotel Altairis Palma
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, address, city, postal_code, country, language_id) VALUES
(4, 'director.palma@altairis.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Marta', 'Serra', '+34 600 000 031', 'Paseo Marítimo 14', 'Palma', '07014', 'Spain', 2);

-- user_id=7, director of Hotel Dalt Vila (Ibiza)
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, address, city, postal_code, country, language_id) VALUES
(4, 'director.ibiza@altairis.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Tomás', 'Tur', '+34 600 000 032', 'Carrer de la Virgen 5', 'Ibiza', '07800', 'Spain', 2);

-- Clients
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, address, city, postal_code, country, language_id) VALUES
(5, 'client1@example.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Juan', 'Martínez', '+34 600 000 005', 'Calle Aragón 120', 'Barcelona', '08015', 'Spain', 2),
(5, 'client2@example.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Emma', 'Wilson', '+44 700 000 006', '12 Baker Street', 'London', 'NW1 6XE', 'United Kingdom', 1);

-- Business Partner users (Viajes Sol - partner_id=1)
-- Address comes from business_partner table for invoicing
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, language_id, business_partner_id) VALUES
(3, 'ana@viajessol.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Ana', 'Fernández', '+34 911 000 010', 2, 1),
(3, 'luis@viajessol.com', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'Luis', 'Romero', '+34 911 000 011', 2, 1);

-- Business Partner users (Mediterranean Tours - partner_id=2)
INSERT INTO user (user_type_id, email, password_hash, first_name, last_name, phone, language_id, business_partner_id) VALUES
(3, 'james@medtours.co.uk', '$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O', 'James', 'Taylor', '+44 207 000 020', 1, 2);
