# Admin-Web

Next.js admin dashboard for the ViajesAltairis hotel reservation platform. Runs on port **3001** inside Docker.

## Docker

```bash
docker compose up admin-web
```

Depends on `admin-api` (port 5001) for all data operations. The API base URL defaults to `http://admin-api:8080` inside the Docker network and can be overridden with `NEXT_PUBLIC_ADMIN_API_URL`.

## Tech Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| Next.js | 15 | App Router, server components, server actions |
| React | 19 | UI rendering |
| TypeScript | 5.9 | Type safety |
| Tailwind CSS | 4 | Styling (via PostCSS) |
| jose | 6 | JWT decoding (edge-compatible) |
| prom-client | 15 | Prometheus metrics |

Output mode is `standalone` for Docker-optimized builds.

## Project Structure

```
src/
  app/
    (dashboard)/          # Protected routes (require auth)
      layout.tsx          # Sidebar + main content wrapper
      page.tsx            # Dashboard homepage
      audit/              # Audit log viewer
      business-partners/  # B2B partner management
      financial/          # Invoices, payment methods, transactions
      hotels/             # Hotel CRUD + detail pages
        [id]/             # Hotel detail (info, images, amenities, rooms)
      operations/         # Blackouts, cancellations, policies
      pricing/            # Promo codes, seasonal margins
      providers/          # Provider CRUD + detail pages
        [id]/             # Provider detail (info, hotel links, room types)
      reservations/       # Reservation list + booking flow
        [id]/             # Reservation detail (owner, lines, guests, submit)
      reviews/            # Review moderation
      statistics/         # Revenue, bookings, occupancy, users, financial
      subscriptions/      # Subscription types + user subscriptions
      system/             # Reference data (15+ subsections)
      users/              # User management + detail pages
        [id]/             # User detail (info, hotel access, subscriptions)
    api/metrics/          # Prometheus metrics endpoint
    login/                # Public login page
    unauthorized/         # 403 error page
    layout.tsx            # Root layout
    globals.css           # Tailwind imports
  components/             # 13 shared UI components
  lib/                    # Server utilities (auth, api, permissions, i18n)
  types/                  # 13 TypeScript DTO modules
  middleware.ts           # Auth + RBAC route protection
```

## Authentication

JWT-based. The admin-api issues tokens on login; the admin-web stores them in an `altairis_token` httpOnly cookie.

The JWT payload contains: `sub` (userId), `email`, `name`, `user_type_id`, `provider_id`, `business_partner_id`, `exp`.

Middleware validates expiry on every request and redirects to `/login` when missing or expired. On 401 from the API, the cookie is cleared automatically.

## Role-Based Access Control

Four roles with granular section-level permissions defined in `lib/permissions.ts`:

| Section | Admin | Manager | Agent | Hotel Staff |
|---------|-------|---------|-------|-------------|
| Dashboard | full | full | own | own |
| Hotels | full | read | read | own |
| Providers | full | - | - | - |
| Reservations | full | full | own | own |
| Users | full | read | - | - |
| Business Partners | full | read | own | - |
| Pricing | full | read | - | - |
| Subscriptions | full | read | read | - |
| Financial | full | read | own | own |
| Operations | full | full | - | own |
| Reviews | full | full | - | own |
| System Config | full | - | - | - |
| Audit Log | full | - | - | - |
| Statistics | read | read | - | - |

Access levels:
- **full** — CRUD on all records
- **read** — read-only access
- **own** — scoped to the user's hotels, partner, or own records

Middleware enforces access at the route level. UI components also check access to show/hide actions.

## Internationalization

Two languages: English (`en`) and Spanish (`es`). The active locale is stored in an `altairis_locale` cookie (defaults to `en`).

Translations are fetched from admin-api (`/api/webtranslations/public`) and cached for 60 seconds. Translation keys use dot notation with an `admin.` prefix (e.g., `admin.reservations.add_line`). The locale switcher is in the sidebar.

