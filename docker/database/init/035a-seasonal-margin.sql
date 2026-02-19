CREATE TABLE seasonal_margin (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    administrative_division_id BIGINT NOT NULL,
    start_month_day CHAR(5) NOT NULL,
    end_month_day CHAR(5) NOT NULL,
    margin DECIMAL(5, 2) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (administrative_division_id) REFERENCES administrative_division(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
