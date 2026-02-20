# Client-Web

Public-facing hotel booking website for ViajesAltairis. Built with Next.js 15 (App Router), React 19, Tailwind CSS 4, and TypeScript.

Runs on port **3002** in Docker. Communicates exclusively with **client-api** (port 5002).

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | Next.js 15 (App Router, standalone output) |
| UI | React 19 + Tailwind CSS 4 |
| Language | TypeScript 5.7 |
| Metrics | prom-client (Prometheus `/api/metrics`) |

No database access — all data flows through the client-api REST endpoints.

## Project Structure

```
src/
├── app/                        # Next.js App Router pages
│   ├── page.tsx                # Home — hero, destinations, top-rated hotels
│   ├── hotels/
│   │   └── [id]/page.tsx       # Hotel detail — images, rooms, reviews, cancellation policy
│   ├── hotels/page.tsx         # Hotel search — filters, sorting, pagination
│   ├── booking/
│   │   ├── page.tsx            # Checkout — review, promo codes, payment, guest info
│   │   └── confirmation/page.tsx
│   ├── login/page.tsx          # Login — demo accounts, forgot-password link
│   ├── register/page.tsx       # Registration
│   ├── forgot-password/page.tsx # Request password reset
│   ├── reset-password/page.tsx  # Set new password (token from URL)
│   ├── reservations/page.tsx   # Reservation list — expandable detail, cancel, write review
│   ├── profile/page.tsx        # View/edit profile, change password, country dropdown
│   ├── invoices/page.tsx       # Invoice list — expandable detail with subtotal/tax breakdown
│   ├── subscriptions/page.tsx  # Subscription plans — view, compare, subscribe
│   ├── api/metrics/route.ts    # Prometheus metrics endpoint
│   ├── layout.tsx              # Root layout — providers, header, footer
│   └── globals.css             # Tailwind imports
│
├── components/                 # Shared UI components
│   ├── Header.tsx              # Nav bar — hotels, reservations, profile, invoices, subscriptions
│   ├── Footer.tsx              # Site footer with links and contact info
│   ├── SearchBar.tsx           # Destination + dates + guests search form
│   ├── FilterSidebar.tsx       # Stars, price range, amenities filters
│   ├── HotelCard.tsx           # Hotel summary card for search results
│   ├── RoomCard.tsx            # Room availability card with board options and add-to-basket
│   ├── BookingBasket.tsx       # Sidebar basket summary with remove buttons
│   ├── ImageCarousel.tsx       # Image gallery with navigation
│   ├── ReviewCard.tsx          # Review display with optional hotel response
│   ├── StarRating.tsx          # Star display (1-5)
│   ├── AmenityBadge.tsx        # Amenity icon + label badge
│   ├── DestinationCard.tsx     # Destination card for home page
│   └── Pagination.tsx          # Page navigation controls
│
├── context/                    # React Context providers
│   ├── AuthContext.tsx          # Authentication — login, register, logout, JWT token management
│   ├── BookingContext.tsx       # Basket — add/remove items, promo codes, checkout submission
│   └── LocaleContext.tsx       # i18n — language toggle, currency selector, translation lookup
│
├── data/                       # Static fallback data (used when API is unavailable)
│   ├── hotels.ts               # 22 hotels with images, amenities, ratings
│   ├── rooms.ts                # 69 room configurations
│   ├── reviews.ts              # Sample reviews
│   ├── amenities.ts            # Hotel and room amenities
│   ├── areas.ts                # Regions, cities, countries
│   └── users.ts                # Demo user accounts
│
├── lib/                        # Utilities and API client
│   ├── api.ts                  # 29 API functions — all client-api endpoints
│   ├── translations-fallback.ts # Hardcoded EN/ES translations (~180 keys each)
│   ├── locale.ts               # Locale constants (en, es)
│   └── utils.ts                # formatPrice, formatDate, calculateNights, filtering, sorting
│
└── types/
    └── index.ts                # All TypeScript interfaces (domain + API DTOs)
```

## Pages

