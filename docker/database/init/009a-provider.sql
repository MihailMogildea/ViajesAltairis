CREATE TABLE provider (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    type_id BIGINT NOT NULL,
    name VARCHAR(150) NOT NULL,
    api_url VARCHAR(500) NULL,
    api_username VARCHAR(150) NULL,
    api_password_encrypted VARCHAR(500) NULL,
    margin DECIMAL(5, 2) NOT NULL DEFAULT 0.00,
    enabled BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (type_id) REFERENCES provider_type(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
