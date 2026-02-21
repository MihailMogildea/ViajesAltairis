-- Good review: Juan loved Hotel Altairis Palma (checkout Jan 15)
INSERT INTO review (id, reservation_id, user_id, hotel_id, rating, title, comment, created_at) VALUES
(1, 1, 8, 1, 5, 'Exceptional stay, will come back!', 'From the moment we arrived, the staff made us feel welcome. The double room was spacious and beautifully decorated with stunning views of the bay. Breakfast was outstanding with a great selection of local and international options. The location is perfect, walking distance to the old town and the beach. Highly recommended for couples looking for a luxury getaway in Palma.', '2026-01-16 10:00:00');

-- Hotel response to good review
INSERT INTO review_response (id, review_id, user_id, comment, created_at) VALUES
(1, 1, 6, 'Dear Juan, thank you so much for your wonderful review! We are delighted to hear that you and Laura enjoyed your stay with us. It was a pleasure to have you, and we look forward to welcoming you back to Hotel Altairis Palma soon. Warm regards, Marta Serra, Hotel Director.', '2026-01-17 11:00:00');

-- Bad review: Emma had issues at Hotel Sol de Palma (checkout Feb 4)
INSERT INTO review (id, reservation_id, user_id, hotel_id, rating, title, comment, created_at) VALUES
(2, 2, 9, 2, 2, 'Disappointing experience, not as advertised', 'The room was much smaller than expected and the air conditioning was not working properly during my stay. I reported it to reception twice but it was never fixed. The bathroom needed updating and the shower pressure was very low. The location is decent but for the price paid I expected much better. The only positive was the friendly reception staff who tried their best to help.', '2026-02-05 14:00:00');

-- Hotel response to bad review
INSERT INTO review_response (id, review_id, user_id, comment, created_at) VALUES
(2, 2, 5, 'Dear Emma, we sincerely apologise for the inconvenience during your stay. We take all feedback seriously and have already addressed the air conditioning issue and scheduled bathroom renovations. We would love the opportunity to make it up to you on a future visit. Please contact us directly for a special arrangement. Kind regards, Antoni Barceló, Regional Manager.', '2026-02-06 09:00:00');

-- Good review: Juan enjoyed Hotel Playa de Palma (checkout Feb 19)
INSERT INTO review (id, reservation_id, user_id, hotel_id, rating, title, comment, created_at) VALUES
(3, 3, 8, 3, 4, 'Great beach hotel', 'Hotel Playa de Palma has an excellent location right on the beachfront with easy access to shops and restaurants. The double room was comfortable and the pool area was really enjoyable, especially in the afternoons. Half board dinner options were varied and well prepared. The only downside was some noise from nearby bars in the evening, which made it hard to sleep with the window open. Overall a very solid choice for a beach holiday.', '2026-02-20 12:00:00');

-- Hotel response to Playa de Palma review
INSERT INTO review_response (id, review_id, user_id, comment, created_at) VALUES
(3, 3, 5, 'Dear Juan, thank you for your kind review of Hotel Playa de Palma. We are glad you enjoyed the beach location and our pool facilities. We apologise for the noise inconvenience and are looking into soundproofing options for the rooms facing the entertainment area. We hope to welcome you back soon. Kind regards, Antoni Barceló, Regional Manager.', '2026-02-21 09:00:00');

-- Average review: Emma at Hotel Casco Antiguo (checkout Jan 23)
INSERT INTO review (id, reservation_id, user_id, hotel_id, rating, title, comment, created_at) VALUES
(4, 4, 9, 4, 3, 'Decent budget option', 'The Casco Antiguo hotel benefits from a fantastic location right in the heart of the old town, within walking distance of the cathedral and main attractions. The twin room was basic but clean and had everything we needed for a short stay. Breakfast was average with a limited selection of continental items and could use more variety. For the price point it is a fair deal, but do not expect luxury. Good enough for budget-conscious travellers who want a central base.', '2026-01-24 15:00:00');

