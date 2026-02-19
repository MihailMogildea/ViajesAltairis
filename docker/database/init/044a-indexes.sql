-- =====================
-- Performance Indexes
-- =====================
-- FK, PK, and UNIQUE constraints already create indexes automatically.
-- These are additional indexes for common query patterns.

-- =====================
-- Hotel Search & Filtering
-- =====================
CREATE INDEX idx_hotel_city_stars_enabled ON hotel (city_id, stars, enabled);
CREATE INDEX idx_hotel_stars ON hotel (stars);
CREATE INDEX idx_hotel_coordinates ON hotel (latitude, longitude);

-- =====================
-- Room Availability (critical for booking flow)
-- =====================
CREATE INDEX idx_reservation_line_availability ON reservation_line (hotel_provider_room_type_id, check_in_date, check_out_date);
CREATE INDEX idx_hprt_hotel_provider_enabled ON hotel_provider_room_type (hotel_provider_id, enabled);
CREATE INDEX idx_hotel_blackout_dates ON hotel_blackout (hotel_id, start_date, end_date);

-- =====================
-- Reservation Queries
-- =====================
CREATE INDEX idx_reservation_status_created ON reservation (status_id, created_at DESC);
CREATE INDEX idx_reservation_booked_by ON reservation (booked_by_user_id, created_at DESC);
CREATE INDEX idx_reservation_owner ON reservation (owner_user_id, created_at DESC);

-- =====================
-- User & Authentication
-- =====================
CREATE INDEX idx_user_type_enabled ON user (user_type_id, enabled);
CREATE INDEX idx_user_provider_enabled ON user (provider_id, enabled);
CREATE INDEX idx_user_business_partner_enabled ON user (business_partner_id, enabled);

-- =====================
-- Subscription Lookup
-- =====================
CREATE INDEX idx_user_subscription_active ON user_subscription (user_id, active);

-- =====================
-- Financial Queries
-- =====================
CREATE INDEX idx_invoice_status_created ON invoice (status_id, created_at DESC);
CREATE INDEX idx_invoice_business_partner_period ON invoice (business_partner_id, period_start DESC);
CREATE INDEX idx_payment_transaction_status ON payment_transaction (status);
CREATE INDEX idx_payment_transaction_reference ON payment_transaction (transaction_reference);
CREATE INDEX idx_payment_transaction_created ON payment_transaction (created_at DESC);

-- =====================
-- Exchange Rate Lookup
-- =====================
CREATE INDEX idx_exchange_rate_currency_validity ON exchange_rate (currency_id, valid_from, valid_to);

-- =====================
-- Tax Resolution (hierarchical geographic lookup)
-- =====================
CREATE INDEX idx_tax_geographic ON tax (country_id, administrative_division_id, city_id, enabled);

-- =====================
-- Seasonal Margin Lookup
-- =====================
CREATE INDEX idx_seasonal_margin_lookup ON seasonal_margin (administrative_division_id, start_month_day, end_month_day);

-- =====================
-- Promo Code Validation
-- =====================
CREATE INDEX idx_promo_code_validation ON promo_code (enabled, valid_from, valid_to);

-- =====================
-- Translation Lookup
-- =====================
CREATE INDEX idx_translation_entity ON translation (entity_type, entity_id);

-- =====================
-- Image Ordering
-- =====================
CREATE INDEX idx_hotel_image_sort ON hotel_image (hotel_id, sort_order);
CREATE INDEX idx_room_image_sort ON room_image (hotel_provider_room_type_id, sort_order);

-- =====================
-- Audit Trail
-- =====================
CREATE INDEX idx_audit_log_entity ON audit_log (entity_type, entity_id, created_at DESC);
CREATE INDEX idx_audit_log_created ON audit_log (created_at DESC);

-- =====================
-- Notification History
-- =====================
CREATE INDEX idx_notification_log_user ON notification_log (user_id, created_at DESC);

-- =====================
-- Review Display
-- =====================
CREATE INDEX idx_review_hotel_rating ON review (hotel_id, rating);

-- =====================
-- Guest Lookup
-- =====================
CREATE INDEX idx_reservation_guest_email ON reservation_guest (email);

-- =====================
-- Cancellation History
-- =====================
CREATE INDEX idx_cancellation_created ON cancellation (created_at DESC);