| Route | Auth | Description |
|-------|------|-------------|
| `/` | Public | Home page — search bar, popular destinations, top-rated hotels |
| `/hotels` | Public | Hotel search with filters, sorting, and pagination |
| `/hotels/[id]` | Public | Hotel detail — gallery, amenities, cancellation policy, rooms, reviews |
| `/login` | Public | Login with demo accounts, forgot-password link |
| `/register` | Public | Account creation |
| `/forgot-password` | Public | Request password reset email |
| `/reset-password?token=` | Public | Set new password via reset token |
| `/booking` | Login required | Checkout flow — review basket, promo codes, guest info, payment |
| `/booking/confirmation` | Login required | Post-booking confirmation with reservation code |
| `/reservations` | Login required | List reservations, expand for detail, cancel, write reviews |
| `/profile` | Login required | View/edit profile, country dropdown, change password |
| `/invoices` | Login required | Invoice list with expandable detail (subtotal, tax, paid status) |
| `/subscriptions` | Public (view) / Login (subscribe) | Subscription plans — compare and subscribe |

## API Integration

All API calls go through `src/lib/api.ts` which wraps `fetch` with:
- JWT Bearer token from `localStorage` (key: `va_token`)
- `Accept-Language` header matching the selected locale
- Automatic 401 handling — clears token and notifies `AuthContext`

### Endpoints Used (29 total)

**Auth**: login, register, forgot-password, reset-password
**Profile**: get, update, change-password
**Hotels**: search, detail, room availability, reviews, cancellation-policy
**Reservations**: create draft, add line, add guest, submit, list, detail, cancel, remove line
**Invoices**: list, detail
**Reviews**: submit
**Subscriptions**: plans, my subscription, subscribe
**Promo Codes**: validate
**Reference**: translations, languages, currencies, countries

## Offline-First Design

The app works without the API running by falling back to:
- **Static data** in `src/data/` — hotels, rooms, reviews, users (seeded from the database)
- **Hardcoded translations** in `src/lib/translations-fallback.ts` — all UI strings in EN and ES
- **Mock authentication** — demo accounts work offline with password `password123`
- **Cached profile** — stored in `localStorage` to survive API unavailability

When the API comes online, real data replaces fallbacks transparently.

## Internationalization

Two languages supported: **English** (default) and **Spanish**.

Translation flow:
1. On page load, `LocaleContext` fetches translations from `GET /reference/translations`
2. If the API is unavailable, falls back to `translations-fallback.ts`
3. `t("key")` resolves: API translations → fallback for locale → key itself

Translation keys follow the pattern `client.{section}.{key}` (e.g., `client.nav.hotels`, `client.reservations.cancel`).

Currency is selectable independently (EUR, GBP, USD) — stored in `localStorage`.

## Booking Flow

1. User browses hotels and adds rooms to basket (stored in `localStorage`)
2. `/booking` page shows basket summary with optional promo code
3. Login required to proceed — unauthenticated users see a prompt
4. Guest information form → payment form (demo, no real processing)
5. On submit: creates draft reservation → adds lines + guests → submits → redirects to confirmation
6. Basket is cleared on success

## Context Providers

The app wraps all pages in three nested providers (see `layout.tsx`):

```
LocaleProvider          → language, currency, translations
  └─ AuthProvider       → user, login/register/logout, JWT management
       └─ BookingProvider → basket items, promo codes, checkout
```

## Development

```bash
cd docker/client-web/project
npm install
npm run dev          # http://localhost:3000
```

Environment variable: `NEXT_PUBLIC_API_URL` — defaults to `http://localhost:5002/api`.

In Docker, this is set to `http://client-api:5002/api` via docker-compose.

## Docker

Standalone Next.js output (`next.config.ts: output: "standalone"`). Built via multi-stage Dockerfile:

```bash
# From repository root:
docker compose up client-web
```

Depends on: `client-api` (which depends on `mariadb`, `redis`, `reservations-api`).

## SQL Translations

The fallback translations in `translations-fallback.ts` are kept in sync with `docker/database/init/007c-web-translation-seed-client.sql`. Both files contain the same ~180 keys in EN and ES. The SQL file is the source of truth when the database is running.
