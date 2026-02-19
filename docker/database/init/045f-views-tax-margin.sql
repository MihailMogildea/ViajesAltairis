-- =============================================
-- Tax and margin views
-- =============================================

-- v_applicable_tax: taxes with type and geographic scope
CREATE OR REPLACE VIEW v_applicable_tax AS
SELECT
    t.id              AS tax_id,
    tt.id             AS tax_type_id,
    tt.name           AS tax_type_name,
    co.id             AS country_id,
    co.name           AS country_name,
    ad.id             AS admin_division_id,
    ad.name           AS admin_division_name,
    c.id              AS city_id,
    c.name            AS city_name,
    t.rate,
    t.is_percentage,
    t.enabled
FROM tax t
JOIN tax_type tt           ON tt.id = t.tax_type_id
LEFT JOIN country co       ON co.id = t.country_id
LEFT JOIN administrative_division ad ON ad.id = t.administrative_division_id
LEFT JOIN city c           ON c.id = t.city_id;

-- v_hotel_margin_stack: combined margins for a hotel (provider + hotel + seasonal)
CREATE OR REPLACE VIEW v_hotel_margin_stack AS
SELECT
    h.id              AS hotel_id,
    h.name            AS hotel_name,
    p.id              AS provider_id,
    p.name            AS provider_name,
    p.margin          AS provider_margin,
    h.margin          AS hotel_margin,
    sm.id             AS seasonal_margin_id,
    sm.start_month_day,
    sm.end_month_day,
    sm.margin         AS seasonal_margin,
    p.margin + h.margin + COALESCE(sm.margin, 0) AS total_margin
FROM hotel h
JOIN city c                ON c.id = h.city_id
JOIN administrative_division ad ON ad.id = c.administrative_division_id
JOIN hotel_provider hp     ON hp.hotel_id = h.id
JOIN provider p            ON p.id = hp.provider_id
LEFT JOIN seasonal_margin sm ON sm.administrative_division_id = ad.id;

-- v_exchange_rate_current: currently valid exchange rates
CREATE OR REPLACE VIEW v_exchange_rate_current AS
SELECT
    er.id             AS exchange_rate_id,
    cur.id            AS currency_id,
    cur.iso_code      AS currency_code,
    cur.name          AS currency_name,
    cur.symbol        AS currency_symbol,
    er.rate_to_eur,
    er.valid_from,
    er.valid_to
FROM exchange_rate er
JOIN currency cur          ON cur.id = er.currency_id
WHERE CURDATE() BETWEEN DATE(er.valid_from) AND DATE(er.valid_to);
