# Tests

Unit and integration tests for ViajesAltairis API microservices.

## Stack

| Package | Version | Purpose |
|---------|---------|---------|
| xUnit | 2.9.2 | Test framework |
| xunit.runner.visualstudio | 2.8.2 | VS / CLI test runner |
| Microsoft.NET.Test.Sdk | 17.12.0 | Test host |
| FluentAssertions | 8.2.0 | Readable assertions |
| NSubstitute | 5.3.0 | Mocking |
| coverlet.collector | 6.0.2 | Code coverage |
| Microsoft.AspNetCore.Mvc.Testing | 9.0.0 | `WebApplicationFactory` integration tests |
| Microsoft.EntityFrameworkCore.Sqlite | 9.0.0 | In-memory DB for admin-api tests |
| Microsoft.Data.Sqlite | 9.0.0 | In-memory DB for client/external-client tests |
| Hangfire.InMemory | 1.0.* | In-memory Hangfire for scheduled-api tests |

Shared package versions and global usings are centralized in `Directory.Build.props`.

## Test Coverage

| Project | Tests | Type | Scope |
|---------|------:|------|-------|
| Admin.Api.Tests | 269 | Integration | 37 CRUD controllers, role-based auth (4 roles), statistics (14 endpoints) |
| Client.Api.Tests | 109 | Unit + Integration | 9 controllers, 30 handlers (auth, hotels, profile, subscriptions, promos, reservations, reviews, invoices) |
| Reservations.Api.Tests | 51 | Integration | Full booking flow (draft, lines, guests, submit, cancel), invoices |
| ExternalClient.Api.Tests | 51 | Unit | B2B auth, hotel search, 7 reservation handlers, 4 validators |
| Scheduled.Api.Tests | 31 | Unit | 3 Hangfire jobs, ECB parser, handler, validator, 2 controllers |
| Providers.Api.Tests | 20 | Integration | Providers, hotels, room types, external ops, sync endpoints |
| **Total** | **531** | | |

## Solution Structure

```
ViajesAltairis.Tests.sln
├── Directory.Build.props                           # Shared packages, TFM, global usings
│
├── ViajesAltairis.Admin.Api.Tests/                 # admin-api (port 5001)
│   ├── Infrastructure/                             # Test fixtures and helpers
│   │   ├── AdminApiFactory.cs                      # WebApplicationFactory + SQLite setup
│   │   ├── AdminApiCollection.cs                   # xUnit collection definition
│   │   ├── SqliteDbConnectionFactory.cs            # Dapper-compatible SQLite connections
│   │   ├── SqliteTypeHandlers.cs                   # Dapper type handlers for SQLite
│   │   ├── FakePasswordService.cs                  # BCrypt bypass for tests
│   │   ├── FakeCacheService.cs                     # Redis bypass for tests
│   │   └── TestAuthHelper.cs                       # JWT token generation for tests
│   └── Features/                                   # One folder per admin feature
│       ├── Auth/                                   # Login, role checks
│       ├── Hotels/, Providers/, Users/ ...         # CRUD controller tests
│       └── Statistics/                             # Dashboard statistics
│
├── ViajesAltairis.Client.Api.Tests/                # client-api (port 5002)
│   ├── Fixtures/                                   # Test infrastructure
│   │   ├── ClientApiFactory.cs                     # WebApplicationFactory + SQLite setup
│   │   └── TestDbHelper.cs                         # Seed data helpers
│   ├── Helpers/                                    # Auth + HTTP utilities
│   │   ├── AuthHelper.cs                           # JWT token generation
│   │   └── HttpClientExtensions.cs                 # Authenticated request helpers
│   ├── Handlers/                                   # Unit tests per feature area
│   │   ├── Auth/                                   # Login, Register, ForgotPassword, ResetPassword
│   │   ├── Reference/                              # Languages, Currencies, WebTranslations
│   │   ├── Hotels/                                 # Reviews, CancellationPolicy
│   │   ├── Profile/                                # GetProfile, UpdateProfile, ChangePassword
│   │   ├── Subscriptions/                          # GetMySubscription, Subscribe
│   │   ├── PromoCodes/                             # ValidatePromoCode
│   │   ├── Reservations/                           # Delegation to reservations-api
│   │   ├── Reviews/                                # SubmitReview
│   │   └── Invoices/                               # Delegation to reservations-api
│   └── Controllers/                                # Integration tests (HTTP pipeline)
│       ├── AuthControllerTests.cs
│       ├── HotelsControllerTests.cs
│       ├── ProfileControllerTests.cs
│       ├── ReferenceControllerTests.cs
│       ├── SubscriptionsControllerTests.cs
│       ├── PromoCodesControllerTests.cs
│       ├── ReservationsControllerTests.cs
│       ├── ReviewsControllerTests.cs
│       └── InvoicesControllerTests.cs
│
├── ViajesAltairis.Providers.Api.Tests/             # providers-api (port 5003)
│   ├── Fixtures/
│   │   └── ProvidersApiFactory.cs                  # WebApplicationFactory
│   └── Tests/
│       ├── ProvidersEndpointTests.cs               # Provider CRUD
│       ├── HotelsEndpointTests.cs                  # Hotel management
│       ├── RoomTypesEndpointTests.cs               # Room configuration
│       ├── ExternalOperationsTests.cs              # External provider sync
│       └── SyncEndpointTests.cs                    # Data sync endpoints
│
├── ViajesAltairis.Reservations.Api.Tests/          # reservations-api (port 5004)
│   ├── Fixtures/
│   │   └── CustomWebApplicationFactory.cs          # WebApplicationFactory
│   ├── Helpers/
│   │   ├── DateOnlyTypeHandler.cs                  # Dapper DateOnly support
│   │   └── DapperMockHelper.cs                     # Dapper mocking utilities
│   ├── IntegrationTestBase.cs                      # Base class for integration tests
│   ├── CreateDraftReservationTests.cs              # Draft creation
│   ├── AddReservationLineTests.cs                  # Add rooms to draft
│   ├── AddReservationGuestTests.cs                 # Add guests to line
│   ├── RemoveReservationLineTests.cs               # Remove rooms from draft
│   ├── SubmitReservationTests.cs                   # Draft → Pending
│   ├── CancelReservationTests.cs                   # Cancellation flow
│   ├── GetReservationTests.cs                      # Reservation queries
│   ├── GetReservationLineInfoTests.cs              # Line detail queries
│   └── InvoiceTests.cs                             # Invoice generation
│
├── ViajesAltairis.ExternalClient.Api.Tests/        # external-client-api (port 5005)
│   ├── Auth/
│   │   └── LoginHandlerTests.cs                    # B2B token auth
│   ├── Hotels/
│   │   └── GetAvailabilityHandlerTests.cs          # Availability queries
│   ├── Reservations/                               # B2B booking flow
│   │   ├── CreatePartnerDraftHandlerTests.cs
│   │   ├── AddPartnerLineHandlerTests.cs
│   │   ├── RemovePartnerLineHandlerTests.cs
│   │   ├── AddPartnerGuestHandlerTests.cs
│   │   ├── SubmitPartnerReservationHandlerTests.cs
│   │   ├── CancelPartnerReservationHandlerTests.cs
│   │   └── GetPartnerReservationsHandlerTests.cs
│   └── Validators/                                 # FluentValidation rule tests
│       ├── CreatePartnerDraftValidatorTests.cs
│       ├── AddPartnerLineValidatorTests.cs
│       ├── SubmitPartnerReservationValidatorTests.cs
│       └── CancelPartnerReservationValidatorTests.cs
│
└── ViajesAltairis.Scheduled.Api.Tests/             # scheduled-api (port 5006)
    ├── Jobs/                                       # Hangfire job tests
    │   ├── ExchangeRateSyncJobTests.cs             # ECB rate sync
    │   ├── SubscriptionBillingJobTests.cs          # Subscription renewal
    │   └── ProviderUpdateJobTests.cs               # Provider data sync
    ├── Handlers/
    │   └── SyncExchangeRatesHandlerTests.cs        # MediatR command handler
    ├── Validators/
    │   └── UpdateJobScheduleValidatorTests.cs      # Cron expression validation
    ├── Services/
    │   ├── EcbRateParserTests.cs                   # ECB CSV parsing
    │   └── ScheduledApiClientTests.cs              # HTTP client to scheduled-api
    └── Controllers/
        ├── JobsControllerTests.cs                  # Job trigger/reload endpoints
        └── HealthControllerTests.cs                # Health check endpoint
```

