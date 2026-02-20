# Admin API

Internal administration service for the ViajesAltairis hotel reservation platform.

- **Port**: 5001 (external) / 8080 (internal)
- **Framework**: .NET 9, ASP.NET Core
- **Architecture**: Clean Architecture + CQRS (MediatR)
- **Auth**: JWT Bearer (HS256, 8h expiry)

## Authentication

```
POST /api/auth/login
Body: { "email": "...", "password": "..." }
Response: { "token": "..." }
```

Client users (user_type_id = 5) are rejected. Only admin staff can authenticate.

Include the token in subsequent requests:

```
Authorization: Bearer <token>
```

## Role-Based Access Control

The `RoleAuthorizationMiddleware` enforces per-section permissions based on user type:

| Section | Admin | Manager | Agent | Hotel Staff |
|---|---|---|---|---|
| system | full | - | - | - |
| hotels | full | read | read | own |
| providers | full | - | - | - |
| reservations | full | full | own | own |
| users | full | read | - | - |
| business-partners | full | read | own | - |
| pricing | full | read | - | - |
| subscriptions | full | read | read | - |
| financial | full | read | own | own |
| operations | full | full | - | own |
| reviews | full | full | - | own |
| audit | full | - | - | - |
| statistics | read | read | - | - |

- **full** = all HTTP methods
- **read** = GET only
- **own** = all methods, scoped to user's data

## Endpoints

### System

| Method | Route | Description |
|---|---|---|
| CRUD | `/api/languages` | Language management |
| CRUD | `/api/countries` | Country management |
| CRUD | `/api/administrativedivisions` | Administrative divisions |
| CRUD | `/api/administrativedivisiontypes` | Division type definitions |
| CRUD | `/api/currencies` | Currency management |
| CRUD | `/api/exchangerates` | Exchange rate management |
| CRUD | `/api/translations` | Entity translations |
| CRUD | `/api/webtranslations` | Web UI translations |
| GET | `/api/webtranslations/public` | Public translations (no auth) |
| CRUD | `/api/providertypes` | Provider type definitions |
| CRUD | `/api/emailtemplates` | Email template management |
| GET | `/api/notificationlogs` | Notification log (read-only) |
| GET | `/api/job-schedules` | List scheduled jobs |
| GET | `/api/job-schedules/{key}` | Get job by key |
| PUT | `/api/job-schedules/{key}` | Update cron/enabled |
| POST | `/api/job-schedules/{key}/trigger` | Trigger job immediately |

### Hotels

| Method | Route | Description |
|---|---|---|
| CRUD | `/api/hotels` | Hotel management |
| PATCH | `/api/hotels/{id}/enabled` | Toggle hotel enabled |
| CRUD | `/api/roomtypes` | Room type definitions |
| CRUD | `/api/hotelimages` | Hotel image management |
| CRUD | `/api/roomimages` | Room image management |
| CRUD | `/api/amenities` | Amenity definitions |
| CRUD | `/api/amenitycategories` | Amenity category definitions |
| CRUD | `/api/hotelamenities` | Hotel-amenity associations |
| CRUD | `/api/hotelproviderroomtypeamenities` | Room-amenity associations |

### Providers

| Method | Route | Description |
|---|---|---|
| CRUD | `/api/providers` | Provider management |
| CRUD | `/api/hotelproviders` | Hotel-provider links |
| CRUD | `/api/hotelproviderroomtypes` | Room configurations per provider |
| CRUD | `/api/hotelproviderroomtypeboards` | Board options per room config |
| CRUD | `/api/boardtypes` | Board type definitions |

### Reservations

| Method | Route | Description |
|---|---|---|
| GET | `/api/reservations` | List all reservations |
| GET | `/api/reservations/{id}` | Get reservation details |
| POST | `/api/reservations` | Create draft reservation |
| PATCH | `/api/reservations/{id}/status` | Change reservation status |
| POST | `/api/reservations/{id}/cancel` | Cancel reservation |
| POST | `/api/reservations/{id}/submit` | Submit (finalize) reservation |
| GET | `/api/reservations/{id}/lines` | Get reservation lines |
| POST | `/api/reservations/{id}/lines` | Add line to reservation |
| DELETE | `/api/reservations/{id}/lines/{lineId}` | Remove line |
| GET | `/api/reservations/{id}/guests` | Get guests |
| POST | `/api/reservations/{id}/lines/{lineId}/guests` | Add guest to line |
| CRUD | `/api/reservationstatuses` | Reservation status definitions |

Reservation operations are delegated to `reservations-api` via `IReservationApiClient`.

### Users

| Method | Route | Description |
|---|---|---|
| CRUD | `/api/users` | User management |
| PATCH | `/api/users/{id}/enabled` | Toggle user enabled |
| CRUD | `/api/usertypes` | User type definitions |
| CRUD | `/api/userhotels` | User-hotel associations |

### Business Partners

| Method | Route | Description |
|---|---|---|
| CRUD | `/api/businesspartners` | Business partner management |
| PATCH | `/api/businesspartners/{id}/enabled` | Toggle partner enabled |

### Pricing

