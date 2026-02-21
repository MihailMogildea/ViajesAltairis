-- Hotel image seeds for all 22 hotels
-- Using picsum.photos for hotel-themed placeholder images
-- 5★ hotels: 4 images, 4★ hotels: 3 images, 3★ hotels: 2 images

-- Hotel Altairis Palma (id=1)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(1, 'https://picsum.photos/seed/hotel-1-exterior/800/600', 'Luxury hotel exterior with elegant facade', 1),
(1, 'https://picsum.photos/seed/hotel-1-lobby/800/600', 'Spacious luxury hotel lobby', 2),
(1, 'https://picsum.photos/seed/hotel-1-pool/800/600', 'Infinity pool with sea view', 3),
(1, 'https://picsum.photos/seed/hotel-1-spa/800/600', 'Luxury spa area', 4);

-- Hotel Sol de Palma (id=2)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(2, 'https://picsum.photos/seed/hotel-2-exterior/800/600', 'Mediterranean style hotel exterior', 1),
(2, 'https://picsum.photos/seed/hotel-2-lobby/800/600', 'Welcoming hotel lobby', 2);

-- Hotel Playa de Palma (id=3)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(3, 'https://picsum.photos/seed/hotel-3-exterior/800/600', 'Beach hotel exterior with ocean backdrop', 1),
(3, 'https://picsum.photos/seed/hotel-3-pool/800/600', 'Swimming pool surrounded by palm trees', 2),
(3, 'https://picsum.photos/seed/hotel-3-beach/800/600', 'Stunning beach view from the hotel', 3);

-- Hotel Casco Antiguo (id=4)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(4, 'https://picsum.photos/seed/hotel-4-exterior/800/600', 'Charming old town hotel exterior', 1),
(4, 'https://picsum.photos/seed/hotel-4-patio/800/600', 'Traditional inner patio', 2);

-- Hotel Bellver Park (id=5)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(5, 'https://picsum.photos/seed/hotel-5-exterior/800/600', 'Hotel exterior with park views', 1),
(5, 'https://picsum.photos/seed/hotel-5-garden/800/600', 'Lush hotel garden', 2),
(5, 'https://picsum.photos/seed/hotel-5-restaurant/800/600', 'On-site restaurant with garden setting', 3);

-- Hotel Marina Palma (id=6)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(6, 'https://picsum.photos/seed/hotel-6-exterior/800/600', 'Luxury marina hotel exterior', 1),
(6, 'https://picsum.photos/seed/hotel-6-rooftop-pool/800/600', 'Rooftop pool with panoramic views', 2),
(6, 'https://picsum.photos/seed/hotel-6-spa/800/600', 'Premium spa and wellness area', 3),
(6, 'https://picsum.photos/seed/hotel-6-marina/800/600', 'Marina view from the hotel terrace', 4);

-- Hotel Catedral Palma (id=7)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(7, 'https://picsum.photos/seed/hotel-7-exterior/800/600', 'Hotel exterior with cathedral views', 1),
(7, 'https://picsum.photos/seed/hotel-7-terrace/800/600', 'Terrace dining area with scenic backdrop', 2),
(7, 'https://picsum.photos/seed/hotel-7-courtyard/800/600', 'Peaceful hotel courtyard', 3);

-- Hotel Portixol (id=8)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(8, 'https://picsum.photos/seed/hotel-8-exterior/800/600', 'Hotel exterior overlooking the port', 1),
(8, 'https://picsum.photos/seed/hotel-8-room/800/600', 'Comfortable and simple guest room', 2);

-- Hotel Santa Catalina (id=9)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(9, 'https://picsum.photos/seed/hotel-9-exterior/800/600', 'Modern boutique hotel exterior', 1),
(9, 'https://picsum.photos/seed/hotel-9-rooftop/800/600', 'Rooftop terrace with city views', 2),
(9, 'https://picsum.photos/seed/hotel-9-bar/800/600', 'Stylish hotel bar', 3);

-- Hotel Son Vida Palace (id=10)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(10, 'https://picsum.photos/seed/hotel-10-exterior/800/600', 'Grand palace hotel exterior', 1),
(10, 'https://picsum.photos/seed/hotel-10-golf/800/600', 'Manicured golf course with mountain backdrop', 2),
(10, 'https://picsum.photos/seed/hotel-10-restaurant/800/600', 'Elegant fine dining restaurant', 3),
(10, 'https://picsum.photos/seed/hotel-10-pool/800/600', 'Palace swimming pool and sun terrace', 4);

-- Hotel Bahía Alcúdia (id=11)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(11, 'https://picsum.photos/seed/hotel-11-exterior/800/600', 'Hotel exterior with bay views', 1),
(11, 'https://picsum.photos/seed/hotel-11-pool/800/600', 'Family-friendly swimming pool', 2),
(11, 'https://picsum.photos/seed/hotel-11-beach/800/600', 'Direct beach access from the hotel', 3);

