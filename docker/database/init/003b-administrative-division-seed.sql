-- Spain - Autonomous Communities (level 1)
INSERT INTO administrative_division (id, country_id, parent_id, name, type_id, level) VALUES
(1, 1, NULL, 'Islas Baleares', 1, 1),
(2, 1, NULL, 'Cataluña', 1, 1);

-- Spain - Islands / Provinces (level 2)
INSERT INTO administrative_division (id, country_id, parent_id, name, type_id, level) VALUES
(3, 1, 1, 'Mallorca', 2, 2),
(4, 1, 1, 'Menorca', 2, 2),
(5, 1, 1, 'Ibiza', 2, 2),
(6, 1, 2, 'Barcelona', 3, 2);

-- France - Région (level 1)
INSERT INTO administrative_division (id, country_id, parent_id, name, type_id, level) VALUES
(7, 2, NULL, 'Provence-Alpes-Côte d''Azur', 4, 1);

-- France - Département (level 2)
INSERT INTO administrative_division (id, country_id, parent_id, name, type_id, level) VALUES
(8, 2, 7, 'Alpes-Maritimes', 5, 2);
