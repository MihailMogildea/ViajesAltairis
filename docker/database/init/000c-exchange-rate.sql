CREATE TABLE exchange_rate (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    currency_id BIGINT NOT NULL,
    rate_to_eur DECIMAL(18, 6) NOT NULL,
    valid_from DATETIME NOT NULL,
    valid_to DATETIME NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (currency_id) REFERENCES currency(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
