INSERT INTO job_schedule (job_key, name, cron_expression) VALUES
('exchange-rate-sync',   'Exchange Rate Sync (ECB)', '15 16 * * 1-5'),
('subscription-billing', 'Subscription Billing',     '0 2 * * *'),
('provider-update',      'Provider Update',          '0 4 * * *');
