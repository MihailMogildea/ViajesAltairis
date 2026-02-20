# ViajesAltairis Monitoring

Prometheus + Grafana observability stack for all platform containers.

## Architecture

```
┌─────────────┐     scrape      ┌────────────┐     query      ┌─────────┐
│  cAdvisor   │────────────────▶│            │◀───────────────│         │
│  Redis Exp. │────────────────▶│ Prometheus │               │ Grafana │
│  MySQL Exp. │────────────────▶│   :9090    │───────────────▶│  :3003  │
│  6 .NET APIs│────────────────▶│            │  datasource    │         │
│  2 Next.js  │────────────────▶│            │                │         │
└─────────────┘                 └────────────┘                └─────────┘
  11 targets                    (internal)                    :3003
```

## Containers

| Container | Image | Purpose | Port |
|-----------|-------|---------|------|
| cadvisor | `gcr.io/cadvisor/cadvisor:v0.49.1` | Container CPU, memory, network I/O | internal |
| redis-exporter | `oliver006/redis_exporter:v1.66.0` | Redis connected clients, memory, hit/miss, commands/sec | internal |
| mysqld-exporter | `prom/mysqld-exporter:v0.16.0` | MariaDB connections, queries/sec, slow queries, InnoDB buffer | internal |
| prometheus | `prom/prometheus:v2.54.1` | Metrics aggregation + storage | internal |
| grafana | `grafana/grafana:11.2.2` | Dashboards + Explore | 3003 |

## Scrape Targets

| Job | Targets | Interval | Metrics Path |
|-----|---------|----------|--------------|
| `prometheus` | localhost:9090 | 15s | /metrics |
| `cadvisor` | cadvisor:8080 | 15s | /metrics |
| `redis` | redis-exporter:9121 | 15s | /metrics |
| `mariadb` | mysqld-exporter:9104 | 15s | /metrics |
| `dotnet-apis` | 6 APIs on :8080 | 10s | /metrics |
| `nextjs-webs` | 2 webs on :3000 | 15s | /api/metrics |

## File Structure

```
monitoring/
├── prometheus/
│   └── prometheus.yml              # Scrape configuration
├── grafana/
│   └── provisioning/
│       └── datasources/
│           └── prometheus.yml      # Auto-provisions Prometheus datasource
└── README.md
```

Config files are mounted read-only into their respective containers. Data persists in named volumes `prometheus-data` and `grafana-data`.

## Application Instrumentation

### .NET APIs (prometheus-net)

Each API has `prometheus-net.AspNetCore 8.2.1` in its `.csproj` and two middleware calls in `Program.cs`:

```csharp
app.UseHttpMetrics();   // after builder.Build()
app.MapMetrics();       // after MapControllers()
```

Exposed metrics:
- `http_request_duration_seconds` — histogram by method, status, controller, action
- `http_requests_in_progress` — gauge
- .NET runtime: GC collections, heap size, thread pool, exceptions
- Process: CPU seconds, memory bytes, uptime

### Next.js Apps (prom-client)

Each web app has `prom-client ^15.1.0` and a route handler at `src/app/api/metrics/route.ts`:

```typescript
import client from "prom-client";
const register = new client.Registry();
client.collectDefaultMetrics({ register });
// GET /api/metrics → register.metrics()
```

Exposed metrics:
- Node.js event loop lag, heap used/total, active handles/requests
- GC duration, process CPU, process memory

## Access

| URL | Purpose |
|-----|---------|
| http://localhost:3003 | Grafana — login with `admin` / `GRAFANA_ADMIN_PASSWORD` from `.env` |

Prometheus has no exposed host port — it is internal-only. Use Grafana's **Explore** mode to query metrics and verify scrape targets (`up` metric).

Default Grafana password: `altairis_grafana` (set in `.env`).

Prometheus is auto-provisioned as the default datasource — use Grafana's **Explore** mode to query immediately.

## Useful PromQL Queries

```promql
# Request rate per API (5m window)
rate(http_request_duration_seconds_count[5m])

# 95th percentile latency per API
histogram_quantile(0.95, rate(http_request_duration_seconds_bucket[5m]))

# Container memory usage
container_memory_usage_bytes{name=~"viajes.*"}

# Redis hit ratio
redis_keyspace_hits_total / (redis_keyspace_hits_total + redis_keyspace_misses_total)

# MariaDB queries per second
rate(mysql_global_status_queries[5m])

# .NET GC collections
rate(dotnet_collection_count_total[5m])
```

## Known Limitations

- **cAdvisor on Docker Desktop (WSL2)**: Some filesystem metrics may be unavailable
- **mysqld-exporter**: If `DB_USER` lacks `PROCESS` privilege, some metrics fail — can grant via init SQL if needed
- **No custom dashboards**: MVP uses Grafana Explore mode; dashboard JSON provisioning can be added later in `grafana/provisioning/dashboards/`
