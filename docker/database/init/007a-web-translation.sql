CREATE TABLE web_translation (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    translation_key VARCHAR(150) NOT NULL,
    language_id BIGINT NOT NULL,
    value TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uq_web_translation (translation_key, language_id),
    FOREIGN KEY (language_id) REFERENCES language(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
