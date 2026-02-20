# ViajesAltairis

B2B/B2C hotel reservation platform. Microservice architecture with .NET APIs, Next.js web apps, MariaDB, Redis, and Prometheus/Grafana monitoring.

## Quick Start

```bash
# Clone and start all 15 containers
docker compose up -d

# Verify everything is running
docker compose ps
```

All credentials are in `.env` (committed for MVP convenience).

## Services

### APIs (.NET 9, Clean Architecture, CQRS with MediatR)

| Service | Port | Purpose | README |
|---------|------|---------|--------|
| admin-api | 5001 | Internal admin operations — full CRUD for every entity, statistics, job scheduling | [docker/admin-api/](docker/admin-api/README.md) |
| client-api | 5002 | Client-facing API — auth, hotel search, reservations, invoices, profile, subscriptions | [docker/client-api/](docker/client-api/README.md) |
| providers-api | internal | Hotel provider management — external provider clients (HotelBeds, Booking.com, TravelGate), sync service | [docker/providers-api/](docker/providers-api/README.md) |
| reservations-api | internal | Central booking engine — all reservation mutations flow through here regardless of origin | [docker/reservations-api/](docker/reservations-api/README.md) |
| external-client-api | 5005 | B2B partner API — token auth (24h expiry), hotel search, reservation flow for business partners | [docker/external-client-api/](docker/external-client-api/README.md) |
| scheduled-api | internal | Scheduled jobs — exchange rate sync (ECB), subscription billing, provider catalog updates | [docker/scheduled-api/](docker/scheduled-api/README.md) |

### Web Apps (Next.js 15, App Router, Tailwind CSS)

| Service | Port | Purpose | README |
|---------|------|---------|--------|
| admin-web | 3001 | Admin dashboard — manage hotels, providers, reservations, users, pricing, system config | [docker/admin-web/](docker/admin-web/README.md) |
| client-web | 3002 | Client booking website — hotel search, reservation flow, profile, invoices | [docker/client-web/](docker/client-web/README.md) |

### Data

| Service | Port | Purpose | README |
|---------|------|---------|--------|
| database | internal | MariaDB 11 — 49 tables, 28 CQRS views, full seed data (22 hotels, 69 room configs, 10 reservations) | [docker/database/](docker/database/README.md) |
| redis | internal | Redis — translation/query caching, cache invalidation by prefix | — |

### Monitoring

| Service | Port | Purpose | README |
|---------|------|---------|--------|
| prometheus | internal | Metrics aggregation — scrapes 11 targets (6 APIs, 2 webs, cAdvisor, Redis exporter, MySQL exporter) | [docker/monitoring/](docker/monitoring/README.md) |
| grafana | 3003 | Dashboards — Prometheus auto-provisioned as datasource, Explore mode for querying | [docker/monitoring/](docker/monitoring/README.md) |
| cadvisor | — | Container metrics (CPU, memory, network I/O) | [docker/monitoring/](docker/monitoring/README.md) |
| redis-exporter | — | Redis metrics (connected clients, memory, hit/miss ratio, commands/sec) | [docker/monitoring/](docker/monitoring/README.md) |
| mysqld-exporter | — | MariaDB metrics (connections, queries/sec, slow queries, InnoDB buffer pool) | [docker/monitoring/](docker/monitoring/README.md) |

### Shared Libraries

| Component | Purpose | README |
|-----------|---------|--------|
| kernel | Shared .NET libraries (Domain, Application, Data, Infrastructure) referenced by all APIs | [docker/kernel/](docker/kernel/README.md) |
| tests | Integration tests for all APIs (WebApplicationFactory + NSubstitute) | [docker/tests/](docker/tests/README.md) |

## Architecture

```
                                    ┌──────────────┐
                                    │  admin-web   │
                                    │    :3001     │
                                    └──────┬───────┘
                                           │
┌──────────────┐   ┌──────────────┐   ┌────▼─────────┐   ┌───────────────┐
│  client-web  │──▶│  client-api  │──▶│  admin-api   │──▶│ scheduled-api │
│    :3002     │   │    :5002     │   │    :5001     │   │  (internal)   │
└──────────────┘   └──────┬───────┘   └──────┬───────┘   └───────────────┘
                          │                  │
                          ▼                  ▼
                   ┌──────────────┐   ┌──────────────┐
                   │ reservations │   │ providers    │
                   │ (internal)   │   │ (internal)   │
                   └──────┬───────┘   └──────┬───────┘
                          │                  │
                   ┌──────▼──────────────────▼───────┐
                   │   database + redis (internal)    │
                   └─────────────────────────────────┘

┌──────────────────┐
│external-client-api│──▶ reservations-api (B2B flow)
│      :5005       │
└──────────────────┘

Exposed: admin-web, admin-api, client-web, client-api, external-client-api, grafana
Internal: database, redis, reservations-api, providers-api, scheduled-api, prometheus
```

All reservations flow through `reservations-api` regardless of origin (client-api, admin-api, external-client-api). APIs never access the reservation database directly — they delegate via HTTP through `IReservationApiClient`.

## Project Structure

