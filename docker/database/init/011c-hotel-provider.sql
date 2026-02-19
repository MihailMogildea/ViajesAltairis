CREATE TABLE hotel_provider (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    hotel_id BIGINT NOT NULL,
    provider_id BIGINT NOT NULL,
    enabled BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uq_hotel_provider (hotel_id, provider_id),
    FOREIGN KEY (hotel_id) REFERENCES hotel(id),
    FOREIGN KEY (provider_id) REFERENCES provider(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