-- Hotel Serra de Tramuntana (id=12)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(12, 'https://picsum.photos/seed/hotel-12-exterior/800/600', 'Mountain hotel nestled in the Tramuntana range', 1),
(12, 'https://picsum.photos/seed/hotel-12-hiking/800/600', 'Scenic hiking views from the hotel', 2),
(12, 'https://picsum.photos/seed/hotel-12-restaurant/800/600', 'Rustic restaurant with local cuisine', 3);

-- Hotel Port Mahón (id=13)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(13, 'https://picsum.photos/seed/hotel-13-exterior/800/600', 'Hotel exterior overlooking Port Mahón', 1),
(13, 'https://picsum.photos/seed/hotel-13-terrace/800/600', 'Sea-facing terrace with dining', 2),
(13, 'https://picsum.photos/seed/hotel-13-garden/800/600', 'Mediterranean garden in Menorca', 3);

-- Hotel Ciutadella Mar (id=14)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(14, 'https://picsum.photos/seed/hotel-14-exterior/800/600', 'Hotel exterior in Ciutadella', 1),
(14, 'https://picsum.photos/seed/hotel-14-terrace/800/600', 'Simple terrace with harbour views', 2);

-- Hotel Dalt Vila (id=15)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(15, 'https://picsum.photos/seed/hotel-15-exterior/800/600', 'Boutique hotel exterior in Dalt Vila', 1),
(15, 'https://picsum.photos/seed/hotel-15-sunset/800/600', 'Terrace with breathtaking sunset views', 2),
(15, 'https://picsum.photos/seed/hotel-15-restaurant/800/600', 'Luxury on-site restaurant', 3),
(15, 'https://picsum.photos/seed/hotel-15-panoramic/800/600', 'Panoramic view of Dalt Vila old town', 4);

-- Hotel Santa Eulària Beach (id=16)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(16, 'https://picsum.photos/seed/hotel-16-exterior/800/600', 'Beachfront hotel exterior in Santa Eulària', 1),
(16, 'https://picsum.photos/seed/hotel-16-pool/800/600', 'Tropical swimming pool area', 2),
(16, 'https://picsum.photos/seed/hotel-16-beach-bar/800/600', 'Relaxing beach bar', 3);

-- Hotel Altairis Barcelona (id=17)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(17, 'https://picsum.photos/seed/hotel-17-exterior/800/600', 'Luxury hotel exterior in Barcelona', 1),
(17, 'https://picsum.photos/seed/hotel-17-lobby/800/600', 'Designer hotel lobby', 2),
(17, 'https://picsum.photos/seed/hotel-17-rooftop/800/600', 'Rooftop terrace with Barcelona skyline', 3),
(17, 'https://picsum.photos/seed/hotel-17-spa/800/600', 'Modern spa and wellness centre', 4);

-- Hotel Gótico (id=18)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(18, 'https://picsum.photos/seed/hotel-18-exterior/800/600', 'Hotel exterior in the Gothic Quarter', 1),
(18, 'https://picsum.photos/seed/hotel-18-interior/800/600', 'Cozy interior with historic charm', 2);

-- Hotel Barceloneta Mar (id=19)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(19, 'https://picsum.photos/seed/hotel-19-exterior/800/600', 'Hotel exterior near Barceloneta beach', 1),
(19, 'https://picsum.photos/seed/hotel-19-terrace/800/600', 'Terrace with Mediterranean sea view', 2),
(19, 'https://picsum.photos/seed/hotel-19-lobby/800/600', 'Modern and bright hotel lobby', 3);

-- Hotel Promenade Nice (id=20)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(20, 'https://picsum.photos/seed/hotel-20-exterior/800/600', 'Elegant hotel on the Promenade des Anglais', 1),
(20, 'https://picsum.photos/seed/hotel-20-room/800/600', 'Luxury room with sea view', 2),
(20, 'https://picsum.photos/seed/hotel-20-pool/800/600', 'Riviera-style swimming pool', 3),
(20, 'https://picsum.photos/seed/hotel-20-restaurant/800/600', 'French fine dining restaurant', 4);

-- Hotel Vieux Nice (id=21)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(21, 'https://picsum.photos/seed/hotel-21-exterior/800/600', 'Charming hotel exterior in Vieux Nice', 1),
(21, 'https://picsum.photos/seed/hotel-21-balcony/800/600', 'Room balcony overlooking old town streets', 2);

-- Hotel Colline de Cimiez (id=22)
INSERT INTO hotel_image (hotel_id, url, alt_text, sort_order) VALUES
(22, 'https://picsum.photos/seed/hotel-22-exterior/800/600', 'Hotel exterior on the Cimiez hill', 1),
(22, 'https://picsum.photos/seed/hotel-22-garden/800/600', 'French-style hotel garden', 2),
(22, 'https://picsum.photos/seed/hotel-22-terrace/800/600', 'Terrace with views over Nice', 3);
