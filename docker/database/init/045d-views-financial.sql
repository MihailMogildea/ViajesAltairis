-- =============================================
-- Financial views
-- =============================================

-- v_invoice_detail: invoice with status, reservation, and partner info
CREATE OR REPLACE VIEW v_invoice_detail AS
SELECT
    i.id              AS invoice_id,
    i.invoice_number,
    ist.id            AS status_id,
    ist.name          AS status_name,
    r.id              AS reservation_id,
    r.reservation_code,
    bp.id             AS business_partner_id,
    bp.company_name   AS business_partner_name,
    i.subtotal,
    i.tax_amount,
    i.discount_amount,
    i.total_amount,
    i.period_start,
    i.period_end,
    i.created_at,
    i.updated_at
FROM invoice i
JOIN invoice_status ist    ON ist.id = i.status_id
JOIN reservation r         ON r.id = i.reservation_id
LEFT JOIN business_partner bp ON bp.id = i.business_partner_id;

-- v_payment_summary: payment with method, reservation, and total fees
CREATE OR REPLACE VIEW v_payment_summary AS
SELECT
    pt.id             AS payment_transaction_id,
    pt.transaction_reference,
    pm.id             AS payment_method_id,
    pm.name           AS payment_method_name,
    r.id              AS reservation_id,
    r.reservation_code,
    pt.amount,
    cur.iso_code      AS currency_code,
    pts.id            AS status_id,
    pts.name          AS status_name,
    pt.created_at,
    (SELECT COALESCE(SUM(ptf.fee_amount), 0)
     FROM payment_transaction_fee ptf
     WHERE ptf.payment_transaction_id = pt.id) AS total_fees
FROM payment_transaction pt
JOIN payment_transaction_status pts ON pts.id = pt.status_id
JOIN payment_method pm     ON pm.id = pt.payment_method_id
JOIN reservation r         ON r.id = pt.reservation_id
JOIN currency cur          ON cur.id = pt.currency_id;

-- v_revenue_by_hotel: total revenue from completed reservations per hotel
CREATE OR REPLACE VIEW v_revenue_by_hotel AS
SELECT
    h.id              AS hotel_id,
    h.name            AS hotel_name,
    c.name            AS city_name,
    co.name           AS country_name,
    COUNT(DISTINCT rl.reservation_id) AS reservation_count,
    SUM(rl.total_price) AS total_revenue,
    cur.iso_code      AS currency_code
FROM reservation_line rl
JOIN reservation r         ON r.id = rl.reservation_id AND r.status_id = 5
JOIN hotel_provider_room_type hprt ON hprt.id = rl.hotel_provider_room_type_id
JOIN hotel_provider hp     ON hp.id = hprt.hotel_provider_id
JOIN hotel h               ON h.id = hp.hotel_id
JOIN city c                ON c.id = h.city_id
JOIN administrative_division ad ON ad.id = c.administrative_division_id
JOIN country co            ON co.id = ad.country_id
JOIN currency cur          ON cur.id = rl.currency_id
GROUP BY h.id, h.name, c.name, co.name, cur.iso_code;

-- v_revenue_by_provider: total revenue from completed reservations per provider
CREATE OR REPLACE VIEW v_revenue_by_provider AS
SELECT
    p.id              AS provider_id,
    p.name            AS provider_name,
    COUNT(DISTINCT rl.reservation_id) AS reservation_count,
    SUM(rl.total_price) AS total_revenue,
    cur.iso_code      AS currency_code
FROM reservation_line rl
JOIN reservation r         ON r.id = rl.reservation_id AND r.status_id = 5
JOIN hotel_provider_room_type hprt ON hprt.id = rl.hotel_provider_room_type_id
JOIN hotel_provider hp     ON hp.id = hprt.hotel_provider_id
JOIN provider p            ON p.id = hp.provider_id
JOIN currency cur          ON cur.id = rl.currency_id
GROUP BY p.id, p.name, cur.iso_code;

-- v_outstanding_balance: unpaid invoices (status = created)
CREATE OR REPLACE VIEW v_outstanding_balance AS
SELECT
    i.id              AS invoice_id,
    i.invoice_number,
    r.id              AS reservation_id,
    r.reservation_code,
    bp.id             AS business_partner_id,
    bp.company_name   AS business_partner_name,
    i.total_amount,
    i.period_start,
    i.period_end,
    i.created_at
FROM invoice i
JOIN reservation r         ON r.id = i.reservation_id
LEFT JOIN business_partner bp ON bp.id = i.business_partner_id
WHERE i.status_id = 1;
