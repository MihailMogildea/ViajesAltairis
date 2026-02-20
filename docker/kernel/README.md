# ViajesAltairis Kernel

Shared .NET libraries referenced by all API microservices. Clean Architecture with DDD, CQRS via MediatR.

## Projects

```
kernel/project/
├── ViajesAltairis.Domain          # Entities, enums, domain interfaces
├── ViajesAltairis.Application     # Features (commands/queries/handlers), DTOs, behaviors, interfaces
├── ViajesAltairis.Data            # EF Core DbContext, Dapper, repositories
└── ViajesAltairis.Infrastructure  # Redis, JWT, encryption, email, payment, HTTP clients
```

All projects target **net9.0**.

## Build

```bash
dotnet build docker/kernel/project/ViajesAltairis.Kernel.sln
```

> **Note:** API solutions reference kernel via project references (`..\..\kernel\project\...`). These resolve only in Docker build context. The kernel `.sln` builds locally for validation.

---

## Domain

Zero dependencies. Contains all entities, enums, value objects, and repository interfaces.

### Entities

Base classes in `Entities/`:
- `BaseEntity` — `long Id`, `DateTime CreatedAt`
- `AuditableEntity : BaseEntity` — adds `DateTime UpdatedAt`

#### Reference & Geography

| Entity | Description |
|--------|-------------|
| `Currency` | ISO 4217 currencies (EUR, GBP) |
| `ExchangeRate` | Time-bound conversion rate to EUR |
| `Country` | Countries with default currency |
| `AdministrativeDivisionType` | Lookup: autonomous community, island, province, region, department |
| `AdministrativeDivision` | Self-referencing geographic hierarchy |
| `City` | Cities linked to administrative divisions |
| `Language` | Supported languages (en, es) |
| `Translation` | Decoupled: `entity_type + entity_id + field + language_id → value` |
| `WebTranslation` | UI text translations (key-value per language, page, section) |

#### Providers & Hotels

| Entity | Description |
|--------|-------------|
| `ProviderType` | Internal / External |
| `Provider` | Company with API credentials (encrypted), margin, sync status |
| `Hotel` | Property with city, stars, location, check-in/out, margin |
| `RoomType` | Single, Double, Twin, Suite, JuniorSuite, Deluxe |
| `HotelProvider` | Junction: which provider manages which hotel |
| `HotelProviderRoomType` | Room config per hotel-provider: capacity, quantity, price, currency |
| `HotelProviderRoomTypeBoard` | Board pricing per room config |
| `HotelProviderRoomTypeAmenity` | Room-level amenity assignments |
| `AmenityCategory` | Hotel or Room scope |
| `Amenity` | WiFi, pool, minibar, etc. |
| `HotelAmenity` | Hotel-level amenity assignments |
| `HotelImage` | Hotel photos with `url` + `sort_order` |
| `RoomImage` | Room photos linked to room config |
| `TaxType` | VAT, TouristTax, CityTax |
| `Tax` | Tax rules with geographic scoping |
| `CancellationPolicy` | Per-hotel: days before check-in → penalty percentage |
| `HotelBlackout` | Date ranges when a hotel is unavailable |
| `SeasonalMargin` | Per-region margin by month-day range |

#### Users & Partners

| Entity | Description |
|--------|-------------|
| `UserType` | Admin, Manager, Agent, HotelStaff, Client |
| `BusinessPartner` | B2B company with discount percentage |
| `User` | All users with type, credentials, optional provider/partner link |
| `UserHotel` | Staff assignment to specific hotels |
| `SubscriptionType` | Plan with monthly price and discount |
| `UserSubscription` | Active subscription per user |

#### Reservations & Payments

| Entity | Description |
|--------|-------------|
| `ReservationStatus` | Draft, Pending, Confirmed, CheckedIn, Completed, Cancelled |
| `PromoCode` | Discount code with validity period and usage limits |
| `BoardType` | RoomOnly, BedAndBreakfast, HalfBoard, FullBoard, AllInclusive |
| `Reservation` | Booking header: owner snapshot (frozen at booking), totals, currency, exchange rate |
| `ReservationLine` | Per-room booking: hotel/room config, dates, board, pricing breakdown |
| `ReservationGuest` | Guest details per line |
| `Cancellation` | Cancellation record with penalty and refund amounts |
| `Invoice` | Invoice with status, totals, period |
| `InvoiceStatus` | Created, Paid, Refunded |
| `PaymentMethod` | Payment method lookup |
| `PaymentTransaction` | Payment with reference, amount, currency |
| `PaymentTransactionFee` | Fee breakdown per transaction |

