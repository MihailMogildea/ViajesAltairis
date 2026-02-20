# Scheduled API

Background job service for ViajesAltairis. Runs recurring tasks on DB-driven schedules using Hangfire.

## Docker

```bash
docker compose up scheduled-api
```

Internal service (no exposed host port). Admin-api reaches it at `scheduled-api:8080` on the Docker network. Depends on `database` and `redis`.

## Jobs

| Job Key | Class | Schedule | Purpose |
|---------|-------|----------|---------|
| `exchange-rate-sync` | `ExchangeRateSyncJob` | `0 3 * * *` (daily 3 AM) | Fetches ECB exchange rates, computes `rate_to_eur = 1/ecb_value`, updates `exchange_rate` table |
| `subscription-billing` | `SubscriptionBillingJob` | `0 1 1 * *` (1st of month) | Processes billing for active subscriptions |
| `provider-update` | `ProviderUpdateJob` | `0 */6 * * *` (every 6h) | Syncs room availability and pricing from external providers |

Schedules are stored in the `job_schedule` table and loaded at startup via `HangfireScheduleLoader`. Admin-api can update schedules and trigger reloads at runtime.

### Exchange Rate Sync Flow

1. Query all non-EUR currencies from `currency` table
2. Fetch latest rates from ECB CSV API (`data-api.ecb.europa.eu`)
3. Parse CSV via `IEcbRateParser` (extracted for testability)
4. For each currency: close current rate (`valid_to = NOW()`), insert new rate
5. Update `job_schedule.last_executed_at`

## Endpoints

| Method | Path | Purpose |
|--------|------|---------|
| `POST` | `/api/jobs/{jobKey}/trigger` | Manually trigger a job (enqueues via Hangfire) |
| `POST` | `/api/jobs/reload` | Reload schedules from `job_schedule` table |
| `GET` | `/api/health` | Health check |

Unknown job keys return `404`.

## Architecture

```
scheduled-api/
├── Controllers/
│   ├── JobsController.cs        # Trigger + reload endpoints
│   └── HealthController.cs      # Health check
├── Jobs/
│   ├── ExchangeRateSyncJob.cs   # ECB rate sync (delegates to MediatR)
│   ├── SubscriptionBillingJob.cs
│   └── ProviderUpdateJob.cs
├── Services/
│   └── HangfireScheduleLoader.cs  # Loads cron schedules from DB
└── Program.cs
```

Jobs are registered as scoped services. `ExchangeRateSyncJob` delegates to `SyncExchangeRatesCommand` in the kernel via MediatR. All jobs update `job_schedule.last_executed_at` after execution.

### Kernel Dependencies

- **`IEcbRateParser`** — CSV parsing + rate calculation (in `Application/Interfaces`, implemented in `Infrastructure/Services/EcbRateParser`)
- **`SyncExchangeRatesCommand`** — MediatR handler in `Application/Features/Scheduled/ExchangeRateSync/`
- **`IDbConnectionFactory`** — Dapper-based DB access
- **`IScheduledApiClient`** — Interface for admin-api to call trigger/reload endpoints

### Admin Integration

Admin-api manages job schedules via `JobSchedulesController`:
- `GET /api/job-schedules` — list all schedules
- `PUT /api/job-schedules/{jobKey}` — update cron expression / enabled flag
- `POST /api/job-schedules/{jobKey}/trigger` — trigger via `IScheduledApiClient`

After updating a schedule, admin-api calls `IScheduledApiClient.ReloadSchedulesAsync()` so Hangfire picks up the change.

## Configuration

Environment variables (via `docker-compose.yml`):

| Variable | Purpose |
|----------|---------|
| `ConnectionStrings__DefaultConnection` | MariaDB connection string |
| `ConnectionStrings__Redis` | Redis connection string |

Hangfire stores its state in MariaDB with `hangfire_` table prefix. The dashboard is available at `/hangfire` in development.

## Tests

```bash
dotnet test docker/tests/project/ViajesAltairis.Scheduled.Api.Tests
```

31 unit tests covering:
- `UpdateJobScheduleValidator` — FluentValidation rules (4 tests)
- `EcbRateParser` — CSV parsing + rate calculation (12 tests)
- `SyncExchangeRatesHandler` — dependency wiring (2 tests)
- `ExchangeRateSyncJob` — MediatR command dispatch (1 test)
- `SubscriptionBillingJob` — job execution flow (1 test)
- `ProviderUpdateJob` — job execution flow (1 test)
- `JobsController` — trigger all jobs + unknown + reload (6 tests)
- `HealthController` — health endpoint (1 test)
- `ScheduledApiClient` — HTTP calls + error handling (4 tests)

DB interactions (Dapper extension methods on `IDbConnection`) are not unit-testable without a real database — SQL correctness is verified via integration/manual testing.
