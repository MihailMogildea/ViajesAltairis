CREATE TABLE payment_transaction (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    reservation_id BIGINT NOT NULL,
    payment_method_id BIGINT NOT NULL,
    transaction_reference VARCHAR(255) NOT NULL,
    amount DECIMAL(10, 2) NOT NULL,
    currency_id BIGINT NOT NULL,
    exchange_rate_id BIGINT NOT NULL,
    status VARCHAR(50) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (reservation_id) REFERENCES reservation(id),
    FOREIGN KEY (payment_method_id) REFERENCES payment_method(id),
    FOREIGN KEY (currency_id) REFERENCES currency(id),
    FOREIGN KEY (exchange_rate_id) REFERENCES exchange_rate(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