#### Reviews & Communications

| Entity | Description |
|--------|-------------|
| `Review` | Rating (1-5) + optional title/comment, visibility flag |
| `ReviewResponse` | Hotel staff response to a review |
| `EmailTemplate` | Transactional email templates (subject/body via Translation) |
| `NotificationLog` | Sent email tracking |
| `AuditLog` | Entity-level audit trail |

#### Scheduling

| Entity | Description |
|--------|-------------|
| `JobSchedule` | Hangfire job definition with cron expression and enabled flag |

### Enums

All in `Enums/`:

| Enum | Values |
|------|--------|
| `ReservationStatusEnum` | Draft=1, Pending=2, Confirmed=3, CheckedIn=4, Completed=5, Cancelled=6 |
| `UserTypeEnum` | Admin=1, Manager=2, Agent=3, HotelStaff=4, Client=5 |
| `InvoiceStatusEnum` | Created=1, Paid=2, Refunded=3 |
| `ProviderTypeEnum` | Internal=1, External=2 |
| `RoomTypeEnum` | Single=1, Double=2, Twin=3, Suite=4, JuniorSuite=5, Deluxe=6 |
| `BoardTypeEnum` | RoomOnly=1, BedAndBreakfast=2, HalfBoard=3, FullBoard=4, AllInclusive=5 |
| `AmenityCategoryEnum` | Hotel=1, Room=2 |
| `TaxTypeEnum` | Vat=1, TouristTax=2, CityTax=3 |
| `AdministrativeDivisionTypeEnum` | AutonomousCommunity=1, Island=2, Province=3, Region=4, Department=5 |

### Domain Interfaces

All in `Interfaces/`:

| Interface | Description |
|-----------|-------------|
| `IRepository<T>` | Generic CRUD for `BaseEntity` subtypes |
| `ISimpleRepository<T>` | Same CRUD without `BaseEntity` constraint (BoardType, etc.) |
| `IHotelRepository` | `GetByCityIdAsync`, `GetWithDetailsAsync` (eager loads) |
| `IReservationRepository` | `GetByCodeAsync`, `GetWithLinesAsync`, `GetByUserIdAsync` |
| `IUserRepository` | `GetByEmailAsync` |
| `IUnitOfWork` | `SaveChangesAsync` |

---

## Application

Depends on: **Domain**

Packages: MediatR 12.4, FluentValidation 11.11, Dapper 2.1, Microsoft.Extensions.Http/Logging

### DI Registration

```csharp
// Called by every API in Program.cs
builder.Services.AddApplicationServices();
```

Registers MediatR handlers + FluentValidation validators from this assembly, plus pipeline behaviors.

### Pipeline Behaviors

Executed in order for every MediatR request:

| Behavior | Purpose |
|----------|---------|
| `ValidationBehavior<,>` | Runs all `IValidator<TRequest>`, throws `ValidationException` on failure |
| `LoggingBehavior<,>` | Logs request name before/after handling |
| `CacheInvalidationBehavior<,>` | After commands implementing `IInvalidatesCache`, removes Redis keys by prefix |

### DTOs

| Class | Purpose |
|-------|---------|
| `Result<T>` | `IsSuccess`, `Data?`, `Error?` — standard response wrapper |
| `PagedResult<T>` | `Items`, `TotalCount`, `Page`, `PageSize`, computed `TotalPages` |

### Interfaces

17 interfaces in `Interfaces/`:

| Interface | Purpose |
|-----------|---------|
| `ICurrentUserService` | Resolved from JWT: `UserId`, `Email`, `UserType`, `LanguageId`, `CurrencyCode` |
| `IJwtTokenService` | `GenerateToken(userId, email, userType)` |
| `IDbConnectionFactory` | `CreateConnection()` for Dapper |
| `ICacheService` | `Get/Set/Remove/RemoveByPrefixAsync` |
| `IInvalidatesCache` | Static interface declaring `CachePrefixes` for auto-invalidation |
| `IEmailService` | `SendEmailAsync(to, subject, body)` |
| `IAuditService` | `LogAsync(userId, entityType, entityId, action, old, new)` |
| `IEncryptionService` | AES `Encrypt/Decrypt` |
| `IPasswordService` | bcrypt `Hash/Verify` |
| `IPaymentService` | `ProcessPaymentAsync`, `ProcessRefundAsync` |
| `IProviderReservationService` | `CreateBookingAsync`, `CancelBookingAsync` against providers-api |
| `IReservationApiClient` | HTTP delegation to reservations-api (full reservation CRUD) |
| `IScheduledApiClient` | `TriggerJobAsync`, `ReloadSchedulesAsync` against scheduled-api |
| `IProvidersApiClient` | `GetHotelAvailabilityAsync` against providers-api |
| `IEcbRateParser` | Parse ECB CSV → exchange rates |
| `ICurrencyConverter` | Convert amounts between currencies via EUR base |
| `ITranslationService` | Resolve translations with caching |

