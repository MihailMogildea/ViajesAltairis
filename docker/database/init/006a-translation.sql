CREATE TABLE translation (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    entity_type VARCHAR(50) NOT NULL,
    entity_id BIGINT NOT NULL,
    field VARCHAR(50) NOT NULL DEFAULT 'name',
    language_id BIGINT NOT NULL,
    value TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uq_translation (entity_type, entity_id, field, language_id),
    FOREIGN KEY (language_id) REFERENCES language(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
