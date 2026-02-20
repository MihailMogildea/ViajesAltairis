ALTER TABLE provider
    ADD COLUMN sync_status VARCHAR(20) NULL AFTER enabled,
    ADD COLUMN last_synced_at TIMESTAMP NULL AFTER sync_status;
