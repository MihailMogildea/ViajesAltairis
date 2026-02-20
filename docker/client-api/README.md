# client-api

Client-facing REST API for the ViajesAltairis hotel reservation platform. Serves the `client-web` Next.js frontend.

**Port:** 5002 (host) → 8080 (container)

## Architecture

Thin controller layer — all business logic lives in the shared kernel (`ViajesAltairis.Application`). Controllers inject `IMediator` and dispatch commands/queries to kernel handlers. Reservation operations are delegated to `reservations-api` via `IReservationApiClient`.

## Endpoints

### Public (AllowAnonymous)

| Method | Route | Handler | Description |
|--------|-------|---------|-------------|
| POST | `/api/auth/login` | `LoginCommand` | JWT login |
| POST | `/api/auth/register` | `RegisterCommand` | Client registration |
| POST | `/api/auth/forgot-password` | `ForgotPasswordCommand` | Password reset request |
| POST | `/api/auth/reset-password` | `ResetPasswordCommand` | Password reset confirmation |
| GET | `/api/hotels` | `SearchHotelsQuery` | Hotel search with filters |
| GET | `/api/hotels/{id}` | `GetHotelDetailQuery` | Hotel detail page |
| GET | `/api/hotels/{id}/availability` | `GetRoomAvailabilityQuery` | Room availability + pricing |
| GET | `/api/hotels/{id}/reviews` | `GetHotelReviewsQuery` | Hotel reviews |
| GET | `/api/hotels/{id}/cancellation-policy` | `GetCancellationPolicyQuery` | Cancellation terms |
| GET | `/api/subscriptions/plans` | `GetSubscriptionPlansQuery` | Available subscription plans |
| GET | `/api/reference/languages` | `GetLanguagesQuery` | Language catalog |
| GET | `/api/reference/currencies` | `GetCurrenciesQuery` | Currency catalog |
| GET | `/api/reference/countries` | `GetCountriesQuery` | Country catalog |
| GET | `/api/reference/translations` | `GetWebTranslationsQuery` | UI translation strings |

### Protected (Authorize Roles = "Client")

| Method | Route | Handler | Description |
|--------|-------|---------|-------------|
| GET | `/api/reservations` | `GetMyReservationsQuery` | List user reservations |
| GET | `/api/reservations/{id}` | `GetReservationDetailQuery` | Reservation detail |
| POST | `/api/reservations` | `CreateDraftReservationCommand` | Create draft (basket) |
| POST | `/api/reservations/{id}/lines` | `AddReservationLineCommand` | Add room to reservation |
| DELETE | `/api/reservations/{id}/lines/{lineId}` | `RemoveReservationLineCommand` | Remove room |
| POST | `/api/reservations/{id}/lines/{lineId}/guests` | `AddReservationGuestCommand` | Add guest to room |
| POST | `/api/reservations/{id}/submit` | `SubmitReservationCommand` | Submit reservation |
| POST | `/api/reservations/{id}/cancel` | `CancelReservationCommand` | Cancel reservation |
| GET | `/api/profile` | `GetProfileQuery` | Get user profile |
| PUT | `/api/profile` | `UpdateProfileCommand` | Update profile |
| PUT | `/api/profile/password` | `ChangePasswordCommand` | Change password |
| GET | `/api/invoices` | `GetMyInvoicesQuery` | List user invoices |
| GET | `/api/invoices/{id}` | `GetInvoiceDetailQuery` | Invoice detail |
| POST | `/api/reviews` | `SubmitReviewCommand` | Submit hotel review |
| GET | `/api/subscriptions/me` | `GetMySubscriptionQuery` | Current subscription |
| POST | `/api/subscriptions` | `SubscribeCommand` | Subscribe to plan |
| GET | `/api/promocodes/validate` | `ValidatePromoCodeQuery` | Validate promo code |

## Authentication

JWT Bearer tokens issued by `POST /api/auth/login`.

- Signing key: `Jwt:SecretKey` config
- Issuer: `ViajesAltairis.ClientApi`
- Audience: `ViajesAltairis.ClientWeb`
- Expiry: 60 minutes
- Role claim mapped to `ClaimTypes.Role`

## Local Services

| Service | Description |
|---------|-------------|
| `CurrentUserService` | Extracts `UserId`, `Email`, `UserType` from JWT claims; `LanguageId` from `Accept-Language` header (es→2, default→1); `CurrencyCode` from `X-Currency` header (default EUR) |
| `ExceptionHandlingMiddleware` | Maps exceptions to HTTP status: `KeyNotFoundException`→404, `UnauthorizedAccessException`→403, `InvalidOperationException`→400, `ValidationException`→400, others→500 |

## Dependencies

- **Kernel**: Domain, Application, Data, Infrastructure (project references)
- **NuGet**: `Microsoft.AspNetCore.Authentication.JwtBearer`, `prometheus-net.AspNetCore`, `Swashbuckle.AspNetCore`
- **Runtime services**: MariaDB, Redis, reservations-api, providers-api

## Configuration

| Key | Default | Description |
|-----|---------|-------------|
| `ConnectionStrings:DefaultConnection` | `Server=database;Port=3306;...` | MariaDB connection |
| `ConnectionStrings:Redis` | `redis:6379,password=altairis_redis` | Redis connection |
| `Jwt:SecretKey` | (from env `Jwt__Key`) | JWT signing key |
| `ReservationsApi:BaseUrl` | `http://reservations-api:8080` | Reservations API URL |
| `ProvidersApi:BaseUrl` | `http://providers-api:8080` | Providers API URL |

## Docker

Multi-stage build: `dotnet/sdk:9.0` (build) → `dotnet/aspnet:9.0` (runtime). Build context is `docker/` to resolve kernel project references.

```bash
docker compose up client-api
```

## Development

The kernel solution (`docker/kernel/project/ViajesAltairis.Kernel.sln`) builds locally for validation. Full project-reference resolution requires Docker build context.

Swagger UI available at `/swagger` in development mode.