```
ViajesAltairis/
├── docker-compose.yml          # All 15 containers
├── .env                        # Credentials (committed for MVP)
├── CLAUDE.md                   # AI assistant context
└── docker/
    ├── admin-api/              # Dockerfile + project/
    ├── admin-web/              # Dockerfile + project/
    ├── client-api/             # Dockerfile + project/
    ├── client-web/             # Dockerfile + project/
    ├── database/               # Dockerfile + init/ (55 SQL files)
    ├── external-client-api/    # Dockerfile + project/
    ├── kernel/                 # Shared .NET libraries (no Dockerfile)
    ├── monitoring/             # Prometheus + Grafana config (no Dockerfile)
    ├── providers-api/          # Dockerfile + project/
    ├── reservations-api/       # Dockerfile + project/
    ├── scheduled-api/          # Dockerfile + project/
    └── tests/                  # Integration tests (no Dockerfile)
```

## Domain Concepts

**Reservation flow:** draft (basket) → pending → confirmed → checked_in → completed

**Margin stack** (additive, applied at booking):
1. Provider margin (`provider.margin`)
2. Hotel margin (`hotel.margin`)
3. Seasonal margin (`seasonal_margin` by region + date range)

**Discount stack** (additive, applied at booking):
1. Business partner discount
2. User discount (admin-granted)
3. Subscription discount
4. Promo code (one per reservation)

**Currency:** EUR base. Exchange rate snapshot frozen on reservation. Invoices in client currency, EUR conversion at export.

**Translation:** Decoupled `entity_type + entity_id + field + language_id` in a single `translation` table. Languages: en, es.

## Access Points

| URL | Purpose |
|-----|---------|
| http://localhost:3001 | Admin dashboard |
| http://localhost:3002 | Client booking website |
| http://localhost:5001/swagger | Admin API docs |
| http://localhost:5002/swagger | Client API docs |
| http://localhost:5005/swagger | External Client API docs |
| http://localhost:3003 | Grafana (admin / `altairis_grafana`) |

> **Internal services** (database, redis, reservations-api, providers-api, scheduled-api, prometheus) have no exposed host ports. They communicate over the `altairis-network` Docker bridge. To query Prometheus metrics, use Grafana's **Explore** mode.

## Scaling Considerations

This setup runs all services as single containers on a single Docker host. It is designed as an **MVP** and will handle moderate traffic reliably. When traffic grows to the point where horizontal scaling is needed, the following adaptations are required:

### What scales by replication

In a Kubernetes or Docker Swarm deployment, most services can be horizontally scaled by adding replicas behind a load balancer:

- **All APIs** (admin-api, client-api, reservations-api, external-client-api, providers-api) — stateless, scale freely
- **Web apps** (admin-web, client-web) — stateless Next.js, scale freely
- **Redis** — move to Redis Cluster or Redis Sentinel for HA
- **MariaDB** — read replicas for CQRS queries, primary for writes

### What must NEVER be replicated

> **scheduled-api must run as exactly one instance at all times — no exceptions.** It uses Hangfire for scheduled job execution. Multiple instances would cause duplicate job runs (double subscription billing, duplicate exchange rate imports, corrupted exchange rate history). This is not a limitation that can be worked around with distributed locks or leader election — Hangfire with MySQL storage does not support safe multi-instance execution. Even in Kubernetes, enforce `replicas: 1` and use a `Recreate` deployment strategy (not `RollingUpdate`) to guarantee no overlap during deploys.

### Monitoring adaptations for scale

The current Prometheus setup uses static scrape targets (container names hardcoded in `prometheus.yml`). This works for a single Docker host but needs to change at scale:

- **Kubernetes:** Replace static targets with Prometheus Operator `ServiceMonitor` CRDs — automatically discovers pods via label selectors, no manual target management
- **Service discovery:** Prometheus supports `kubernetes_sd_configs` for auto-discovery of new replicas as they scale up/down
- **Metrics cardinality:** With N replicas per service, aggregate using `sum by (job)` or `sum by (namespace, pod)` in PromQL — Grafana dashboards will show the full fleet view
- **Storage:** Single-node Prometheus is fine for MVP. At scale, consider Thanos or Grafana Mimir for long-term storage and cross-cluster querying
- **Exporters:** cAdvisor runs as a DaemonSet (one per node). Redis and MySQL exporters remain singletons pointing at their respective data stores

The MVP monitoring stack provides the same metrics categories (HTTP latency, container resources, DB stats, Redis stats) that will be needed at scale — only the discovery mechanism and storage backend change.

## Environment Variables

All defined in `.env`:

| Variable | Purpose |
|----------|---------|
| `DB_ROOT_PASSWORD` | MariaDB root password |
| `DB_NAME` | Database name |
| `DB_USER` / `DB_PASSWORD` | Application database credentials |
| `REDIS_PASSWORD` | Redis auth |
| `ENCRYPTION_KEY` | AES key for provider API password encryption |
| `JWT_SECRET` | HMAC key for JWT tokens (min 32 chars) |
| `NEXT_PUBLIC_API_URL` | Client API URL exposed to browser |
| `GRAFANA_ADMIN_PASSWORD` | Grafana admin login |

> **Production:** Replace all values, use proper secret management (Vault, K8s Secrets, cloud KMS). Never commit real credentials.