## Running Tests

```bash
# All tests
dotnet test docker/tests/project/ViajesAltairis.Tests.sln

# Single project
dotnet test docker/tests/project/ViajesAltairis.Client.Api.Tests

# With coverage
dotnet test docker/tests/project/ViajesAltairis.Tests.sln --collect:"XPlat Code Coverage"

# Specific test class
dotnet test docker/tests/project/ViajesAltairis.Tests.sln --filter "FullyQualifiedName~LoginHandlerTests"
```

## Test Patterns

### Unit Tests (Handlers, Validators, Services)

Mock dependencies with NSubstitute, test business logic in isolation:

```csharp
var repo = Substitute.For<IRepository<User>>();
repo.GetByIdAsync(1).Returns(testUser);
var handler = new GetProfileHandler(repo);

var result = await handler.Handle(new GetProfileQuery(1), CancellationToken.None);

result.Email.Should().Be("test@example.com");
```

### Integration Tests (Controllers)

Use `WebApplicationFactory` with SQLite to test the full HTTP pipeline:

```csharp
public class AuthControllerTests : IClassFixture<ClientApiFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(ClientApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", credentials);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

### Delegation Tests (Client → Reservations)

Verify that client-api correctly delegates to reservations-api via `IReservationApiClient`:

```csharp
var apiClient = Substitute.For<IReservationApiClient>();
apiClient.CreateDraftAsync(Arg.Any<CreateDraftRequest>())
    .Returns(new DraftResponse { Id = 1 });

// Verify the handler delegates without direct DB access
```

## Project References

Each test project references its corresponding API and the kernel shared projects. References resolve within the Docker build context (`./docker` root) and via the kernel solution locally.

| Test Project | References |
|-------------|------------|
| Admin.Api.Tests | Kernel (Domain, Application, Data, Infrastructure) + AdminApi |
| Client.Api.Tests | Kernel (Domain, Application, Data, Infrastructure) + ClientApi |
| Providers.Api.Tests | ProvidersApi (includes kernel transitively) |
| Reservations.Api.Tests | Kernel (Domain, Application, Data, Infrastructure) + ReservationsApi |
| ExternalClient.Api.Tests | Kernel (Domain, Application) + ExternalClient.Api |
| Scheduled.Api.Tests | Kernel (Domain, Application, Data, Infrastructure) + ScheduledApi |
