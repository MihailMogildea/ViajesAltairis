-- =====================
-- Room Summaries (field = 'summary')
-- Seeding for Hotel Altairis Palma (hprt 1-4)
-- =====================

-- Single room (hprt_id=1)
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('hotel_provider_room_type', 1, 'summary', 1, 'Cozy single room with city views, ideal for solo travelers. Includes complimentary breakfast.'),
('hotel_provider_room_type', 1, 'summary', 2, 'Acogedora habitación individual con vistas a la ciudad, ideal para viajeros solos. Incluye desayuno.');

-- Double room (hprt_id=2)
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('hotel_provider_room_type', 2, 'summary', 1, 'Spacious double room with sea views, king-size bed, and modern amenities for a comfortable stay.'),
('hotel_provider_room_type', 2, 'summary', 2, 'Amplia habitación doble con vistas al mar, cama king-size y comodidades modernas para una estancia confortable.');

-- Suite (hprt_id=3)
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('hotel_provider_room_type', 3, 'summary', 1, 'Elegant suite with separate living area, panoramic sea views, jacuzzi, and premium room service available 24/7.'),
('hotel_provider_room_type', 3, 'summary', 2, 'Elegante suite con sala de estar independiente, vistas panorámicas al mar, jacuzzi y servicio de habitaciones premium 24/7.');

-- Deluxe (hprt_id=4)
INSERT INTO translation (entity_type, entity_id, field, language_id, value) VALUES
('hotel_provider_room_type', 4, 'summary', 1, 'Our finest accommodation featuring a private terrace, premium furnishings, and exclusive access to the executive lounge.'),
('hotel_provider_room_type', 4, 'summary', 2, 'Nuestra mejor habitación con terraza privada, mobiliario premium y acceso exclusivo al salón ejecutivo.');
