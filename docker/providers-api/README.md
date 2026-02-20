# Providers API

Hotel provider management and external provider integration service. Internal service (no exposed host port). Other APIs reach it at `providers-api:8080` on the Docker network.

## Purpose

- Manages hotel providers (internal and external)
- Syncs hotel catalogs from external providers into the local database
- Proxies availability searches, bookings, and cancellations to external providers
- Exposes synced hotel data for search and detail queries

## Tech Stack

- .NET 9.0, Dapper, MySqlConnector
- Redis (cache invalidation after sync)
- Prometheus metrics (`/metrics`)

## Architecture

Standalone microservice — no kernel dependency. Uses Dapper with raw SQL for all data access.

```
Controllers/
  ProvidersController.cs    — Provider CRUD + sync + external operations
  HotelsController.cs       — Hotel search and detail
  RoomTypesController.cs    — Room types with board prices
ExternalClients/
  IExternalProviderClient.cs — Common interface for all providers
  ExternalDtos.cs            — Normalized DTOs (provider-agnostic)
  HotelBeds/                 — HotelBeds client + mapper + raw models
  BookingDotCom/             — Booking.com client + mapper + raw models
  TravelGate/                — TravelGate client + mapper + raw models
Repositories/
  IProviderRepository.cs     — Provider data access interface
  IHotelSyncRepository.cs   — Hotel/room/board data access interface
  ProviderRepository.cs      — Provider queries (Dapper)
  HotelSyncRepository.cs    — Hotel sync + search queries (Dapper)
Services/
  SyncService.cs             — Sync orchestration (fire-and-forget)
  ProviderRegistrationService.cs — Auto-registers external providers on startup
```

## Endpoints

### Providers (`/api/providers`)

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/providers` | List all enabled providers |
| GET | `/api/providers/{id}` | Provider detail |
| POST | `/api/providers/{id}/sync` | Trigger catalog sync (external only, 202 Accepted) |
| GET | `/api/providers/{id}/availability` | Search availability by city |
| GET | `/api/providers/{id}/availability/hotel/{hotelId}` | Availability for a specific hotel |
| POST | `/api/providers/{id}/book` | Create booking |
| DELETE | `/api/providers/{id}/bookings/{reference}` | Cancel booking |

External-only endpoints (sync, availability, book, cancel) return **400** for internal providers and **404** if the provider doesn't exist. Sync returns **409** if already in progress.

### Hotels (`/api/hotels`)

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/hotels?city=X&stars=N` | Search hotels (both params optional) |
| GET | `/api/hotels/{id}` | Hotel detail |

### Room Types (`/api/roomtypes`)

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/roomtypes?hotelId=X&providerId=Y` | Room types with nested board prices |

### Infrastructure

| Route | Description |
|-------|-------------|
| `/health` | Health check |
| `/metrics` | Prometheus metrics |
| `/swagger` | Swagger UI |

## External Provider Clients

Each client implements `IExternalProviderClient` and provides a hardcoded demo catalog (no real API calls). Each has its own raw model types and a mapper that normalizes to the shared DTOs.

| Client | Hotels | Region |
|--------|--------|--------|
| HotelBeds | 4 | Mallorca (Palma, Calvia, Pollenca) |
| Booking.com | 4 | Barcelona |
| TravelGate | 4 | Nice |

**Normalized room types**: single, double, twin, suite, junior_suite, deluxe
**Normalized board types**: room_only, bed_and_breakfast, half_board, full_board, all_inclusive

## Sync Flow

1. `POST /api/providers/{id}/sync` sets `sync_status = 'updating'` (row-level lock prevents concurrent syncs)
2. Fire-and-forget background task calls `ExecuteSyncAsync`
3. Resolves provider's currency and current exchange rate
4. Fetches hotel catalog from external client
5. For each hotel: match by name+city or create, ensure hotel-provider link, upsert rooms and boards
6. Prices get a random +/-15% variation (demo purposes)
7. On completion: sets `sync_status = 'updated'`, invalidates `hotel:*` Redis cache keys
8. On failure: sets `sync_status = 'failed'`

## Startup Behavior

`ProviderRegistrationService` runs on startup and auto-registers any external provider clients that aren't already in the database (type_id=2, margin=10%).

## Configuration

| Key | Default | Description |
|-----|---------|-------------|
| `ConnectionStrings:DefaultConnection` | (see appsettings.json) | MariaDB connection |
| `Redis:ConnectionString` | `localhost:6379` | Redis connection |

## Tests

20 integration tests in `docker/tests/project/ViajesAltairis.Providers.Api.Tests/`:

| Test Class | Count | Coverage |
|------------|-------|----------|
| ProvidersEndpointTests | 4 | Provider CRUD |
| HotelsEndpointTests | 4 | Hotel search + detail |
| RoomTypesEndpointTests | 2 | Room types with boards |
| ExternalOperationsTests | 6 | Availability, booking, cancellation |
| SyncEndpointTests | 4 | Sync trigger + conflict |

Run with:

```bash
dotnet test docker/tests/project/ViajesAltairis.Providers.Api.Tests/
```
