CREATE TABLE hotel_provider_room_type (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    hotel_provider_id BIGINT NOT NULL,
    room_type_id BIGINT NOT NULL,
    capacity TINYINT NOT NULL,
    quantity INT NOT NULL,
    price_per_night DECIMAL(10, 2) NOT NULL,
    currency_id BIGINT NOT NULL,
    exchange_rate_id BIGINT NOT NULL,
    enabled BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uq_hotel_provider_room_type (hotel_provider_id, room_type_id),
    FOREIGN KEY (hotel_provider_id) REFERENCES hotel_provider(id),
    FOREIGN KEY (room_type_id) REFERENCES room_type(id),
    FOREIGN KEY (currency_id) REFERENCES currency(id),
    FOREIGN KEY (exchange_rate_id) REFERENCES exchange_rate(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
