CREATE TABLE hotel_provider_room_type_board (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    hotel_provider_room_type_id BIGINT NOT NULL,
    board_type_id BIGINT NOT NULL,
    price_per_night DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    enabled BOOLEAN NOT NULL DEFAULT TRUE,
    UNIQUE KEY uq_hprt_board (hotel_provider_room_type_id, board_type_id),
    FOREIGN KEY (hotel_provider_room_type_id) REFERENCES hotel_provider_room_type(id),
    FOREIGN KEY (board_type_id) REFERENCES board_type(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
