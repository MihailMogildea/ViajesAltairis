CREATE TABLE review (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    reservation_id BIGINT NOT NULL,
    user_id BIGINT NULL,
    hotel_id BIGINT NOT NULL,
    rating TINYINT NOT NULL,
    title VARCHAR(200) NULL,
    comment TEXT NULL,
    visible BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY uq_review_reservation (reservation_id),
    FOREIGN KEY (reservation_id) REFERENCES reservation(id),
    FOREIGN KEY (user_id) REFERENCES user(id),
    FOREIGN KEY (hotel_id) REFERENCES hotel(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
