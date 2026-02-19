CREATE TABLE payment_transaction_fee (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    payment_transaction_id BIGINT NOT NULL,
    fee_type VARCHAR(50) NOT NULL,
    fee_amount DECIMAL(10, 2) NOT NULL,
    fee_percentage DECIMAL(5, 4) NULL,
    fixed_fee_amount DECIMAL(10, 2) NULL,
    currency_id BIGINT NOT NULL,
    description VARCHAR(500) NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (payment_transaction_id) REFERENCES payment_transaction(id),
    FOREIGN KEY (currency_id) REFERENCES currency(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
