CREATE TABLE tax (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    tax_type_id BIGINT NOT NULL,
    country_id BIGINT NULL,
    administrative_division_id BIGINT NULL,
    city_id BIGINT NULL,
    rate DECIMAL(10, 4) NOT NULL,
    is_percentage BOOLEAN NOT NULL DEFAULT TRUE,
    enabled BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (tax_type_id) REFERENCES tax_type(id),
    FOREIGN KEY (country_id) REFERENCES country(id),
    FOREIGN KEY (administrative_division_id) REFERENCES administrative_division(id),
    FOREIGN KEY (city_id) REFERENCES city(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
