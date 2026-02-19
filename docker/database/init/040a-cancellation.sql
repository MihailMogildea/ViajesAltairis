CREATE TABLE cancellation (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    reservation_id BIGINT NOT NULL,
    cancelled_by_user_id BIGINT NOT NULL,
    reason TEXT NULL,
    penalty_percentage DECIMAL(5, 2) NOT NULL DEFAULT 0.00,
    penalty_amount DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    refund_amount DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    currency_id BIGINT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (reservation_id) REFERENCES reservation(id),
    FOREIGN KEY (cancelled_by_user_id) REFERENCES user(id),
    FOREIGN KEY (currency_id) REFERENCES currency(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
