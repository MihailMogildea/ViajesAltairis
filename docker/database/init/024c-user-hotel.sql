CREATE TABLE user_hotel (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    user_id BIGINT NOT NULL,
    hotel_id BIGINT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uq_user_hotel (user_id, hotel_id),
    FOREIGN KEY (user_id) REFERENCES user(id),
    FOREIGN KEY (hotel_id) REFERENCES hotel(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
