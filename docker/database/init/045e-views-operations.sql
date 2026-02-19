-- =============================================
-- Operations views
-- =============================================

-- v_cancellation_detail: cancellation with reservation and user info
CREATE OR REPLACE VIEW v_cancellation_detail AS
SELECT
    cn.id             AS cancellation_id,
    cn.reservation_id,
    r.reservation_code,
    cn.cancelled_by_user_id,
    u.first_name      AS cancelled_by_first_name,
    u.last_name       AS cancelled_by_last_name,
    cn.reason,
    cn.penalty_percentage,
    cn.penalty_amount,
    cn.refund_amount,
    cur.iso_code      AS currency_code,
    cn.created_at
FROM cancellation cn
JOIN reservation r         ON r.id = cn.reservation_id
JOIN user u                ON u.id = cn.cancelled_by_user_id
JOIN currency cur          ON cur.id = cn.currency_id;

-- v_hotel_blackout_calendar: blackout periods per hotel
CREATE OR REPLACE VIEW v_hotel_blackout_calendar AS
SELECT
    hb.id             AS blackout_id,
    h.id              AS hotel_id,
    h.name            AS hotel_name,
    c.name            AS city_name,
    hb.start_date,
    hb.end_date,
    hb.reason
FROM hotel_blackout hb
JOIN hotel h               ON h.id = hb.hotel_id
JOIN city c                ON c.id = h.city_id;

-- v_seasonal_margin_calendar: seasonal margins by region
CREATE OR REPLACE VIEW v_seasonal_margin_calendar AS
SELECT
    sm.id             AS seasonal_margin_id,
    ad.id             AS admin_division_id,
    ad.name           AS admin_division_name,
    co.id             AS country_id,
    co.name           AS country_name,
    sm.start_month_day,
    sm.end_month_day,
    sm.margin
FROM seasonal_margin sm
JOIN administrative_division ad ON ad.id = sm.administrative_division_id
JOIN country co            ON co.id = ad.country_id;

-- v_notification_history: sent notifications with user and template info
CREATE OR REPLACE VIEW v_notification_history AS
SELECT
    nl.id             AS notification_id,
    nl.user_id,
    u.first_name,
    u.last_name,
    et.id             AS email_template_id,
    et.name           AS email_template_name,
    nl.recipient_email,
    nl.subject,
    nl.created_at
FROM notification_log nl
JOIN user u                ON u.id = nl.user_id
JOIN email_template et     ON et.id = nl.email_template_id;

-- v_audit_trail: audit log with optional user info
CREATE OR REPLACE VIEW v_audit_trail AS
SELECT
    al.id             AS audit_log_id,
    al.user_id,
    u.first_name      AS user_first_name,
    u.last_name       AS user_last_name,
    al.entity_type,
    al.entity_id,
    al.action,
    al.old_values,
    al.new_values,
    al.created_at
FROM audit_log al
LEFT JOIN user u           ON u.id = al.user_id;
