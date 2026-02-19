CREATE TABLE hotel_provider_room_type_amenity (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    hotel_provider_room_type_id BIGINT NOT NULL,
    amenity_id BIGINT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uq_hp_room_type_amenity (hotel_provider_room_type_id, amenity_id),
    FOREIGN KEY (hotel_provider_room_type_id) REFERENCES hotel_provider_room_type(id),
    FOREIGN KEY (amenity_id) REFERENCES amenity(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
