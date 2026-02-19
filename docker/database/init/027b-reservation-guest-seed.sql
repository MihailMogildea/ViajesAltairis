-- Guests for Juan's reservation (line 1, double room - 2 guests)
INSERT INTO reservation_guest (reservation_line_id, first_name, last_name, email, phone) VALUES
(1, 'Juan', 'Martínez', 'client1@example.com', '+34 600 000 005'),
(1, 'Laura', 'Martínez', 'laura.martinez@example.com', '+34 600 000 015');

-- Guest for Emma's reservation (line 2, single room - 1 guest)
INSERT INTO reservation_guest (reservation_line_id, first_name, last_name, email, phone) VALUES
(2, 'Emma', 'Wilson', 'client2@example.com', '+44 700 000 006');

-- Guests for Juan's reservation at Playa de Palma (line 3, double room - 2 guests)
INSERT INTO reservation_guest (reservation_line_id, first_name, last_name, email, phone) VALUES
(3, 'Juan', 'Martínez', 'client1@example.com', '+34 600 000 005'),
(3, 'Laura', 'Martínez', 'laura.martinez@example.com', '+34 600 000 015');

-- Guests for Emma's reservation at Casco Antiguo (line 4, twin room - 2 guests)
INSERT INTO reservation_guest (reservation_line_id, first_name, last_name, email, phone) VALUES
(4, 'Emma', 'Wilson', 'client2@example.com', '+44 700 000 006'),
(4, 'Sarah', 'Brown', 'sarah.brown@example.com', '+44 700 000 020');

-- Guests for Roberto's reservation at Bellver Park (line 5, suite - 2 guests)
INSERT INTO reservation_guest (reservation_line_id, first_name, last_name, email, phone) VALUES
(5, 'Roberto', 'García', 'roberto.garcia@email.com', '+34 655 123 456'),
(5, 'María', 'García', 'maria.garcia@email.com', '+34 655 123 457');

-- Guests for Juan's reservation at Marina Palma (line 6, suite - 2 guests)
INSERT INTO reservation_guest (reservation_line_id, first_name, last_name, email, phone) VALUES
(6, 'Juan', 'Martínez', 'client1@example.com', '+34 600 000 005'),
(6, 'Laura', 'Martínez', 'laura.martinez@example.com', '+34 600 000 015');

-- Guest for Oliver's reservation at Catedral Palma (line 7, double room - 1 guest)
INSERT INTO reservation_guest (reservation_line_id, first_name, last_name, email, phone) VALUES
(7, 'Oliver', 'Smith', 'oliver.smith@email.co.uk', '+44 777 888 999');

-- Guests for Carmen's reservation at Portixol (line 8, double room - 2 guests)
INSERT INTO reservation_guest (reservation_line_id, first_name, last_name, email, phone) VALUES
(8, 'Carmen', 'Ruiz', 'carmen.ruiz@email.com', '+34 666 555 444'),
(8, 'Diego', 'Ruiz', 'diego.ruiz@email.com', '+34 666 555 445');

-- Guest for Emma's reservation at Santa Catalina (line 9, jr_suite - 1 guest)
INSERT INTO reservation_guest (reservation_line_id, first_name, last_name, email, phone) VALUES
(9, 'Emma', 'Wilson', 'client2@example.com', '+44 700 000 006');

-- Guests for Friedrich's reservation at Son Vida Palace (line 10, deluxe - 2 guests)
INSERT INTO reservation_guest (reservation_line_id, first_name, last_name, email, phone) VALUES
(10, 'Friedrich', 'Weber', 'friedrich.weber@email.de', '+49 170 123 4567'),
(10, 'Helga', 'Weber', 'helga.weber@email.de', '+49 170 123 4568');
