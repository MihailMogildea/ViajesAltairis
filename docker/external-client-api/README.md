# External Client API

B2B partner API for hotel search and reservation management. Partners authenticate with JWT tokens (24h expiry) and can search hotels, check availability, and manage reservations on behalf of their customers.

**Port:** 5005

## Endpoints

### Authentication

| Method | Route | Auth | Description |
|--------|-------|:----:|-------------|
| POST | `/api/auth/login` | No | Authenticate with email/password, receive JWT token |

### Hotels

| Method | Route | Auth | Description |
|--------|-------|:----:|-------------|
| GET | `/api/hotels` | Yes | Search hotels (city, country, star range, pagination) |
| GET | `/api/hotels/{id}` | Yes | Hotel detail with rooms, board options, amenities |
| GET | `/api/hotels/{id}/availability` | Yes | Room availability for date range |

### Reservations

| Method | Route | Auth | Description |
|--------|-------|:----:|-------------|
| POST | `/api/reservations` | Yes | Create draft reservation (owner details, currency) |
| POST | `/api/reservations/{id}/lines` | Yes | Add room line to reservation |
| DELETE | `/api/reservations/{id}/lines/{lineId}` | Yes | Remove room line |
| POST | `/api/reservations/{id}/lines/{lineId}/guests` | Yes | Add guest to room line |
| POST | `/api/reservations/{id}/submit` | Yes | Submit reservation for confirmation |
| POST | `/api/reservations/{id}/cancel` | Yes | Cancel reservation |
| GET | `/api/reservations` | Yes | List partner's reservations (status filter, pagination) |
| GET | `/api/reservations/{id}` | Yes | Reservation detail with lines and guests |

## Authentication Flow

1. Partner calls `POST /api/auth/login` with agent credentials (user_type = `agent`)
2. Server validates password (bcrypt), checks user and partner are enabled
3. Returns JWT token with claims: `userId`, `email`, `businessPartnerId`, `partnerName`
4. Client sends `Authorization: Bearer <token>` on subsequent requests
5. Token expires after 24 hours

### JWT Configuration

| Setting | Default |
|---------|---------|
| Issuer | `ViajesAltairis.ExternalClientApi` |
| Audience | `ViajesAltairis.ExternalPartners` |
| Algorithm | HS256 |
| Expiration | 24 hours |
| Clock skew | 0 (no tolerance) |

## Authorization Layers

1. **JWT validation** — standard bearer token authentication on all protected endpoints
2. **BusinessPartnerAuthorizationMiddleware** — checks `business_partner.enabled` on every authenticated request; returns 403 if partner is disabled or not found
3. **Handler-level ownership** — reservation commands verify the reservation belongs to the authenticated partner's user before proceeding

## Reservation Flow

```
POST /reservations          → draft (status: draft)
POST /lines                 → add rooms to draft
POST /lines/{id}/guests     → add guests per room
POST /submit                → submit for confirmation (status: pending → confirmed)
POST /cancel                → cancel reservation (status: cancelled)
```

All reservation mutations route through `reservations-api` (internal, port 8080 on Docker network) via `IReservationApiClient`. The external-client-api never writes reservation data directly — it only reads via views and delegates writes.

## Architecture

```
external-client-api (thin controller layer)
├── Controllers extract JWT claims, map requests
├── MediatR dispatches to kernel handlers
│   ├── Hotel queries → Dapper against MariaDB views
│   ├── Login command → direct DB query + password verification
│   └── Reservation commands → ownership check + IReservationApiClient proxy
└── Middleware validates partner status on every request
```

### Kernel Features

All business logic lives in `ViajesAltairis.Application/Features/ExternalClient/`:

- **Auth** — `LoginCommand` + `LoginHandler` + `LoginValidator`
- **Hotels** — `SearchHotelsQuery`, `GetHotelDetailQuery`, `GetAvailabilityQuery` (read-only, Dapper)
- **Reservations** — 6 command handlers (ownership + delegation), 2 query handlers (Dapper + views)

### Database Views Used

| View | Purpose |
|------|---------|
| `v_hotel_card` | Hotel search results (name, stars, city, rating, cancellation policy) |
| `v_hotel_detail` | Hotel detail (full metadata, coordinates, review stats) |
| `v_hotel_room_catalog` | Room types with prices and provider info |
| `v_room_board_option` | Board options per room type |
| `v_hotel_amenity_list` | Hotel amenities by category |
| `v_reservation_summary` | Partner reservation list (header + line count) |
| `v_reservation_line_detail` | Reservation lines with hotel/room/board names |
| `v_reservation_guest_list` | Guests per reservation line |

### Validators

| Validator | Key Rules |
|-----------|-----------|
| LoginValidator | Email format, password not empty |
| CreatePartnerDraftValidator | Owner name required, valid email, currency code ≤ 3 chars |
| AddPartnerLineValidator | IDs > 0, CheckOut > CheckIn, guests > 0 |
| SubmitPartnerReservationValidator | Reservation and payment method IDs > 0 |
| CancelPartnerReservationValidator | Reservation ID > 0 |

## Configuration

### Environment Variables

| Variable | Description |
|----------|-------------|
| `ConnectionStrings__DefaultConnection` | MariaDB connection string |
| `Jwt__Key` | JWT signing key (override for production) |
| `ReservationsApi__BaseUrl` | URL for reservations-api proxy |
| `Redis__ConnectionString` | Redis connection string |

### Docker Compose

```yaml
external-client-api:
  build:
    context: .
    dockerfile: docker/external-client-api/Dockerfile
  ports:
    - "5005:8080"
  environment:
    - Jwt__Key=${JWT_KEY}
    - ReservationsApi__BaseUrl=http://reservations-api:8080
  depends_on:
    - database
    - reservations-api
    - redis
```

## Error Responses

| Status | Cause |
|--------|-------|
| 400 | Validation failure (FluentValidation) or invalid operation (ownership check) |
| 401 | Invalid credentials, missing/expired token, or missing required claim |
| 403 | Business partner disabled or not found (middleware) |
| 404 | Hotel or reservation not found |

## Project Structure

```
docker/external-client-api/project/
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── Authentication/
│   ├── AuthController.cs
│   └── JwtTokenService.cs
├── Configuration/
│   └── JwtSettings.cs
├── Hotels/
│   └── HotelsController.cs
├── Reservations/
│   └── ReservationsController.cs
└── Middleware/
    └── BusinessPartnerAuthorizationMiddleware.cs
```

## Tests

51 unit tests in `docker/tests/project/ViajesAltairis.ExternalClient.Api.Tests/`. Uses SQLite in-memory as a lightweight substitute for MariaDB (Dapper works natively with SQLite). No Docker required.

```bash
cd docker/tests/project
dotnet test ViajesAltairis.ExternalClient.Api.Tests/ --verbosity normal
```
