-- Room amenity IDs: 9=air_conditioning, 10=tv, 11=wifi, 12=minibar, 13=safe,
-- 14=balcony, 15=jacuzzi, 16=room_service, 17=hair_dryer, 18=coffee_maker
--
-- hotel_provider_room_type IDs are auto-incremented from 12b seed.
-- Seeding a representative sample for flagship hotels.

-- =====================
-- Hotel Altairis Palma (5 stars)
-- hprt: 1=single, 2=double, 3=suite, 4=deluxe
-- =====================
-- Single: basics
INSERT INTO hotel_provider_room_type_amenity (hotel_provider_room_type_id, amenity_id) VALUES
(1, 9), (1, 10), (1, 11), (1, 17);
-- Double: more
INSERT INTO hotel_provider_room_type_amenity (hotel_provider_room_type_id, amenity_id) VALUES
(2, 9), (2, 10), (2, 11), (2, 12), (2, 13), (2, 17);
-- Suite: most
INSERT INTO hotel_provider_room_type_amenity (hotel_provider_room_type_id, amenity_id) VALUES
(3, 9), (3, 10), (3, 11), (3, 12), (3, 13), (3, 14), (3, 15), (3, 16), (3, 17), (3, 18);
-- Deluxe: everything
INSERT INTO hotel_provider_room_type_amenity (hotel_provider_room_type_id, amenity_id) VALUES
(4, 9), (4, 10), (4, 11), (4, 12), (4, 13), (4, 14), (4, 15), (4, 16), (4, 17), (4, 18);

-- =====================
-- Hotel Sol de Palma (3 stars)
-- hprt: 5=single, 6=double, 7=twin
-- =====================
INSERT INTO hotel_provider_room_type_amenity (hotel_provider_room_type_id, amenity_id) VALUES
(5, 9), (5, 10), (5, 11);
INSERT INTO hotel_provider_room_type_amenity (hotel_provider_room_type_id, amenity_id) VALUES
(6, 9), (6, 10), (6, 11), (6, 17);
INSERT INTO hotel_provider_room_type_amenity (hotel_provider_room_type_id, amenity_id) VALUES
(7, 9), (7, 10), (7, 11), (7, 17);