### Features

546 files organized by API consumer and domain area.

#### Features/Admin/

Full CRUD for every entity. 37+ domains, each with `Commands/`, `Queries/`, `Dtos/`, `Validators/`.

| Domain | Commands | Queries |
|--------|----------|---------|
| Hotels | Create, Update, Delete, SetEnabled | GetAll, GetById |
| Providers | Create, Update, Delete, SetEnabled | GetAll, GetById |
| Reservations | SetReservationStatus | GetAll, GetById, GetLines, GetGuests |
| Users | Create, Update, Delete, SetEnabled | GetAll, GetById |
| BusinessPartners | Create, Update, Delete, SetEnabled | GetAll, GetById |
| SubscriptionTypes | Create, Update, Delete, SetEnabled | GetAll, GetById |
| PromoCodes | Create, Update, Delete, SetEnabled | GetAll, GetById |
| Translations | Create, Update, Delete | GetAll, GetById |
| WebTranslations | Create, Update, Delete | GetAll, GetById, GetBatch |
| JobSchedules | UpdateJobSchedule | GetAll, GetByKey |
| Statistics | — | 14 queries (BookingVolume, Revenue, Occupancy, UserGrowth, etc.) |
| *(+ 26 more domains)* | Full CRUD pattern | GetAll, GetById |

Read-only domains (no commands): AuditLogs, Cancellations, InvoiceStatuses, NotificationLogs, PaymentTransactions, PaymentTransactionFees, ReservationStatuses, UserTypes.

#### Features/Client/

Scoped to what end-users need:

| Domain | Use Cases |
|--------|-----------|
| Auth | Login, Register, ForgotPassword, ResetPassword |
| Hotels | SearchHotels, GetHotelDetail, GetRoomAvailability, GetHotelReviews, GetCancellationPolicy |
| Reservations | CreateDraft, AddLine, RemoveLine, AddGuest, Submit, Cancel, GetMyReservations, GetDetail |
| Invoices | GetMyInvoices, GetInvoiceDetail |
| Profile | GetProfile, UpdateProfile, ChangePassword |
| Reviews | SubmitReview |
| Subscriptions | GetPlans, GetMySubscription, Subscribe |
| PromoCodes | ValidatePromoCode |
| Reference | GetCountries, GetCurrencies, GetLanguages, GetWebTranslations |

Reservation handlers delegate to `IReservationApiClient` — client-api never touches the reservation DB directly.

#### Features/ExternalClient/

B2B partner API — mirrors client reservation flow with token auth:

| Domain | Use Cases |
|--------|-----------|
| Auth | Login (24h token) |
| Hotels | SearchHotels, GetHotelDetail, GetAvailability |
| Reservations | CreatePartnerDraft, AddLine, RemoveLine, AddGuest, Submit, Cancel, GetAll, GetById |

#### Features/Scheduled/

| Domain | Use Cases |
|--------|-----------|
| ExchangeRateSync | `SyncExchangeRatesCommand` — fetches ECB CSV, computes `rate_to_eur = 1/ecb_value`, closes old rates, inserts new |

---

## Data

Depends on: **Domain**, **Application**

Packages: Pomelo.EntityFrameworkCore.MySql 9.0, Dapper 2.1, MySqlConnector 2.4

### DI Registration

```csharp
// Called by APIs that access the DB directly
builder.Services.AddDataServices(connectionString);
```

Registers: `AppDbContext`, `IUnitOfWork`, `IHotelRepository`, `IReservationRepository`, `IUserRepository`, generic `IRepository<>`, `IDbConnectionFactory`.

### AppDbContext

