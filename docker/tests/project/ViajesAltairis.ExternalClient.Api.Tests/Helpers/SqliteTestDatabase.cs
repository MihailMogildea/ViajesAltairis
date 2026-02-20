using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.ExternalClient.Api.Tests.Helpers;

public class SqliteTestDatabase : IDbConnectionFactory, IDisposable
{
    private readonly SqliteConnection _connection;

    public SqliteTestDatabase()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    public IDbConnection CreateConnection() => new TypeCoercingConnection(_connection);

    public SqliteTestDatabase CreateSchema()
    {
        Execute("""
            CREATE TABLE user_type (
                id INTEGER PRIMARY KEY,
                name TEXT NOT NULL
            );

            CREATE TABLE business_partner (
                id INTEGER PRIMARY KEY,
                company_name TEXT NOT NULL,
                enabled INTEGER NOT NULL DEFAULT 1
            );

            CREATE TABLE country (
                id INTEGER PRIMARY KEY,
                name TEXT NOT NULL
            );

            CREATE TABLE administrative_division (
                id INTEGER PRIMARY KEY,
                name TEXT NOT NULL,
                country_id INTEGER NOT NULL,
                FOREIGN KEY (country_id) REFERENCES country(id)
            );

            CREATE TABLE city (
                id INTEGER PRIMARY KEY,
                name TEXT NOT NULL,
                administrative_division_id INTEGER NOT NULL,
                FOREIGN KEY (administrative_division_id) REFERENCES administrative_division(id)
            );

            CREATE TABLE user (
                id INTEGER PRIMARY KEY,
                email TEXT NOT NULL,
                password_hash TEXT NOT NULL,
                enabled INTEGER NOT NULL DEFAULT 1,
                user_type_id INTEGER NOT NULL,
                business_partner_id INTEGER,
                first_name TEXT,
                last_name TEXT,
                FOREIGN KEY (user_type_id) REFERENCES user_type(id),
                FOREIGN KEY (business_partner_id) REFERENCES business_partner(id)
            );

            CREATE TABLE reservation_status (
                id INTEGER PRIMARY KEY,
                name TEXT NOT NULL
            );

            CREATE TABLE currency (
                id INTEGER PRIMARY KEY,
                iso_code TEXT NOT NULL
            );

            CREATE TABLE promo_code (
                id INTEGER PRIMARY KEY,
                code TEXT NOT NULL
            );

            CREATE TABLE reservation (
                id INTEGER PRIMARY KEY,
                reservation_code TEXT NOT NULL,
                status_id INTEGER NOT NULL,
                booked_by_user_id INTEGER NOT NULL,
                currency_id INTEGER NOT NULL,
                owner_first_name TEXT NOT NULL,
                owner_last_name TEXT NOT NULL,
                owner_email TEXT NOT NULL,
                owner_phone TEXT,
                owner_tax_id TEXT,
                subtotal REAL NOT NULL DEFAULT 0,
                tax_amount REAL NOT NULL DEFAULT 0,
                margin_amount REAL NOT NULL DEFAULT 0,
                discount_amount REAL NOT NULL DEFAULT 0,
                total_price REAL NOT NULL DEFAULT 0,
                promo_code_id INTEGER,
                notes TEXT,
                created_at TEXT NOT NULL DEFAULT (datetime('now')),
                updated_at TEXT NOT NULL DEFAULT (datetime('now')),
                FOREIGN KEY (status_id) REFERENCES reservation_status(id),
                FOREIGN KEY (booked_by_user_id) REFERENCES user(id),
                FOREIGN KEY (currency_id) REFERENCES currency(id),
                FOREIGN KEY (promo_code_id) REFERENCES promo_code(id)
            );

            CREATE TABLE hotel (
                id INTEGER PRIMARY KEY,
                name TEXT NOT NULL,
                stars INTEGER NOT NULL DEFAULT 3,
                address TEXT NOT NULL DEFAULT '',
                email TEXT,
                phone TEXT,
                check_in_time TEXT NOT NULL DEFAULT '14:00',
                check_out_time TEXT NOT NULL DEFAULT '11:00',
                latitude REAL,
                longitude REAL,
                margin REAL NOT NULL DEFAULT 0,
                enabled INTEGER NOT NULL DEFAULT 1,
                city_id INTEGER NOT NULL DEFAULT 1,
                FOREIGN KEY (city_id) REFERENCES city(id)
            );

            CREATE TABLE provider (
                id INTEGER PRIMARY KEY,
                name TEXT NOT NULL
            );

            CREATE TABLE hotel_provider (
                id INTEGER PRIMARY KEY,
                hotel_id INTEGER NOT NULL,
                provider_id INTEGER NOT NULL,
                FOREIGN KEY (hotel_id) REFERENCES hotel(id),
                FOREIGN KEY (provider_id) REFERENCES provider(id)
            );

            CREATE TABLE room_type (
                id INTEGER PRIMARY KEY,
                name TEXT NOT NULL
            );

            CREATE TABLE board_type (
                id INTEGER PRIMARY KEY,
                name TEXT NOT NULL
            );

            CREATE TABLE hotel_provider_room_type (
                id INTEGER PRIMARY KEY,
                hotel_provider_id INTEGER NOT NULL,
                room_type_id INTEGER NOT NULL,
                currency_id INTEGER NOT NULL,
                quantity INTEGER NOT NULL,
                price_per_night REAL NOT NULL,
                capacity INTEGER NOT NULL DEFAULT 2,
                enabled INTEGER NOT NULL DEFAULT 1,
                FOREIGN KEY (hotel_provider_id) REFERENCES hotel_provider(id),
                FOREIGN KEY (room_type_id) REFERENCES room_type(id),
                FOREIGN KEY (currency_id) REFERENCES currency(id)
            );

            CREATE TABLE hotel_provider_room_type_board (
                id INTEGER PRIMARY KEY,
                hotel_provider_room_type_id INTEGER NOT NULL,
                board_type_id INTEGER NOT NULL,
                price_per_night REAL NOT NULL DEFAULT 0,
                enabled INTEGER NOT NULL DEFAULT 1,
                FOREIGN KEY (hotel_provider_room_type_id) REFERENCES hotel_provider_room_type(id),
                FOREIGN KEY (board_type_id) REFERENCES board_type(id)
            );

            CREATE TABLE reservation_line (
                id INTEGER PRIMARY KEY,
                reservation_id INTEGER NOT NULL,
                hotel_provider_room_type_id INTEGER NOT NULL,
                board_type_id INTEGER NOT NULL,
                check_in_date TEXT NOT NULL,
                check_out_date TEXT NOT NULL,
                num_rooms INTEGER NOT NULL DEFAULT 1,
                num_guests INTEGER NOT NULL DEFAULT 1,
                price_per_night REAL NOT NULL DEFAULT 0,
                board_price_per_night REAL NOT NULL DEFAULT 0,
                num_nights INTEGER NOT NULL DEFAULT 1,
                subtotal REAL NOT NULL DEFAULT 0,
                tax_amount REAL NOT NULL DEFAULT 0,
                margin_amount REAL NOT NULL DEFAULT 0,
                discount_amount REAL NOT NULL DEFAULT 0,
                total_price REAL NOT NULL DEFAULT 0,
                currency_id INTEGER NOT NULL,
                FOREIGN KEY (reservation_id) REFERENCES reservation(id),
                FOREIGN KEY (hotel_provider_room_type_id) REFERENCES hotel_provider_room_type(id),
                FOREIGN KEY (board_type_id) REFERENCES board_type(id),
                FOREIGN KEY (currency_id) REFERENCES currency(id)
            );

            CREATE TABLE reservation_guest (
                id INTEGER PRIMARY KEY,
                reservation_line_id INTEGER NOT NULL,
                first_name TEXT NOT NULL,
                last_name TEXT NOT NULL,
                email TEXT,
                phone TEXT,
                FOREIGN KEY (reservation_line_id) REFERENCES reservation_line(id)
            );

            CREATE TABLE review (
                id INTEGER PRIMARY KEY,
                hotel_id INTEGER NOT NULL,
                user_id INTEGER NOT NULL,
                reservation_id INTEGER,
                rating INTEGER NOT NULL,
                title TEXT,
                comment TEXT,
                visible INTEGER NOT NULL DEFAULT 1,
                created_at TEXT NOT NULL DEFAULT (datetime('now')),
                FOREIGN KEY (hotel_id) REFERENCES hotel(id),
                FOREIGN KEY (user_id) REFERENCES user(id)
            );

            CREATE TABLE cancellation_policy (
                id INTEGER PRIMARY KEY,
                hotel_id INTEGER NOT NULL,
                free_cancellation_hours INTEGER,
                penalty_percentage REAL,
                enabled INTEGER NOT NULL DEFAULT 1,
                FOREIGN KEY (hotel_id) REFERENCES hotel(id)
            );

            CREATE TABLE amenity_category (
                id INTEGER PRIMARY KEY,
                name TEXT NOT NULL
            );

            CREATE TABLE amenity (
                id INTEGER PRIMARY KEY,
                name TEXT NOT NULL,
                category_id INTEGER NOT NULL,
                FOREIGN KEY (category_id) REFERENCES amenity_category(id)
            );

            CREATE TABLE hotel_amenity (
                id INTEGER PRIMARY KEY,
                hotel_id INTEGER NOT NULL,
                amenity_id INTEGER NOT NULL,
                FOREIGN KEY (hotel_id) REFERENCES hotel(id),
                FOREIGN KEY (amenity_id) REFERENCES amenity(id)
            );
            """);
        return this;
    }