-- Hotel response to Casco Antiguo review
INSERT INTO review_response (id, review_id, user_id, comment, created_at) VALUES
(4, 4, 5, 'Dear Emma, thank you for staying with us at Hotel Casco Antiguo and for your honest feedback. We are pleased you appreciated our central location. We have taken your comments about the breakfast on board and are working on expanding the menu with more hot options and fresh pastries. We hope you will give us another chance on your next visit to Palma. Kind regards, Antoni Barceló, Regional Manager.', '2026-01-25 10:00:00');

-- Excellent review: Roberto at Hotel Bellver Park (checkout Dec 29)
INSERT INTO review (id, reservation_id, user_id, hotel_id, rating, title, comment, created_at) VALUES
(5, 5, NULL, 5, 5, 'Perfect Christmas getaway', 'We spent a wonderful Christmas week at Hotel Bellver Park and it exceeded all our expectations. The suite had amazing views of Bellver Castle and the surrounding parkland, which was magical during the festive season. The restaurant served outstanding food with a special Christmas Eve dinner that was truly memorable. The staff were incredibly family friendly and organised activities for all ages. We cannot recommend this hotel enough for anyone looking for a special holiday experience in Palma.', '2025-12-30 11:00:00');

-- Hotel response to Bellver Park review
INSERT INTO review_response (id, review_id, user_id, comment, created_at) VALUES
(5, 5, 5, 'Dear Roberto, what a wonderful review! We are thrilled that you and María had such a special Christmas at Hotel Bellver Park. Our team works very hard to create a festive atmosphere and it is rewarding to know it was appreciated. We would love to welcome your family back for another holiday celebration. Kind regards, Antoni Barceló, Regional Manager.', '2025-12-31 14:00:00');

-- Good review: Juan at Hotel Marina Palma (checkout Mar 4)
INSERT INTO review (id, reservation_id, user_id, hotel_id, rating, title, comment, created_at) VALUES
(6, 6, 8, 6, 4, 'Luxury with minor flaws', 'Hotel Marina Palma is a stunning five-star property with breathtaking views over the marina and the Mediterranean. The suite was beautifully appointed and the spa facilities were world class, easily the best massage I have had in years. The all-inclusive dining was excellent with a wide variety of international and local cuisine. The only issue was one evening when room service took over an hour to arrive, which was disappointing given the premium price. Otherwise a truly luxurious experience.', '2026-02-08 16:00:00');

-- Hotel response to Marina Palma review
INSERT INTO review_response (id, review_id, user_id, comment, created_at) VALUES
(6, 6, 5, 'Dear Juan, thank you for your generous review of Hotel Marina Palma. We are delighted you enjoyed the marina views and our spa. We sincerely apologise for the delay with room service that evening and have reviewed our staffing during peak hours to ensure this does not happen again. We look forward to welcoming you and Laura back for another luxurious stay. Kind regards, Antoni Barceló, Regional Manager.', '2026-02-09 10:00:00');

-- Average review: Oliver at Hotel Catedral Palma (checkout Jan 10)
INSERT INTO review (id, reservation_id, user_id, hotel_id, rating, title, comment, created_at) VALUES
(7, 7, NULL, 7, 3, 'Good location, average hotel', 'The views of Palma Cathedral from my room were absolutely beautiful and worth the stay alone. The location is perfect for exploring the historic centre on foot. However, the room itself felt a bit dated with worn furniture and an old bathroom that could use a renovation. Breakfast was good quality with fresh local produce but the selection was somewhat limited compared to other four-star hotels I have stayed in. A decent hotel carried largely by its prime location.', '2026-01-11 14:30:00');

-- Hotel response to Catedral Palma review
INSERT INTO review_response (id, review_id, user_id, comment, created_at) VALUES
(7, 7, 5, 'Dear Oliver, thank you for your feedback on Hotel Catedral Palma. We are glad the cathedral views lived up to expectations. We appreciate your honest comments about the room furnishings and are pleased to share that a full renovation programme is scheduled for the coming months. We hope you will return to see the improvements. Kind regards, Antoni Barceló, Regional Manager.', '2026-01-12 09:00:00');

