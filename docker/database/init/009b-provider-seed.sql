-- Internal providers (currency_id=1 EUR by default)
INSERT INTO provider (id, type_id, name, margin) VALUES
(1, 1, 'Altairis Hotels Mallorca', 15.00),
(2, 1, 'Altairis Hotels Menorca', 15.00),
(3, 1, 'Altairis Hotels Ibiza', 15.00),
(4, 1, 'Altairis Hotels Peninsula', 12.00),
(5, 1, 'Altairis Hotels France', 10.00);

-- External providers
INSERT INTO provider (id, type_id, currency_id, name, api_url, api_username, margin) VALUES
(6, 2, 1, 'HotelBeds', 'https://api.hotelbeds.com/hotel-api/1.0', 'demo_user', 8.00),
(7, 2, 2, 'BookingDotCom', 'https://distribution-xml.booking.com/2.0', 'demo_user', 10.00),
(8, 2, 1, 'TravelGate', 'https://api.travelgatex.com', 'demo_user', 7.00);