    public SqliteTestDatabase CreateBookingViews()
    {
        Execute("""
            CREATE VIEW v_reservation_summary AS
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

            CREATE VIEW v_reservation_line_detail AS
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

            CREATE VIEW v_reservation_guest_list AS
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
            """);
        return this;
    }

    public SqliteTestDatabase CreateHotelViews()
    {
        Execute("""
            CREATE VIEW v_hotel_card AS
            SELECT
                h.id              AS hotel_id,
                h.name            AS hotel_name,
                h.stars,
                c.id              AS city_id,
                c.name            AS city_name,
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
                     c.id, c.name,
                     ad.id, ad.name,
                     co.id, co.name,
                     cp.free_cancellation_hours,
                     h.enabled;

            CREATE VIEW v_hotel_detail AS
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

            CREATE VIEW v_hotel_room_catalog AS
            SELECT
                hprt.id           AS hotel_provider_room_type_id,
                h.id              AS hotel_id,
                h.name            AS hotel_name,
                p.id              AS provider_id,
                p.name            AS provider_name,
                rt.id             AS room_type_id,
                rt.name           AS room_type_name,
                hprt.capacity,
                hprt.quantity,
                hprt.price_per_night,
                cur.iso_code      AS currency_code,
                hprt.enabled
            FROM hotel_provider_room_type hprt
            JOIN hotel_provider hp     ON hp.id = hprt.hotel_provider_id
            JOIN hotel h               ON h.id = hp.hotel_id
            JOIN provider p            ON p.id = hp.provider_id
            JOIN room_type rt          ON rt.id = hprt.room_type_id
            JOIN currency cur          ON cur.id = hprt.currency_id;

            CREATE VIEW v_room_board_option AS
            SELECT
                hprtb.id          AS hotel_provider_room_type_board_id,
                hprtb.hotel_provider_room_type_id,
                bt.id             AS board_type_id,
                bt.name           AS board_type_name,
                hprtb.price_per_night,
                hprtb.enabled
            FROM hotel_provider_room_type_board hprtb
            JOIN board_type bt         ON bt.id = hprtb.board_type_id;

            CREATE VIEW v_hotel_amenity_list AS
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
            """);
        return this;
    }

    public SqliteTestDatabase SeedReferenceData()
    {
        Execute("""
            INSERT INTO user_type (id, name) VALUES (1, 'client'), (2, 'admin'), (3, 'agent');
            INSERT INTO reservation_status (id, name) VALUES (1, 'draft'), (2, 'pending'), (3, 'confirmed'), (4, 'checked_in'), (5, 'completed'), (6, 'cancelled');
            INSERT INTO currency (id, iso_code) VALUES (1, 'EUR'), (2, 'USD'), (3, 'GBP');
            INSERT INTO board_type (id, name) VALUES (1, 'Room Only'), (2, 'Bed & Breakfast'), (3, 'Half Board'), (4, 'Full Board');
            INSERT INTO room_type (id, name) VALUES (1, 'Standard'), (2, 'Superior'), (3, 'Suite');
            INSERT INTO country (id, name) VALUES (1, 'Spain'), (2, 'France');
            INSERT INTO administrative_division (id, name, country_id) VALUES (1, 'Catalonia', 1), (2, 'Ile-de-France', 2);
            INSERT INTO city (id, name, administrative_division_id) VALUES (1, 'Barcelona', 1), (2, 'Paris', 2);
            """);
        return this;
    }

    public void Execute(string sql)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }
}
