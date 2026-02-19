# ViajesAltairis Database

MariaDB 11 database for the ViajesAltairis hotel reservation platform (B2B/B2C).

## Docker

```bash
docker compose up database
```

The database container builds from `Dockerfile`, which copies all SQL scripts from `init/` into `/docker-entrypoint-initdb.d/`. MariaDB executes them **alphabetically** on first startup.

Credentials are defined in the root `.env` file.

## File Naming Convention

```
NNNx-table-name.sql
```

- `NNN` = 3-digit number for execution order (000-045)
- `x` = letter suffix for sub-ordering within the same group:
  - `a` = table definition
  - `b` = seed data
  - `c`, `d`, `e`, `f` = additional tables/seeds in the same group

Translation seeds use an extra suffix: `006b-translation-seed-{a-w}.sql` (one file per entity type).

## Schema Overview

### Foundation (000-007)

| File | Table | Purpose |
|------|-------|---------|
| 000a | `currency` | ISO 4217 currencies (EUR, GBP) |
| 000c | `exchange_rate` | Currency conversion rates to EUR, time-bound |
| 001a | `country` | Countries with default currency, enabled flag |
| 002a | `administrative_division_type` | Division type lookup (autonomous_community, island, province, region, department) |
| 003a | `administrative_division` | Self-referencing geographic hierarchy with type and level |
| 004a | `city` | Cities linked to administrative divisions |
| 005a | `language` | Supported languages (en, es) |
| 006a | `translation` | Decoupled translation system: entity_type + entity_id + field + language_id |
| 007a | `web_translation` | UI text translations (key-value per language) |

### Providers & Hotels (008-020)

| File | Table | Purpose |
|------|-------|---------|
| 008a | `provider_type` | internal, external |
| 009a | `provider` | Provider companies with API credentials (encrypted), margin percentage |
| 010a | `hotel` | Hotels with city, stars, location, check-in/out times, margin |
| 011a | `room_type` | Room categories: single, double, twin, suite, junior_suite, deluxe |
| 011c | `hotel_provider` | Many-to-many: which provider manages which hotel |
| 012a | `hotel_provider_room_type` | Room configuration per hotel-provider: capacity, quantity, price, currency |
| 013a | `amenity_category` | hotel, room |
| 014a | `amenity` | WiFi, pool, minibar, etc. |
| 015a | `hotel_amenity` | Hotel-level amenity assignments |
| 016a | `hotel_provider_room_type_amenity` | Room-level amenity assignments |
| 017a | `tax_type` | vat, tourist_tax, city_tax |
| 018a | `tax` | Tax rules with geographic scoping (country/division/city level) |
| 019a | `hotel_image` | Hotel photos with sort order |
| 020a | `room_image` | Room photos linked to hotel_provider_room_type |

### Users & Access (021-024)

| File | Table | Purpose |
|------|-------|---------|
| 021a | `user_type` | admin, manager, agent, hotel_staff, client |
| 023a | `business_partner` | B2B companies with discount percentage |
| 024a | `user` | All users with type, credentials, personal discount, optional provider/partner link |
| 024c | `user_hotel` | Junction for hotel_staff access to specific hotels |

### Booking Flow (025-027)

| File | Table | Purpose |
|------|-------|---------|
| 025a | `reservation_status` | draft, pending, confirmed, checked_in, completed, cancelled |
| 025c | `promo_code` | Discount codes with validity period and usage limits |
| 025e | `board_type` | room_only, bed_and_breakfast, half_board, full_board, all_inclusive |
| 026a | `reservation` | Booking header: owner snapshot, totals, currency, promo code |
| 026b | `reservation_line` | Line items: room type, board, dates, num_rooms, per-line pricing |
| 027a | `reservation_guest` | Guest details per reservation line (per room) |

### Financials (029-032)

| File | Table | Purpose |
|------|-------|---------|
| 029a | `invoice_status` | created, paid, refunded |
| 029c | `payment_method` | visa, mastercard, bank_transfer, paypal (with min_days_before_checkin) |
| 030a | `invoice` | Invoice linked to reservation, with period for B2B monthly billing |
| 031a | `payment_transaction` | Payment records with transaction reference |
| 032a | `payment_transaction_fee` | Processing fees per transaction |

### Operations (033-043)

| File | Table | Purpose |
|------|-------|---------|
| 033a | `cancellation_policy` | Per hotel: free cancellation hours, penalty percentage |
| 034a | `audit_log` | Change tracking: entity_type, action, old/new values (JSON) |
| 035a | `seasonal_margin` | Regional margin by date range (MM-DD), stacks on provider+hotel margins |
| 036a | `subscription_type` | Monthly plans: basic (3%), premium (7%), vip (12%) discount |
| 036c | `user_subscription` | Active subscriptions per user |
| 037a | `hotel_provider_room_type_board` | Board type pricing per room type |
| 039a | `hotel_blackout` | Hotel closure periods (renovation, private events) |
| 040a | `cancellation` | Actual cancellation records: penalty, refund amounts |
| 041a | `email_template` | Template keys for transactional emails |
| 042a | `notification_log` | Sent email log per user |
| 043a | `review` | Post-stay reviews: rating 1-5, title, comment |
| 043b | `review_response` | Hotel staff response to reviews |

### Indexes (044)

| File | Purpose |
|------|---------|
| 044a | Performance indexes across all tables |

