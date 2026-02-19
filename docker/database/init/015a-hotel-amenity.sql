CREATE TABLE hotel_amenity (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    hotel_id BIGINT NOT NULL,
    amenity_id BIGINT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uq_hotel_amenity (hotel_id, amenity_id),
    FOREIGN KEY (hotel_id) REFERENCES hotel(id),
    FOREIGN KEY (amenity_id) REFERENCES amenity(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
