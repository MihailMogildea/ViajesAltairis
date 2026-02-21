CREATE TABLE city (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    administrative_division_id BIGINT NOT NULL,
    name VARCHAR(150) NOT NULL,
    image_url VARCHAR(500) NULL,
    enabled BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (administrative_division_id) REFERENCES administrative_division(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