## API Communication

All API calls go through `lib/api.ts` which:
- Prepends the admin-api base URL
- Attaches `Authorization: Bearer {token}` and `Accept-Language: {locale}` headers
- Handles 401 (clears session) and 403 (throws) responses

Data fetching happens in server components via `apiFetch()`. Mutations use server actions that call the same function.

## Shared Components

| Component | Purpose |
|-----------|---------|
| `data-table` | Generic table with columns, actions, and empty state |
| `form-modal` | Modal dialog for create/edit forms |
| `form-field` | Labeled input (text, number, email, date, select, checkbox, textarea) |
| `sidebar` | Navigation filtered by role, with locale switcher and logout |
| `status-badge` | Colored badge (enabled, disabled, success, warning, danger, info) |
| `breadcrumb` | Page breadcrumb navigation |
| `section-header` | Section title |
| `tab-bar` | Tabbed interface |
| `toast-message` | Dismissable toast notification |
| `confirm-dialog` | Confirmation modal |
| `bar-chart` | Chart visualization |
| `date-range-picker` | Date range input with preset buttons |
| `locale-switcher` | EN/ES toggle buttons |

## Type Definitions

TypeScript interfaces in `src/types/` mirror admin-api DTOs:

| File | Domain |
|------|--------|
| `hotel.ts` | Hotels, images, amenities, hotel-providers, room types, blackouts |
| `user.ts` | Users, user types, hotel access, subscriptions |
| `reservation.ts` | Reservations, lines, guests, statuses, cancellations |
| `system.ts` | Languages, currencies, countries, exchange rates, cities, taxes, translations, email templates, notifications |
| `provider.ts` | Providers |
| `business-partner.ts` | Business partners |
| `pricing.ts` | Promo codes, seasonal margins |
| `subscription.ts` | Subscription types |
| `payment.ts` | Payment methods, transactions |
| `invoice.ts` | Invoices |
| `review.ts` | Reviews |
| `statistics.ts` | Statistics aggregates |
| `audit.ts` | Audit log entries |

## Page Sections

### Dashboard
Stats cards (hotels, pending reservations, hidden reviews) and recent reservations table.

### Hotels
List with create/edit. Detail page with tabs: hotel info, images (sortable), amenities, provider room types.

### Providers
List with create/edit. Detail page: provider info, hotel links, room type configurations.

### Reservations
List with status filter pills and create-draft modal. Detail page with full booking flow:
- Owner snapshot (frozen at booking time)
- Reservation lines table (add/remove in draft status)
- Guests grouped by line (add in draft status)
- Pricing breakdown
- Submit with payment method dropdown
- Cancel with reason

### Users
List with create. Detail page: user info, hotel access assignments, subscription history.

### Financial
Three tabs: invoices, payment methods (CRUD), transactions.

### Operations
Three tabs: blackout periods, cancellations, cancellation policies.

### Reviews
List with visibility toggle for moderation.

### Statistics
Five tabs: revenue (by hotel, provider, period), bookings (volume, by status, averages), occupancy, users (growth, by type, subscriptions), financial (cancellations, promo codes, MRR).

### System Config
Hub page linking to 15+ subsections: countries, currencies, cities, languages, exchange rates, taxes, translations, web translations, email templates, notifications, scheduler (Hangfire jobs), provider types, admin divisions, admin division types, board types, room types, amenities.

### Audit Log
Searchable table of all system changes (entity type, entity ID, action, user, old/new values, timestamp).

## Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `NEXT_PUBLIC_ADMIN_API_URL` | `http://admin-api:8080` | Admin API base URL |

Port 3000 internally, mapped to **3001** in docker-compose.

## Development

The project is designed to run inside Docker. For local development:

```bash
cd docker/admin-web/project
npm install
npm run dev
```

Note: `NEXT_PUBLIC_ADMIN_API_URL` must point to a running admin-api instance (e.g., `http://localhost:5001`).