| Method | Route | Description |
|---|---|---|
| CRUD | `/api/seasonalmargins` | Seasonal margin rules |
| CRUD | `/api/taxtypes` | Tax type definitions |
| CRUD | `/api/taxes` | Tax management |
| CRUD | `/api/promocodes` | Promo code management |
| PATCH | `/api/promocodes/{id}/enabled` | Toggle promo code enabled |

### Subscriptions

| Method | Route | Description |
|---|---|---|
| CRUD | `/api/subscriptiontypes` | Subscription tier definitions |
| CRUD | `/api/usersubscriptions` | User subscription management |

### Financial

| Method | Route | Description |
|---|---|---|
| GET | `/api/invoices` | List invoices |
| GET | `/api/invoices/{id}` | Get invoice details |
| PATCH | `/api/invoices/{id}/status` | Update invoice status |
| CRUD | `/api/invoicestatuses` | Invoice status definitions |
| CRUD | `/api/paymentmethods` | Payment method definitions |
| CRUD | `/api/paymenttransactions` | Payment transaction records |
| CRUD | `/api/paymenttransactionfees` | Transaction fee records |

### Operations

| Method | Route | Description |
|---|---|---|
| CRUD | `/api/hotelblackouts` | Hotel blackout dates |
| CRUD | `/api/cancellations` | Cancellation records |
| CRUD | `/api/cancellationpolicies` | Cancellation policy definitions |

### Reviews

| Method | Route | Description |
|---|---|---|
| CRUD | `/api/reviews` | Review management |
| CRUD | `/api/reviewresponses` | Review response management |

### Audit

| Method | Route | Description |
|---|---|---|
| GET | `/api/auditlogs` | List audit logs (read-only) |
| GET | `/api/auditlogs/{id}` | Get audit log entry |

### Statistics

All statistics endpoints are read-only and accept optional `from` and `to` query parameters for date filtering.

| Method | Route | Description |
|---|---|---|
| GET | `/api/statistics/revenue/by-hotel` | Revenue breakdown by hotel |
| GET | `/api/statistics/revenue/by-provider` | Revenue breakdown by provider |
| GET | `/api/statistics/revenue/by-period` | Revenue over time (groupBy param) |
| GET | `/api/statistics/bookings/volume` | Booking volume over time |
| GET | `/api/statistics/bookings/by-status` | Bookings by status |
| GET | `/api/statistics/bookings/average` | Average booking value |
| GET | `/api/statistics/occupancy` | Hotel occupancy rates |
| GET | `/api/statistics/users/growth` | User growth over time |
| GET | `/api/statistics/users/by-type` | Users by type |
| GET | `/api/statistics/users/subscriptions` | Subscription distribution |
| GET | `/api/statistics/reviews` | Review statistics |
| GET | `/api/statistics/cancellations` | Cancellation statistics |
| GET | `/api/statistics/promo-codes` | Promo code usage |
| GET | `/api/statistics/subscriptions/mrr` | Monthly recurring revenue |

## CRUD Convention

Standard CRUD controllers follow this pattern:

| Method | Route | Response |
|---|---|---|
| GET | `/api/{resource}` | 200 + array |
| GET | `/api/{resource}/{id}` | 200 + object / 404 |
| POST | `/api/{resource}` | 201 + object |
| PUT | `/api/{resource}/{id}` | 200 + object |
| DELETE | `/api/{resource}/{id}` | 204 |

## Error Responses

| Status | Cause | Body |
|---|---|---|
| 400 | Validation failure | `{ "errors": [{ "propertyName": "...", "errorMessage": "..." }] }` |
| 401 | Missing/invalid token | `{ "error": "Unauthorized" }` |
| 403 | Insufficient role | `{ "error": "Forbidden" }` |
| 404 | Entity not found | `{ "error": "..." }` |
| 500 | Unhandled exception | `{ "error": "An unexpected error occurred" }` |

## Headers

| Header | Purpose | Default |
|---|---|---|
| `Authorization` | Bearer JWT token | Required (except public routes) |
| `Accept-Language` | Response language (`en`, `es`) | `en` (language_id = 1) |
| `X-Currency` | Price currency code | `EUR` |

## Dependencies

- **database** (MariaDB) — primary data store
- **redis** — caching layer
- **reservations-api** — reservation workflow operations
- **scheduled-api** — job schedule management

## Configuration

| Variable | Description |
|---|---|
| `ConnectionStrings__DefaultConnection` | MariaDB connection string |
| `Redis__ConnectionString` | Redis connection string |
| `Encryption__Key` | AES key for provider password encryption |
| `Jwt__SecretKey` | HS256 signing key |
| `ReservationsApi__BaseUrl` | Reservations API URL |
| `ScheduledApi__BaseUrl` | Scheduled API URL |

## Project Structure

```
docker/admin-api/
  Dockerfile
  project/
    ViajesAltairis.AdminApi/
      Program.cs
      Auth/                  # AuthController, JwtTokenService, DTOs
      Middleware/             # ExceptionHandler, RoleAuthorization
      Services/               # CurrentUserService
      Features/               # 50 controllers organized by domain
```

Business logic lives in the shared kernel (`docker/kernel/project/ViajesAltairis.Application/Features/Admin/`). Controllers are thin routing layers that delegate to MediatR handlers.
