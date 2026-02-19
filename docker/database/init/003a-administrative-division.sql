CREATE TABLE administrative_division (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    country_id BIGINT NOT NULL,
    parent_id BIGINT NULL,
    name VARCHAR(150) NOT NULL,
    type_id BIGINT NOT NULL,
    level TINYINT NOT NULL,
    enabled BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (country_id) REFERENCES country(id),
    FOREIGN KEY (parent_id) REFERENCES administrative_division(id),
    FOREIGN KEY (type_id) REFERENCES administrative_division_type(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
