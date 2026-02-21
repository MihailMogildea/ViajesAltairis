-- =============================================
-- Hotel display views
-- =============================================

-- v_hotel_card: hotel summary for listing cards
CREATE OR REPLACE VIEW v_hotel_card AS
SELECT
    h.id              AS hotel_id,
    h.name            AS hotel_name,
    h.stars,
    c.id              AS city_id,
    c.name            AS city_name,
    c.image_url       AS city_image_url,
    ad.id             AS admin_division_id,
    ad.name           AS admin_division_name,
    co.id             AS country_id,
    co.name           AS country_name,
    AVG(r.rating)     AS avg_rating,
    COUNT(r.id)       AS review_count,
    cp.free_cancellation_hours,
    h.enabled
FROM hotel h
JOIN city c                ON c.id = h.city_id
JOIN administrative_division ad ON ad.id = c.administrative_division_id
JOIN country co            ON co.id = ad.country_id
LEFT JOIN review r         ON r.hotel_id = h.id AND r.visible = TRUE
LEFT JOIN cancellation_policy cp ON cp.id = (
    SELECT cp2.id FROM cancellation_policy cp2
    WHERE cp2.hotel_id = h.id AND cp2.enabled = TRUE
    ORDER BY cp2.id LIMIT 1
)
GROUP BY h.id, h.name, h.stars,
         c.id, c.name, c.image_url,
         ad.id, ad.name,
         co.id, co.name,
         cp.free_cancellation_hours,
         h.enabled;

-- v_hotel_detail: extended hotel info for detail page
CREATE OR REPLACE VIEW v_hotel_detail AS
SELECT
    h.id              AS hotel_id,
    h.name            AS hotel_name,
    h.stars,
    h.address,
    h.email,
    h.phone,
    h.check_in_time,
    h.check_out_time,
    h.latitude,
    h.longitude,
    h.margin          AS hotel_margin,
    h.enabled,
    c.id              AS city_id,
    c.name            AS city_name,
    ad.id             AS admin_division_id,
    ad.name           AS admin_division_name,
    co.id             AS country_id,
    co.name           AS country_name,
    AVG(r.rating)     AS avg_rating,
    COUNT(r.id)       AS review_count,
    cp.free_cancellation_hours,
    cp.penalty_percentage
FROM hotel h
JOIN city c                ON c.id = h.city_id
JOIN administrative_division ad ON ad.id = c.administrative_division_id
JOIN country co            ON co.id = ad.country_id
LEFT JOIN review r         ON r.hotel_id = h.id AND r.visible = TRUE
LEFT JOIN cancellation_policy cp ON cp.id = (
    SELECT cp2.id FROM cancellation_policy cp2
    WHERE cp2.hotel_id = h.id AND cp2.enabled = TRUE
    ORDER BY cp2.id LIMIT 1
)
GROUP BY h.id, h.name, h.stars, h.address, h.email, h.phone,
         h.check_in_time, h.check_out_time, h.latitude, h.longitude,
         h.margin, h.enabled,
         c.id, c.name,
         ad.id, ad.name,
         co.id, co.name,
         cp.free_cancellation_hours, cp.penalty_percentage;

-- v_hotel_room_catalog: room types available per hotel
CREATE OR REPLACE VIEW v_hotel_room_catalog AS
SELECT
    hprt.id           AS hotel_provider_room_type_id,
    h.id              AS hotel_id,
    h.name            AS hotel_name,
    p.id              AS provider_id,
    p.name            AS provider_name,
    p.type_id         AS provider_type_id,
    rt.id             AS room_type_id,
    rt.name           AS room_type_name,
    hprt.capacity,
    hprt.quantity,
    hprt.price_per_night,
    p.margin          AS provider_margin,
    h.margin          AS hotel_margin,
    cur.iso_code      AS currency_code,
    hprt.enabled
FROM hotel_provider_room_type hprt
JOIN hotel_provider hp     ON hp.id = hprt.hotel_provider_id
JOIN hotel h               ON h.id = hp.hotel_id
JOIN provider p            ON p.id = hp.provider_id
JOIN room_type rt          ON rt.id = hprt.room_type_id
JOIN currency cur          ON cur.id = hprt.currency_id;

-- v_room_board_option: board types per room
CREATE OR REPLACE VIEW v_room_board_option AS
SELECT
    hprtb.id          AS hotel_provider_room_type_board_id,
    hprtb.hotel_provider_room_type_id,
    bt.id             AS board_type_id,
    bt.name           AS board_type_name,
    hprtb.price_per_night,
    hprtb.enabled
FROM hotel_provider_room_type_board hprtb
JOIN board_type bt         ON bt.id = hprtb.board_type_id;

-- v_hotel_amenity_list: hotel-level amenities
CREATE OR REPLACE VIEW v_hotel_amenity_list AS
SELECT
    ha.id             AS hotel_amenity_id,
    ha.hotel_id,
    a.id              AS amenity_id,
    a.name            AS amenity_name,
    ac.id             AS amenity_category_id,
    ac.name           AS amenity_category_name
FROM hotel_amenity ha
JOIN amenity a             ON a.id = ha.amenity_id
JOIN amenity_category ac   ON ac.id = a.category_id;

-- v_room_amenity_list: room-level amenities
CREATE OR REPLACE VIEW v_room_amenity_list AS
SELECT
    hprta.id          AS room_amenity_id,
    hprta.hotel_provider_room_type_id,
    a.id              AS amenity_id,
    a.name            AS amenity_name,
    ac.id             AS amenity_category_id,
    ac.name           AS amenity_category_name
FROM hotel_provider_room_type_amenity hprta
JOIN amenity a             ON a.id = hprta.amenity_id
JOIN amenity_category ac   ON ac.id = a.category_id;

-- v_hotel_review_detail: reviews with reviewer and response info
CREATE OR REPLACE VIEW v_hotel_review_detail AS
SELECT
    rv.id             AS review_id,
    rv.hotel_id,
    h.name            AS hotel_name,
    rv.reservation_id,
    rv.user_id        AS reviewer_user_id,
    u.first_name      AS reviewer_first_name,
    u.last_name       AS reviewer_last_name,
    rv.rating,
    rv.title,
    rv.comment,
    rv.visible,
    rv.created_at     AS review_created_at,
    rr.id             AS response_id,
    rr.comment        AS response_comment,
    ru.id             AS responder_user_id,
    ru.first_name     AS responder_first_name,
    ru.last_name      AS responder_last_name,
    rr.created_at     AS response_created_at
FROM review rv
JOIN hotel h               ON h.id = rv.hotel_id
JOIN user u                ON u.id = rv.user_id
LEFT JOIN review_response rr ON rr.review_id = rv.id
LEFT JOIN user ru          ON ru.id = rr.user_id;