### Views (045)

| File | Views | Purpose |
|------|-------|---------|
| 045a | `v_hotel_card`, `v_hotel_detail`, `v_hotel_room_catalog`, `v_room_board_option`, `v_hotel_amenity_list`, `v_room_amenity_list`, `v_hotel_review_detail` | Hotel display: listings, detail pages, room catalog, amenities, reviews |
| 045b | `v_room_availability`, `v_reservation_summary`, `v_reservation_line_detail`, `v_reservation_guest_list`, `v_active_promo_code` | Booking flow: availability, reservation details, guests, promo codes |
| 045c | `v_user_profile`, `v_user_hotel_access`, `v_user_subscription_status` | User access: profiles with combined discounts, hotel access by city, subscriptions |
| 045d | `v_invoice_detail`, `v_payment_summary`, `v_revenue_by_hotel`, `v_revenue_by_provider`, `v_outstanding_balance` | Financial: invoices, payments, revenue reports, outstanding balances |
| 045e | `v_cancellation_detail`, `v_hotel_blackout_calendar`, `v_seasonal_margin_calendar`, `v_notification_history`, `v_audit_trail` | Operations: cancellations, blackouts, seasonal margins, notifications, audit |
| 045f | `v_applicable_tax`, `v_hotel_margin_stack`, `v_exchange_rate_current` | Tax & margin: tax rules, combined margin stack, current exchange rates |

## Key Design Decisions

### Translation System
Decoupled pattern using `entity_type + entity_id + field + language_id`. Supports multiple translatable fields per entity (e.g., hotel name vs summary). Also used for email template subjects and bodies.

### Margin Stack
Final price includes three additive margins:
1. **Provider margin** - base profit (e.g., 15% for Mallorca provider)
2. **Hotel margin** - per-hotel adjustment (premium positioning, renovations)
3. **Seasonal margin** - regional, by date range (e.g., Mallorca +15% Jun-Sep)

### Discount Stack
Applied at booking time, all additive:
1. **Business partner discount** - from `business_partner.discount`
2. **User discount** - from `user.discount` (admin-granted)
3. **Subscription discount** - from active `subscription_type.discount`
4. **Promo code** - one-time code on reservation

### Reservation Architecture
- **Header** (`reservation`) - owner snapshot, currency, exchange rate, totals summary
- **Lines** (`reservation_line`) - one per room booking, can have different hotels/dates/board types
- **Guests** (`reservation_guest`) - per line, for hotel check-in
- **Draft flow** - cart/basket creates draft reservation + lines, reducing availability

### Currency Handling
- Base currency: EUR
- Prices stored in provider currency (EUR for internal)
- Exchange rate snapshot frozen at booking time on reservation
- Invoices in client currency, EUR conversion at export using reservation exchange rate

### Owner Snapshot
Reservation stores a copy of owner details (name, email, address, tax_id) at booking time. This ensures stable invoice records even if the user updates their profile later.

## Seed Data Summary

| Entity | Count |
|--------|-------|
| Countries | 3 (Spain, France, Great Britain) |
| Administrative divisions | 8 (Balearic Islands hierarchy, Cataluña, PACA) |
| Cities | 32 (10 per island + Barcelona + Nice) |
| Languages | 2 (English, Spanish) |
| Translation entries | ~300+ across 23 seed files |
| Providers | 5 internal (Mallorca, Menorca, Ibiza, Peninsula, France) |
| Hotels | 22 (10 Palma, 1 Alcudia, 1 Soller, 2 Menorca, 2 Ibiza, 3 Barcelona, 3 Nice) |
| Room types | 6 categories, 69 hotel-room configurations |
| Board types | 5 (room only through all inclusive) |
| Board pricing | All 69 room types with star-appropriate options |
| Hotel images | 66 (4 per 5-star, 3 per 4-star, 2 per 3-star) |
| Room images | 153 (3 per suite/deluxe, 2 per other types) |
| Amenities | 18 (8 hotel-level, 10 room-level) |
| Tax rules | 4 (Spain VAT, Balearic tourist tax, France VAT, Nice city tax) |
| Users | 12 (1 admin, 1 manager, 2 agents, 3 hotel staff, 2 clients, 3 B2B agents) |
| Business partners | 2 (Viajes Sol 8%, Mediterranean Tours 5%) |
| Subscription types | 3 (basic, premium, vip) |
| Promo codes | 3 (WELCOME10, SUMMER25, VIP2026) |
| Cancellation policies | 22 (one per hotel, scaled by stars) |
| Seasonal margins | 5 (Mallorca, Menorca, Ibiza, Barcelona, Alpes-Maritimes) |
| Reservations | 10 (all Palma hotels, completed, with full line/guest data) |
| Reviews | 10 with responses (ratings 1-5, all responded by regional manager) |
| Email templates | 7 (booking, cancellation, payment, invoice, discount, subscription x2) |

## All IDs are BIGINT

All primary keys and foreign keys use `BIGINT` for high-traffic readiness.

## Password Hashing

Seed users use bcrypt factor 10: `$2a$10$q87spkM7XAW.Oce4GEDLu.1HFgbFeWlqafwYsrYXM6nGbe3USGZ/O` (password: `password123`).

Provider API passwords use AES encryption with the `ENCRYPTION_KEY` from `.env`.
