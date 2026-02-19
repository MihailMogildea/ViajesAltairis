-- =============================================
-- User access views
-- =============================================

-- v_user_profile: user with type, language, partner, provider, subscriptions
CREATE OR REPLACE VIEW v_user_profile AS
SELECT
    u.id              AS user_id,
    u.email,
    u.first_name,
    u.last_name,
    u.phone,
    ut.id             AS user_type_id,
    ut.name           AS user_type_name,
    l.id              AS language_id,
    l.iso_code        AS language_code,
    bp.id             AS business_partner_id,
    bp.company_name   AS business_partner_name,
    bp.discount       AS partner_discount,
    p.id              AS provider_id,
    p.name            AS provider_name,
    u.discount        AS user_discount,
    us.id             AS subscription_id,
    st.id             AS subscription_type_id,
    st.name           AS subscription_type_name,
    st.discount       AS subscription_discount,
    us.start_date     AS subscription_start_date,
    us.end_date       AS subscription_end_date,
    COALESCE(bp.discount, 0) + u.discount + COALESCE(st.discount, 0) AS combined_discount,
    u.enabled
FROM user u
JOIN user_type ut          ON ut.id = u.user_type_id
LEFT JOIN language l       ON l.id = u.language_id
LEFT JOIN business_partner bp ON bp.id = u.business_partner_id
LEFT JOIN provider p       ON p.id = u.provider_id
LEFT JOIN user_subscription us ON us.user_id = u.id AND us.active = TRUE
LEFT JOIN subscription_type st ON st.id = us.subscription_type_id;

-- v_user_hotel_access: which hotels a user can manage
CREATE OR REPLACE VIEW v_user_hotel_access AS
SELECT
    uh.id             AS user_hotel_id,
    u.id              AS user_id,
    u.first_name,
    u.last_name,
    u.email,
    h.id              AS hotel_id,
    h.name            AS hotel_name,
    c.name            AS city_name
FROM user_hotel uh
JOIN user u                ON u.id = uh.user_id
JOIN hotel h               ON h.id = uh.hotel_id
JOIN city c                ON c.id = h.city_id;

-- v_user_subscription_status: subscription details with remaining days
CREATE OR REPLACE VIEW v_user_subscription_status AS
SELECT
    us.id             AS user_subscription_id,
    us.user_id,
    u.first_name,
    u.last_name,
    u.email,
    st.id             AS subscription_type_id,
    st.name           AS subscription_type_name,
    st.price_per_month,
    st.discount       AS subscription_discount,
    cur.iso_code      AS currency_code,
    us.start_date,
    us.end_date,
    us.active,
    CASE
        WHEN us.end_date IS NULL THEN NULL
        ELSE DATEDIFF(us.end_date, CURDATE())
    END               AS remaining_days
FROM user_subscription us
JOIN user u                ON u.id = us.user_id
JOIN subscription_type st  ON st.id = us.subscription_type_id
JOIN currency cur          ON cur.id = st.currency_id;
