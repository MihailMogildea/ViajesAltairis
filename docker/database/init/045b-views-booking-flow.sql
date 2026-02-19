-- =============================================
-- Booking flow views
-- =============================================

-- v_room_availability: available rooms per hotel_provider_room_type
CREATE OR REPLACE VIEW v_room_availability AS
SELECT
    hprt.id           AS hotel_provider_room_type_id,
    h.id              AS hotel_id,
    h.name            AS hotel_name,
    rt.name           AS room_type_name,
    p.name            AS provider_name,
    hprt.quantity     AS total_rooms,
    hprt.price_per_night,
    cur.iso_code      AS currency_code,
    rl.check_in_date,
    rl.check_out_date,
    COALESCE(SUM(rl.num_rooms), 0) AS booked_rooms,
    hprt.quantity - COALESCE(SUM(rl.num_rooms), 0) AS available_rooms
FROM hotel_provider_room_type hprt
JOIN hotel_provider hp     ON hp.id = hprt.hotel_provider_id
JOIN hotel h               ON h.id = hp.hotel_id
JOIN provider p            ON p.id = hp.provider_id
JOIN room_type rt          ON rt.id = hprt.room_type_id
JOIN currency cur          ON cur.id = hprt.currency_id
LEFT JOIN reservation_line rl ON rl.hotel_provider_room_type_id = hprt.id
    AND rl.reservation_id IN (
        SELECT res.id FROM reservation res WHERE res.status_id BETWEEN 1 AND 4
    )
GROUP BY hprt.id, h.id, h.name, rt.name, p.name,
         hprt.quantity, hprt.price_per_night, cur.iso_code,
         rl.check_in_date, rl.check_out_date;

-- v_reservation_summary: reservation header overview
CREATE OR REPLACE VIEW v_reservation_summary AS
SELECT
    r.id              AS reservation_id,
    r.reservation_code,
    rs.id             AS status_id,
    rs.name           AS status_name,
    r.booked_by_user_id,
    ub.first_name     AS booked_by_first_name,
    ub.last_name      AS booked_by_last_name,
    r.owner_first_name,
    r.owner_last_name,
    r.owner_email,
    r.owner_phone,
    r.owner_tax_id,
    r.subtotal,
    r.tax_amount,
    r.margin_amount,
    r.discount_amount,
    r.total_price,
    cur.iso_code      AS currency_code,
    r.promo_code_id,
    pc.code           AS promo_code,
    r.notes,
    r.created_at,
    r.updated_at,
    (SELECT COUNT(*) FROM reservation_line rl2 WHERE rl2.reservation_id = r.id) AS line_count
FROM reservation r
JOIN reservation_status rs ON rs.id = r.status_id
JOIN user ub               ON ub.id = r.booked_by_user_id
JOIN currency cur          ON cur.id = r.currency_id
LEFT JOIN promo_code pc    ON pc.id = r.promo_code_id;

-- v_reservation_line_detail: reservation line with hotel/room info
CREATE OR REPLACE VIEW v_reservation_line_detail AS
SELECT
    rl.id             AS reservation_line_id,
    rl.reservation_id,
    r.reservation_code,
    h.id              AS hotel_id,
    h.name            AS hotel_name,
    rt.id             AS room_type_id,
    rt.name           AS room_type_name,
    bt.id             AS board_type_id,
    bt.name           AS board_type_name,
    p.id              AS provider_id,
    p.name            AS provider_name,
    rl.check_in_date,
    rl.check_out_date,
    rl.num_rooms,
    rl.num_guests,
    rl.price_per_night,
    rl.board_price_per_night,
    rl.num_nights,
    rl.subtotal,
    rl.tax_amount,
    rl.margin_amount,
    rl.discount_amount,
    rl.total_price,
    cur.iso_code      AS currency_code
FROM reservation_line rl
JOIN reservation r         ON r.id = rl.reservation_id
JOIN hotel_provider_room_type hprt ON hprt.id = rl.hotel_provider_room_type_id
JOIN hotel_provider hp     ON hp.id = hprt.hotel_provider_id
JOIN hotel h               ON h.id = hp.hotel_id
JOIN provider p            ON p.id = hp.provider_id
JOIN room_type rt          ON rt.id = hprt.room_type_id
JOIN board_type bt         ON bt.id = rl.board_type_id
JOIN currency cur          ON cur.id = rl.currency_id;

-- v_reservation_guest_list: guests with their reservation line context
CREATE OR REPLACE VIEW v_reservation_guest_list AS
SELECT
    rg.id             AS guest_id,
    rg.reservation_line_id,
    rl.reservation_id,
    r.reservation_code,
    rg.first_name,
    rg.last_name,
    rg.email,
    rg.phone,
    h.id              AS hotel_id,
    h.name            AS hotel_name,
    rt.name           AS room_type_name
FROM reservation_guest rg
JOIN reservation_line rl   ON rl.id = rg.reservation_line_id
JOIN reservation r         ON r.id = rl.reservation_id
JOIN hotel_provider_room_type hprt ON hprt.id = rl.hotel_provider_room_type_id
JOIN hotel_provider hp     ON hp.id = hprt.hotel_provider_id
JOIN hotel h               ON h.id = hp.hotel_id
JOIN room_type rt          ON rt.id = hprt.room_type_id;

-- v_active_promo_code: currently valid and usable promo codes
CREATE OR REPLACE VIEW v_active_promo_code AS
SELECT
    pc.id             AS promo_code_id,
    pc.code,
    pc.discount_percentage,
    pc.discount_amount,
    cur.iso_code      AS currency_code,
    pc.valid_from,
    pc.valid_to,
    pc.max_uses,
    pc.current_uses
FROM promo_code pc
LEFT JOIN currency cur     ON cur.id = pc.currency_id
WHERE pc.enabled = TRUE
  AND CURDATE() BETWEEN pc.valid_from AND pc.valid_to
  AND (pc.max_uses IS NULL OR pc.current_uses < pc.max_uses);