-- Terrible review: Carmen at Hotel Portixol (checkout Feb 12)
INSERT INTO review (id, reservation_id, user_id, hotel_id, rating, title, comment, created_at) VALUES
(8, 8, NULL, 8, 1, 'Terrible experience', 'This was by far the worst hotel stay I have ever had. Our double room faced the main street and the noise was unbearable throughout the night with traffic and people shouting. The bathroom was not properly cleaned when we arrived and I found hair in the bathtub which was disgusting. To make matters worse, the night shift receptionist was extremely rude when I tried to raise these issues. My husband and I checked out early and I would absolutely not recommend this hotel to anyone.', '2026-02-13 09:00:00');

-- Hotel response to Portixol review (apologetic)
INSERT INTO review_response (id, review_id, user_id, comment, created_at) VALUES
(8, 8, 5, 'Dear Carmen, we are deeply sorry to read about your experience at Hotel Portixol. This falls far below the standards we expect and we take your complaints very seriously. We have launched an immediate investigation into the cleanliness protocols and the conduct of our night shift staff. We would very much like to make amends and invite you to contact us directly so we can discuss appropriate compensation. Please accept our sincerest apologies. Kind regards, Antoni Barceló, Regional Manager.', '2026-02-14 10:00:00');

-- Good review: Emma at Hotel Santa Catalina (checkout Mar 14)
INSERT INTO review (id, reservation_id, user_id, hotel_id, rating, title, comment, created_at) VALUES
(9, 9, 9, 9, 4, 'Lovely neighbourhood hotel', 'Hotel Santa Catalina is perfectly situated in the trendy Santa Catalina neighbourhood with fantastic restaurants, cafés, and boutiques just steps from the door. The junior suite was modern and very comfortable with a lovely balcony overlooking the rooftops. The half board dinner was genuinely good with a rotating menu that featured fresh local ingredients. The only reason I am not giving five stars is that the hotel could benefit from a small gym or wellness area. A great choice for anyone who wants to experience the real Palma.', '2026-02-15 11:00:00');

-- Hotel response to Santa Catalina review
INSERT INTO review_response (id, review_id, user_id, comment, created_at) VALUES
(9, 9, 5, 'Dear Emma, thank you so much for your lovely review of Hotel Santa Catalina. We are thrilled you enjoyed the neighbourhood and found the junior suite to your liking. Your suggestion about a wellness area is noted and something we are actively exploring as part of our expansion plans. We hope to welcome you back to Santa Catalina soon. Kind regards, Antoni Barceló, Regional Manager.', '2026-02-16 09:00:00');

-- Excellent review: Friedrich at Hotel Son Vida Palace (checkout Jan 31)
INSERT INTO review (id, reservation_id, user_id, hotel_id, rating, title, comment, created_at) VALUES
(10, 10, NULL, 10, 5, 'Best hotel in Mallorca', 'Hotel Son Vida Palace is without question the finest hotel on the island. The palatial estate and grounds are magnificent, set on a hilltop with panoramic views over the bay of Palma. The deluxe room was extraordinary with every luxury imaginable. The world-class golf course is beautifully maintained and the Michelin-quality dining was an experience in itself, with every meal surpassing the last. The service was impeccable throughout our six-night stay, with staff who anticipated our every need. Worth every cent and we are already planning our return.', '2026-02-01 17:00:00');

-- Hotel response to Son Vida Palace review
INSERT INTO review_response (id, review_id, user_id, comment, created_at) VALUES
(10, 10, 5, 'Dear Friedrich, what an honour to receive such a glowing review. We are truly delighted that you and Helga experienced the very best of Hotel Son Vida Palace. Our team takes enormous pride in delivering exceptional service and your words are a wonderful testament to their dedication. We very much look forward to welcoming you back for another unforgettable stay. Kind regards, Antoni Barceló, Regional Manager.', '2026-02-02 10:00:00');
