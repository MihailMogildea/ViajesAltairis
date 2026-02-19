CREATE TABLE business_partner (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    company_name VARCHAR(200) NOT NULL,
    tax_id VARCHAR(50) NULL,
    discount DECIMAL(5, 2) NOT NULL DEFAULT 0.00,
    address VARCHAR(300) NOT NULL,
    city VARCHAR(100) NOT NULL,
    postal_code VARCHAR(20) NULL,
    country VARCHAR(100) NOT NULL,
    contact_email VARCHAR(200) NOT NULL,
    contact_phone VARCHAR(50) NULL,
    enabled BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
