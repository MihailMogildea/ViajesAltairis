# Reservations API

Central booking engine for the platform. All reservation operations flow through this service regardless of origin (client-api, admin-api, external-client-api). Internal service (no exposed host port). Other APIs reach it at `reservations-api:8080` on the Docker network.

## Purpose

- Creates and manages draft reservations (basket)
- Adds/removes reservation lines (room bookings with pricing calculation)
- Manages guests per line
- Submits reservations (payment processing, provider bookings)
- Cancels reservations (penalty calculation, refunds, external provider cancellation)
- Provides reservation and invoice read queries

## Tech Stack

- .NET 9.0, ASP.NET Core, MediatR (CQRS)
- EF Core (writes) + Dapper (reads and complex queries)
- MariaDB 11, Redis
- FluentValidation (pipeline behavior)
- Prometheus metrics (`/metrics`)

## Architecture

Thin API layer — controllers only map HTTP requests to MediatR commands/queries. All business logic lives in the shared kernel (`docker/kernel/project/`).

```
Controllers/
  ReservationsController.cs — 9 reservation endpoints + 4 request DTOs
  InvoicesController.cs     — 2 invoice endpoints
Kernel (Application/Reservations/)
  Commands/
    CreateDraftReservation.cs  — Draft creation with owner resolution
    AddReservationLine.cs      — Pricing, margins, discounts, taxes
    RemoveReservationLine.cs   — Line removal + header recalculation
    AddReservationGuest.cs     — Guest assignment to lines
    SubmitReservation.cs       — Payment + provider booking + promo validation
    CancelReservation.cs       — Penalty + refund + provider cancellation
  Queries/
    GetReservationById.cs      — Full reservation detail (lines + guests)
    GetReservationsByUser.cs   — Paginated list with status filter
    GetReservationLineInfo.cs  — Line→reservation→hotel→user lookup
    GetInvoiceById.cs          — Invoice detail
    GetInvoicesByUser.cs       — Paginated invoice list
```

## Endpoints

### Reservations (`/api/reservations`)

| Method | Route | Description | Returns |
|--------|-------|-------------|---------|
| POST | `/api/reservations/draft` | Create draft reservation | 201 + reservation ID |
| GET | `/api/reservations/{id}` | Reservation detail with lines and guests | 200 / 404 |
| GET | `/api/reservations?userId=&page=&pageSize=&status=` | Paginated list by user | 200 |
| POST | `/api/reservations/{id}/lines` | Add reservation line (room booking) | 201 + line ID |
| DELETE | `/api/reservations/{id}/lines/{lineId}` | Remove line, recalculate totals | 204 |
| POST | `/api/reservations/{id}/lines/{lineId}/guests` | Add guest to line | 204 |
| POST | `/api/reservations/{id}/submit` | Submit reservation (payment + booking) | 200 + SubmitResult |
| POST | `/api/reservations/{id}/cancel` | Cancel reservation (penalty + refund) | 204 |
| GET | `/api/reservations/lines/{lineId}/info` | Line info (reservation, hotel, user IDs) | 200 / 404 |

### Invoices (`/api/invoices`)

| Method | Route | Description | Returns |
|--------|-------|-------------|---------|
| GET | `/api/invoices?userId=&page=&pageSize=` | Paginated invoice list by user | 200 |
| GET | `/api/invoices/{id}?userId=` | Invoice detail (scoped to user) | 200 / 404 |

### Infrastructure

| Route | Description |
|-------|-------------|
| `/metrics` | Prometheus metrics |
| `/swagger` | Swagger UI (Development only) |

## Business Logic

### Reservation Flow

```
Draft → Confirmed → CheckedIn → Completed
  ↓         ↓
Cancelled  Cancelled (with penalty)
```

- **Draft**: basket phase, lines can be added/removed, guests assigned
- **Submit**: validates payment method, processes payment, books with external providers (3 retries, refund on failure)
- **Cancel**: calculates penalty from `cancellation_policy`, refunds if paid, cancels with external providers (best-effort)

### Pricing (AddReservationLine)

```
subtotal = (pricePerNight + boardPricePerNight) * numNights

Margins (additive, applied to subtotal):
  provider margin + hotel margin + seasonal margin (by region + date)

Discounts (additive, applied to subtotal + margin):
  user discount + business partner discount + subscription discount + promo %

Taxes (hierarchical, most specific per tax type wins):
  city > administrative_division > country

lineTotal = subtotal + marginAmount - discountAmount + taxAmount
```

Promo fixed amounts are applied at the reservation header level, not per line.

Currency conversion happens on-the-fly via `ICurrencyConverter` when provider currency differs from reservation currency.

### Cancellation

- Queries `cancellation_policy` for the strictest applicable policy (highest penalty %)
- Compares `free_cancellation_hours` against hours until earliest check-in
- If within free period: no penalty, full refund
- If outside: penalty = totalPrice * penalty %
- External providers: calls `IProviderReservationService.CancelBookingAsync` (best-effort, no retry)
- Refund: calls `IPaymentService.ProcessRefundAsync` for the latest completed payment

## Dependencies

| Service | Purpose |
|---------|---------|
| MariaDB | Persistent storage (EF Core + Dapper) |
| Redis | Cache layer |
| providers-api | External provider bookings/cancellations (via `IProviderReservationService`) |

Other APIs depend on reservations-api via `IReservationApiClient`:

| Consumer | Connection |
|----------|------------|
| admin-api | `ReservationsApi__BaseUrl` |
| client-api | `ReservationsApi__BaseUrl` |
| external-client-api | `ReservationsApi__BaseUrl` |

## Configuration

| Key | Description |
|-----|-------------|
| `ConnectionStrings:DefaultConnection` | MariaDB connection string |
| `ConnectionStrings:Redis` | Redis connection string |
| `Encryption:Key` | AES key for provider API password decryption |

## Tests

51 integration tests in `docker/tests/project/ViajesAltairis.Reservations.Api.Tests/`:

| Test Class | Count | Coverage |
|------------|-------|----------|
| CreateDraftReservationTests | 8 | Self/walk-in/on-behalf booking, promo codes, error cases |
| AddReservationLineTests | 12 | Pricing math, margins, discounts, taxes, currency conversion |
| RemoveReservationLineTests | 3 | Line removal, header recalculation, draft-only guard |
| AddReservationGuestTests | 2 | Guest addition, not-found error |
| SubmitReservationTests | 9 | Payment, promo re-validation, provider bookings, retry/refund |
| CancelReservationTests | 9 | Free/penalty cancellation, refunds, provider cancellation |
| GetReservationTests | 3 | Get by ID, get by user (paginated) |
| GetReservationLineInfoTests | 2 | Line info lookup |
| InvoiceTests | 3 | Invoice list, detail, wrong-user guard |

Run with:

```bash
dotnet test docker/tests/project/ViajesAltairis.Reservations.Api.Tests/
```