`AppDbContext : DbContext, IUnitOfWork` — 51 DbSets, one per entity. Uses `ApplyConfigurationsFromAssembly` to load all `IEntityTypeConfiguration<T>` classes from `Configurations/`.

Each configuration maps to snake_case table and column names matching the MariaDB schema.

### Repositories

| Class | Interface | Notes |
|-------|-----------|-------|
| `Repository<T>` | `IRepository<T>` | Generic EF Core CRUD |
| `SimpleRepository<T>` | `ISimpleRepository<T>` | For entities without `BaseEntity` base |
| `HotelRepository` | `IHotelRepository` | Eager loads City, Images, Amenities, Providers, RoomTypes |
| `ReservationRepository` | `IReservationRepository` | Eager loads Lines, Guests |
| `UserRepository` | `IUserRepository` | Lookup by email |
| `DapperConnectionFactory` | `IDbConnectionFactory` | Returns `MySqlConnection` from config |

---

## Infrastructure

Depends on: **Domain**, **Application**

Packages: StackExchange.Redis 2.8, BCrypt.Net-Next 4.0, System.IdentityModel.Tokens.Jwt 8.3

### DI Registration

```csharp
// Called by APIs that need infra services
builder.Services.AddInfrastructureServices(redisConnectionString);
```

Registers all service implementations below, plus HTTP clients for inter-service communication.

### Services

| Folder | Class | Interface | Description |
|--------|-------|-----------|-------------|
| `Cache/` | `RedisCacheService` | `ICacheService` | JSON to Redis strings; prefix-based invalidation |
| `Security/` | `JwtTokenService` | `IJwtTokenService` | HS256 JWT with configurable expiry |
| `Security/` | `PasswordService` | `IPasswordService` | bcrypt work factor 10 |
| `Security/` | `EncryptionService` | `IEncryptionService` | AES-256-CBC, IV prepended to ciphertext |
| `Audit/` | `AuditService` | `IAuditService` | Writes `AuditLog` via EF Core |
| `Email/` | `EmailService` | `IEmailService` | **Stub** — logs only, SMTP/SendGrid TBD |
| `Payment/` | `PaymentService` | `IPaymentService` | **Stub** — simulated delays, fake references |
| `Translations/` | `TranslationService` | `ITranslationService` | Dapper + Redis cache (1h TTL) |
| `Currency/` | `CurrencyConverter` | `ICurrencyConverter` | Two-step via EUR base rate |
| `Services/` | `EcbRateParser` | `IEcbRateParser` | Parse ECB CSV, `rate_to_eur = 1/ecb_value` |

### HTTP Clients (inter-service)

| Class | Interface | Target | Pattern |
|-------|-----------|--------|---------|
| `ReservationApiClient` | `IReservationApiClient` | reservations-api (5004) | Named client `"ReservationsApi"` |
| `ScheduledApiClient` | `IScheduledApiClient` | scheduled-api (5006) | Named client `"ScheduledApi"` |
| `ProvidersApiClient` | `IProvidersApiClient` | providers-api (5003) | Named client `"ProvidersApi"` |
| `ProviderReservationApiClient` | `IProviderReservationService` | providers-api (5003) | Typed client |

---

## How APIs Consume the Kernel

Each API is a thin controller layer. Typical `Program.cs`:

```csharp
builder.Services.AddApplicationServices();                    // MediatR + validators
builder.Services.AddDataServices(connectionString);           // EF Core + repos
builder.Services.AddInfrastructureServices(redisConnectionString); // Redis + services

var app = builder.Build();
app.MapControllers();
```

Controllers inject `IMediator` and dispatch commands/queries:

```csharp
[HttpPost]
public async Task<IActionResult> Create(CreateHotelCommand command)
{
    var result = await _mediator.Send(command);
    return Ok(result);
}
```

### API → Feature Mapping

| API | Feature Area | Notes |
|-----|-------------|-------|
| admin-api (5001) | `Features/Admin/*` | Full CRUD + statistics |
| client-api (5002) | `Features/Client/*` | Reservation ops delegate via `IReservationApiClient` |
| external-client-api (5005) | `Features/ExternalClient/*` | B2B token auth, reservation ops delegate |
| reservations-api (5004) | Reservation entities directly | Central booking engine, DB access |
| scheduled-api (5006) | `Features/Scheduled/*` | Hangfire jobs |
| providers-api (5003) | Own codebase (not kernel) | Standalone with Dapper, no kernel reference |
