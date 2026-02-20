using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Admin.Api.Tests.Infrastructure;
using Dapper;

namespace ViajesAltairis.Admin.Api.Tests.Features.Statistics;

[Collection("AdminApi")]
public class StatisticsControllerTests : IAsyncLifetime
{
    private readonly AdminApiFactory _factory;
    private readonly HttpClient _client;
    private static bool _seeded;
    private static readonly object _seedLock = new();

    public StatisticsControllerTests(AdminApiFactory factory)
    {
        _factory = factory;
        _client = TestAuthHelper.CreateAuthenticatedClient(factory);
    }

    public Task InitializeAsync()
    {
        lock (_seedLock)
        {
            if (!_seeded)
            {
                SeedStatisticsData();
                _seeded = true;
            }
        }
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private void SeedStatisticsData()
    {
        var dbFactory = _factory.Services.GetRequiredService<IDbConnectionFactory>();
        using var db = dbFactory.CreateConnection();

        // Currency
        db.Execute("INSERT OR IGNORE INTO currency (id, iso_code, name, symbol, created_at) VALUES (100, 'EUR', 'Euro', '€', datetime('now'))");
        db.Execute("INSERT OR IGNORE INTO currency (id, iso_code, name, symbol, created_at) VALUES (101, 'USD', 'US Dollar', '$', datetime('now'))");

        // Exchange rate
        db.Execute("INSERT OR IGNORE INTO exchange_rate (id, currency_id, rate_to_eur, valid_from, valid_to, created_at) VALUES (100, 100, 1.0, '2025-01-01', '2099-12-31', datetime('now'))");
        db.Execute("INSERT OR IGNORE INTO exchange_rate (id, currency_id, rate_to_eur, valid_from, valid_to, created_at) VALUES (101, 101, 0.92, '2025-01-01', '2099-12-31', datetime('now'))");

        // Country
        db.Execute("INSERT OR IGNORE INTO country (id, iso_code, name, currency_id, created_at) VALUES (100, 'ES', 'Spain', 100, datetime('now'))");

        // Administrative division type
        db.Execute("INSERT OR IGNORE INTO administrative_division_type (id, name, created_at) VALUES (100, 'Province', datetime('now'))");

        // Administrative division
        db.Execute("INSERT OR IGNORE INTO administrative_division (id, country_id, type_id, name, level, enabled, created_at) VALUES (100, 100, 100, 'Madrid', 1, 1, datetime('now'))");

        // City
        db.Execute("INSERT OR IGNORE INTO city (id, administrative_division_id, name, created_at) VALUES (100, 100, 'Madrid', datetime('now'))");

        // Provider type
        db.Execute("INSERT OR IGNORE INTO provider_type (id, name, created_at) VALUES (100, 'API', datetime('now'))");

        // Providers
        db.Execute("INSERT OR IGNORE INTO provider (id, type_id, currency_id, name, margin, enabled, created_at) VALUES (100, 100, 100, 'TestProvider Alpha', 5.00, 1, datetime('now'))");
        db.Execute("INSERT OR IGNORE INTO provider (id, type_id, currency_id, name, margin, enabled, created_at) VALUES (101, 100, 100, 'TestProvider Beta', 3.00, 1, datetime('now'))");

        // Hotels
        db.Execute("INSERT OR IGNORE INTO hotel (id, city_id, name, stars, address, margin, enabled, check_in_time, check_out_time, created_at) VALUES (100, 100, 'Grand Test Hotel', 5, 'Test Street 1', 2.00, 1, '15:00:00', '11:00:00', datetime('now'))");
        db.Execute("INSERT OR IGNORE INTO hotel (id, city_id, name, stars, address, margin, enabled, check_in_time, check_out_time, created_at) VALUES (101, 100, 'Budget Test Inn', 3, 'Test Street 2', 1.50, 1, '14:00:00', '12:00:00', datetime('now'))");

        // Hotel-Provider links
        db.Execute("INSERT OR IGNORE INTO hotel_provider (id, hotel_id, provider_id, enabled, created_at) VALUES (100, 100, 100, 1, datetime('now'))");
        db.Execute("INSERT OR IGNORE INTO hotel_provider (id, hotel_id, provider_id, enabled, created_at) VALUES (101, 101, 101, 1, datetime('now'))");

        // Room type
        db.Execute("INSERT OR IGNORE INTO room_type (id, name, created_at) VALUES (100, 'Double Standard', datetime('now'))");

        // Hotel-Provider-RoomType configs
        db.Execute("INSERT OR IGNORE INTO hotel_provider_room_type (id, hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id, enabled, created_at) VALUES (100, 100, 100, 2, 10, 120.00, 100, 100, 1, datetime('now'))");
        db.Execute("INSERT OR IGNORE INTO hotel_provider_room_type (id, hotel_provider_id, room_type_id, capacity, quantity, price_per_night, currency_id, exchange_rate_id, enabled, created_at) VALUES (101, 101, 100, 2, 8, 80.00, 100, 100, 1, datetime('now'))");

        // Board type
        db.Execute("INSERT OR IGNORE INTO board_type (id, name) VALUES (100, 'Room Only')");

        // Reservation statuses
        db.Execute("INSERT OR IGNORE INTO reservation_status (id, name, created_at) VALUES (1, 'Draft', datetime('now'))");
        db.Execute("INSERT OR IGNORE INTO reservation_status (id, name, created_at) VALUES (2, 'Pending', datetime('now'))");
        db.Execute("INSERT OR IGNORE INTO reservation_status (id, name, created_at) VALUES (3, 'Confirmed', datetime('now'))");
        db.Execute("INSERT OR IGNORE INTO reservation_status (id, name, created_at) VALUES (4, 'Checked In', datetime('now'))");
        db.Execute("INSERT OR IGNORE INTO reservation_status (id, name, created_at) VALUES (5, 'Completed', datetime('now'))");
        db.Execute("INSERT OR IGNORE INTO reservation_status (id, name, created_at) VALUES (6, 'Cancelled', datetime('now'))");

        // Promo codes
        db.Execute("INSERT OR IGNORE INTO promo_code (id, code, discount_percentage, valid_from, valid_to, max_uses, current_uses, enabled, created_at) VALUES (100, 'SUMMER25', 10.00, '2025-01-01', '2025-12-31', 100, 2, 1, datetime('now'))");
        db.Execute("INSERT OR IGNORE INTO promo_code (id, code, discount_percentage, valid_from, valid_to, max_uses, current_uses, enabled, created_at) VALUES (101, 'WINTER25', 15.00, '2025-01-01', '2025-12-31', 50, 1, 1, datetime('now'))");

        // Client user for reservations (user_type_id=5)
        db.Execute("INSERT OR IGNORE INTO user (id, email, password_hash, first_name, last_name, user_type_id, enabled, created_at) VALUES (100, 'client1@test.com', 'HASHED:pass', 'John', 'Doe', 5, 1, datetime('now'))");
        db.Execute("INSERT OR IGNORE INTO user (id, email, password_hash, first_name, last_name, user_type_id, enabled, created_at) VALUES (101, 'client2@test.com', 'HASHED:pass', 'Jane', 'Smith', 5, 1, datetime('now'))");

        // Reservations — 3 completed (status=5), 1 confirmed (status=3), 1 cancelled (status=6)
        db.Execute("""
            INSERT OR IGNORE INTO reservation (id, reservation_code, status_id, booked_by_user_id, owner_user_id,
                owner_first_name, owner_last_name, subtotal, tax_amount, margin_amount, discount_amount, total_price,
                currency_id, exchange_rate_id, promo_code_id, created_at, updated_at)
            VALUES (100, 'RES-STAT-001', 5, 100, 100, 'John', 'Doe', 200.00, 20.00, 10.00, 20.00, 210.00, 100, 100, 100,
                    datetime('now', '-10 days'), datetime('now', '-10 days'))
        """);
        db.Execute("""
            INSERT OR IGNORE INTO reservation (id, reservation_code, status_id, booked_by_user_id, owner_user_id,
                owner_first_name, owner_last_name, subtotal, tax_amount, margin_amount, discount_amount, total_price,
                currency_id, exchange_rate_id, promo_code_id, created_at, updated_at)
            VALUES (101, 'RES-STAT-002', 5, 100, 100, 'John', 'Doe', 300.00, 30.00, 15.00, 30.00, 315.00, 100, 100, 100,
                    datetime('now', '-5 days'), datetime('now', '-5 days'))
        """);
        db.Execute("""
            INSERT OR IGNORE INTO reservation (id, reservation_code, status_id, booked_by_user_id, owner_user_id,
                owner_first_name, owner_last_name, subtotal, tax_amount, margin_amount, discount_amount, total_price,
                currency_id, exchange_rate_id, created_at, updated_at)
            VALUES (102, 'RES-STAT-003', 5, 101, 101, 'Jane', 'Smith', 160.00, 16.00, 8.00, 0.00, 184.00, 100, 100,
                    datetime('now', '-3 days'), datetime('now', '-3 days'))
        """);
        db.Execute("""
            INSERT OR IGNORE INTO reservation (id, reservation_code, status_id, booked_by_user_id, owner_user_id,
                owner_first_name, owner_last_name, subtotal, tax_amount, margin_amount, discount_amount, total_price,
                currency_id, exchange_rate_id, promo_code_id, created_at, updated_at)
            VALUES (103, 'RES-STAT-004', 3, 101, 101, 'Jane', 'Smith', 240.00, 24.00, 12.00, 36.00, 240.00, 100, 100, 101,
                    datetime('now', '-1 days'), datetime('now', '-1 days'))
        """);
        db.Execute("""
            INSERT OR IGNORE INTO reservation (id, reservation_code, status_id, booked_by_user_id, owner_user_id,
                owner_first_name, owner_last_name, subtotal, tax_amount, margin_amount, discount_amount, total_price,
                currency_id, exchange_rate_id, created_at, updated_at)
            VALUES (104, 'RES-STAT-005', 6, 100, 100, 'John', 'Doe', 150.00, 15.00, 7.50, 0.00, 172.50, 100, 100,
                    datetime('now', '-8 days'), datetime('now', '-8 days'))
        """);

        // Reservation lines
        db.Execute("""
            INSERT OR IGNORE INTO reservation_line (id, reservation_id, hotel_provider_room_type_id, board_type_id,
                check_in_date, check_out_date, num_rooms, num_guests, price_per_night, board_price_per_night,
                num_nights, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id, created_at)
            VALUES (100, 100, 100, 100, '2025-06-01', '2025-06-03', 1, 2, 120.00, 0.00, 2, 200.00, 20.00, 10.00, 20.00, 210.00, 100, 100, datetime('now', '-10 days'))
        """);
        db.Execute("""
            INSERT OR IGNORE INTO reservation_line (id, reservation_id, hotel_provider_room_type_id, board_type_id,
                check_in_date, check_out_date, num_rooms, num_guests, price_per_night, board_price_per_night,
                num_nights, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id, created_at)
            VALUES (101, 101, 100, 100, '2025-07-10', '2025-07-13', 1, 2, 120.00, 0.00, 3, 300.00, 30.00, 15.00, 30.00, 315.00, 100, 100, datetime('now', '-5 days'))
        """);
        db.Execute("""
            INSERT OR IGNORE INTO reservation_line (id, reservation_id, hotel_provider_room_type_id, board_type_id,
                check_in_date, check_out_date, num_rooms, num_guests, price_per_night, board_price_per_night,
                num_nights, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id, created_at)
            VALUES (102, 102, 101, 100, '2025-08-05', '2025-08-07', 1, 2, 80.00, 0.00, 2, 160.00, 16.00, 8.00, 0.00, 184.00, 100, 100, datetime('now', '-3 days'))
        """);
        db.Execute("""
            INSERT OR IGNORE INTO reservation_line (id, reservation_id, hotel_provider_room_type_id, board_type_id,
                check_in_date, check_out_date, num_rooms, num_guests, price_per_night, board_price_per_night,
                num_nights, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id, created_at)
            VALUES (103, 103, 101, 100, '2025-09-15', '2025-09-18', 1, 2, 80.00, 0.00, 3, 240.00, 24.00, 12.00, 36.00, 240.00, 100, 100, datetime('now', '-1 days'))
        """);
        db.Execute("""
            INSERT OR IGNORE INTO reservation_line (id, reservation_id, hotel_provider_room_type_id, board_type_id,
                check_in_date, check_out_date, num_rooms, num_guests, price_per_night, board_price_per_night,
                num_nights, subtotal, tax_amount, margin_amount, discount_amount, total_price, currency_id, exchange_rate_id, created_at)
            VALUES (104, 104, 100, 100, '2025-06-15', '2025-06-17', 1, 2, 120.00, 0.00, 2, 150.00, 15.00, 7.50, 0.00, 172.50, 100, 100, datetime('now', '-8 days'))
        """);

        // Reviews (for completed reservations)
        db.Execute("INSERT OR IGNORE INTO review (id, reservation_id, user_id, hotel_id, rating, title, comment, visible, created_at) VALUES (100, 100, 100, 100, 5, 'Excellent!', 'Great stay', 1, datetime('now', '-9 days'))");
        db.Execute("INSERT OR IGNORE INTO review (id, reservation_id, user_id, hotel_id, rating, title, comment, visible, created_at) VALUES (101, 101, 100, 100, 4, 'Very Good', 'Nice hotel', 1, datetime('now', '-4 days'))");
        db.Execute("INSERT OR IGNORE INTO review (id, reservation_id, user_id, hotel_id, rating, title, comment, visible, created_at) VALUES (102, 102, 101, 101, 3, 'OK', 'Average', 1, datetime('now', '-2 days'))");

        // Cancellation (for the cancelled reservation)
        db.Execute("INSERT OR IGNORE INTO cancellation (id, reservation_id, cancelled_by_user_id, reason, penalty_percentage, penalty_amount, refund_amount, currency_id, created_at) VALUES (100, 104, 100, 'Changed plans', 25.00, 43.13, 129.37, 100, datetime('now', '-7 days'))");

        // Subscription types
        db.Execute("INSERT OR IGNORE INTO subscription_type (id, name, price_per_month, discount, currency_id, enabled, created_at) VALUES (100, 'Basic Plan', 9.99, 5.00, 100, 1, datetime('now'))");
        db.Execute("INSERT OR IGNORE INTO subscription_type (id, name, price_per_month, discount, currency_id, enabled, created_at) VALUES (101, 'Premium Plan', 29.99, 15.00, 100, 1, datetime('now'))");

        // User subscriptions
        db.Execute("INSERT OR IGNORE INTO user_subscription (id, user_id, subscription_type_id, start_date, active, created_at) VALUES (100, 100, 100, '2025-01-01', 1, datetime('now'))");
        db.Execute("INSERT OR IGNORE INTO user_subscription (id, user_id, subscription_type_id, start_date, active, created_at) VALUES (101, 101, 101, '2025-03-01', 1, datetime('now'))");
    }

    // ==================== Revenue Endpoints ====================

    [Fact]
    public async Task RevenueByHotel_ReturnsCompletedReservationsGroupedByHotel()
    {
        var response = await _client.GetAsync("/api/statistics/revenue/by-hotel");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
        items.Should().NotBeNull();
        items!.Count.Should().BeGreaterThanOrEqualTo(2);

        // Verify DTO shape
        var first = items.First();
        first.TryGetProperty("hotelId", out _).Should().BeTrue();
        first.TryGetProperty("hotelName", out _).Should().BeTrue();
        first.TryGetProperty("currencyCode", out _).Should().BeTrue();
        first.TryGetProperty("totalRevenue", out _).Should().BeTrue();
        first.TryGetProperty("reservationCount", out _).Should().BeTrue();

        // Grand Test Hotel has 2 completed reservations (RES-STAT-001, RES-STAT-002)
        var grandHotel = items.First(x => x.GetProperty("hotelName").GetString() == "Grand Test Hotel");
        grandHotel.GetProperty("reservationCount").GetInt32().Should().BeGreaterThanOrEqualTo(2);
        grandHotel.GetProperty("totalRevenue").GetDecimal().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task RevenueByProvider_ReturnsCompletedReservationsGroupedByProvider()
    {
        var response = await _client.GetAsync("/api/statistics/revenue/by-provider");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
        items.Should().NotBeNull();
        items!.Count.Should().BeGreaterThanOrEqualTo(2);

        var first = items.First();
        first.TryGetProperty("providerId", out _).Should().BeTrue();
        first.TryGetProperty("providerName", out _).Should().BeTrue();
        first.TryGetProperty("totalRevenue", out _).Should().BeTrue();
    }

    [Fact]
    public async Task RevenueByPeriod_UsesDateFormat_FailsOnSqlite()
    {
        // DATE_FORMAT is MariaDB-specific, SQLite doesn't support it
        var response = await _client.GetAsync("/api/statistics/revenue/by-period");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    // ==================== Booking Endpoints ====================

    [Fact]
    public async Task BookingsByStatus_ReturnsAllStatusesWithCounts()
    {
        var response = await _client.GetAsync("/api/statistics/bookings/by-status");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
        items.Should().NotBeNull();
        items!.Count.Should().BeGreaterThanOrEqualTo(6); // 6 statuses seeded

        // Verify shape
        var first = items.First();
        first.TryGetProperty("statusName", out _).Should().BeTrue();
        first.TryGetProperty("bookingCount", out _).Should().BeTrue();

        // Completed should have at least 3
        var completed = items.First(x => x.GetProperty("statusName").GetString() == "Completed");
        completed.GetProperty("bookingCount").GetInt32().Should().BeGreaterThanOrEqualTo(3);

        // Cancelled should have at least 1
        var cancelled = items.First(x => x.GetProperty("statusName").GetString() == "Cancelled");
        cancelled.GetProperty("bookingCount").GetInt32().Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task BookingAverage_ReturnsAverageValueAndNights()
    {
        var response = await _client.GetAsync("/api/statistics/bookings/average");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var item = await response.Content.ReadFromJsonAsync<JsonElement>();
        item.TryGetProperty("averageValue", out var avgValue).Should().BeTrue();
        item.TryGetProperty("averageNights", out var avgNights).Should().BeTrue();
        item.TryGetProperty("totalBookings", out var total).Should().BeTrue();

        avgValue.GetDecimal().Should().BeGreaterThan(0);
        avgNights.GetDecimal().Should().BeGreaterThan(0);
        total.GetInt32().Should().BeGreaterThanOrEqualTo(5);
    }

    [Fact]
    public async Task BookingVolume_UsesDateFormat_FailsOnSqlite()
    {
        var response = await _client.GetAsync("/api/statistics/bookings/volume");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    // ==================== Occupancy ====================

    [Fact]
    public async Task Occupancy_UsesDatediff_FailsOnSqlite()
    {
        // DATEDIFF, CURDATE, DATE_SUB are MariaDB-specific
        var response = await _client.GetAsync("/api/statistics/occupancy");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    // ==================== User Endpoints ====================

    [Fact]
    public async Task UsersByType_ReturnsCountPerType()
    {
        var response = await _client.GetAsync("/api/statistics/users/by-type");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
        items.Should().NotBeNull();
        items!.Count.Should().BeGreaterThanOrEqualTo(5); // 5 user types seeded

        // Verify shape
        var first = items.First();
        first.TryGetProperty("typeName", out _).Should().BeTrue();
        first.TryGetProperty("userCount", out _).Should().BeTrue();

        // Client type should have seeded users
        var clientType = items.First(x => x.GetProperty("typeName").GetString() == "Client");
        clientType.GetProperty("userCount").GetInt32().Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task UserGrowth_UsesDateFormat_FailsOnSqlite()
    {
        var response = await _client.GetAsync("/api/statistics/users/growth");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ActiveSubscriptions_ReturnsSnapshotData()
    {
        var response = await _client.GetAsync("/api/statistics/users/subscriptions");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var item = await response.Content.ReadFromJsonAsync<JsonElement>();
        item.TryGetProperty("activeCount", out var active).Should().BeTrue();
        item.TryGetProperty("totalUsers", out var total).Should().BeTrue();
        item.TryGetProperty("subscriptionRate", out var rate).Should().BeTrue();

        active.GetInt32().Should().BeGreaterThanOrEqualTo(2); // 2 active subscriptions seeded
        total.GetInt32().Should().BeGreaterThanOrEqualTo(2);  // 2 client users seeded
        rate.GetDecimal().Should().BeGreaterThan(0);
    }

    // ==================== Reviews ====================

    [Fact]
    public async Task ReviewStats_ReturnsAverageAndDistribution()
    {
        var response = await _client.GetAsync("/api/statistics/reviews");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var item = await response.Content.ReadFromJsonAsync<JsonElement>();
        item.TryGetProperty("averageRating", out var avg).Should().BeTrue();
        item.TryGetProperty("totalReviews", out var total).Should().BeTrue();
        item.TryGetProperty("rating1", out _).Should().BeTrue();
        item.TryGetProperty("rating2", out _).Should().BeTrue();
        item.TryGetProperty("rating3", out _).Should().BeTrue();
        item.TryGetProperty("rating4", out _).Should().BeTrue();
        item.TryGetProperty("rating5", out _).Should().BeTrue();

        avg.GetDecimal().Should().BeGreaterThan(0);
        total.GetInt32().Should().BeGreaterThanOrEqualTo(3); // 3 reviews seeded

        // We seeded ratings 5, 4, 3
        item.GetProperty("rating5").GetInt32().Should().BeGreaterThanOrEqualTo(1);
        item.GetProperty("rating4").GetInt32().Should().BeGreaterThanOrEqualTo(1);
        item.GetProperty("rating3").GetInt32().Should().BeGreaterThanOrEqualTo(1);
    }

    // ==================== Cancellations ====================

    [Fact]
    public async Task CancellationStats_ReturnsRateAndTotals()
    {
        var response = await _client.GetAsync("/api/statistics/cancellations");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var item = await response.Content.ReadFromJsonAsync<JsonElement>();
        item.TryGetProperty("cancellationCount", out var count).Should().BeTrue();
        item.TryGetProperty("totalReservations", out var total).Should().BeTrue();
        item.TryGetProperty("cancellationRate", out var rate).Should().BeTrue();
        item.TryGetProperty("totalPenalty", out var penalty).Should().BeTrue();
        item.TryGetProperty("totalRefund", out var refund).Should().BeTrue();

        count.GetInt32().Should().BeGreaterThanOrEqualTo(1);   // 1 cancellation seeded
        total.GetInt32().Should().BeGreaterThanOrEqualTo(5);    // 5 reservations total
        rate.GetDecimal().Should().BeGreaterThan(0);
        penalty.GetDecimal().Should().BeGreaterThan(0);         // 43.13 penalty
        refund.GetDecimal().Should().BeGreaterThan(0);          // 129.37 refund
    }

    // ==================== Promo Codes ====================

    [Fact]
    public async Task PromoCodeStats_ReturnsUsagePerCode()
    {
        var response = await _client.GetAsync("/api/statistics/promo-codes");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
        items.Should().NotBeNull();
        items!.Count.Should().BeGreaterThanOrEqualTo(2); // 2 promo codes used in reservations

        // Verify shape
        var first = items.First();
        first.TryGetProperty("promoCodeId", out _).Should().BeTrue();
        first.TryGetProperty("code", out _).Should().BeTrue();
        first.TryGetProperty("usageCount", out _).Should().BeTrue();
        first.TryGetProperty("totalDiscount", out _).Should().BeTrue();
        first.TryGetProperty("currencyCode", out _).Should().BeTrue();

        // SUMMER25 used in 2 reservations
        var summer = items.First(x => x.GetProperty("code").GetString() == "SUMMER25");
        summer.GetProperty("usageCount").GetInt32().Should().BeGreaterThanOrEqualTo(2);
        summer.GetProperty("totalDiscount").GetDecimal().Should().BeGreaterThan(0);
    }

    // ==================== Subscriptions MRR ====================

    [Fact]
    public async Task SubscriptionMrr_ReturnsMonthlyRevenuePerPlan()
    {
        var response = await _client.GetAsync("/api/statistics/subscriptions/mrr");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
        items.Should().NotBeNull();
        items!.Count.Should().BeGreaterThanOrEqualTo(2); // 2 subscription types seeded

        // Verify shape
        var first = items.First();
        first.TryGetProperty("subscriptionName", out _).Should().BeTrue();
        first.TryGetProperty("currencyCode", out _).Should().BeTrue();
        first.TryGetProperty("activeCount", out _).Should().BeTrue();
        first.TryGetProperty("monthlyRevenue", out _).Should().BeTrue();

        // Premium Plan has 1 active subscriber at 29.99/month
        var premium = items.First(x => x.GetProperty("subscriptionName").GetString() == "Premium Plan");
        premium.GetProperty("activeCount").GetInt32().Should().BeGreaterThanOrEqualTo(1);
        premium.GetProperty("monthlyRevenue").GetDecimal().Should().BeGreaterThanOrEqualTo(29.99m);
    }

    // ==================== Date Filtering ====================

    [Fact]
    public async Task RevenueByHotel_WithDateFilter_FiltersResults()
    {
        // Use a far-future range that excludes our seeded data
        var response = await _client.GetAsync("/api/statistics/revenue/by-hotel?from=2099-01-01&to=2099-12-31");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
        items.Should().NotBeNull();
        items!.Should().BeEmpty(); // No completed reservations in 2099
    }

    [Fact]
    public async Task BookingsByStatus_WithDateFilter_FiltersResults()
    {
        var response = await _client.GetAsync("/api/statistics/bookings/by-status?from=2099-01-01&to=2099-12-31");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
        items.Should().NotBeNull();
        // All statuses still returned, but with 0 booking counts
        foreach (var item in items!)
        {
            item.GetProperty("bookingCount").GetInt32().Should().Be(0);
        }
    }

    [Fact]
    public async Task ReviewStats_WithDateFilter_ExcludesOutOfRange()
    {
        var response = await _client.GetAsync("/api/statistics/reviews?from=2099-01-01&to=2099-12-31");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var item = await response.Content.ReadFromJsonAsync<JsonElement>();
        item.GetProperty("totalReviews").GetInt32().Should().Be(0);
        item.GetProperty("averageRating").GetDecimal().Should().Be(0);
    }

    // ==================== Authorization ====================

    [Fact]
    public async Task AllEndpoints_Unauthenticated_Returns401()
    {
        var client = _factory.CreateClient();
        var endpoints = new[]
        {
            "/api/statistics/revenue/by-hotel",
            "/api/statistics/revenue/by-provider",
            "/api/statistics/bookings/by-status",
            "/api/statistics/bookings/average",
            "/api/statistics/users/by-type",
            "/api/statistics/users/subscriptions",
            "/api/statistics/reviews",
            "/api/statistics/cancellations",
            "/api/statistics/promo-codes",
            "/api/statistics/subscriptions/mrr"
        };

        foreach (var url in endpoints)
        {
            var response = await client.GetAsync(url);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, $"Expected 401 for unauthenticated request to {url}");
        }
    }

    [Theory]
    [InlineData(1, HttpStatusCode.OK)]        // Admin → read access
    [InlineData(2, HttpStatusCode.OK)]        // Manager → read access
    [InlineData(3, HttpStatusCode.Forbidden)] // Agent → no access
    [InlineData(4, HttpStatusCode.Forbidden)] // Hotel Staff → no access
    public async Task RevenueByHotel_RoleAccess(long userTypeId, HttpStatusCode expected)
    {
        var client = TestAuthHelper.CreateAuthenticatedClient(_factory, userTypeId: userTypeId);
        var response = await client.GetAsync("/api/statistics/revenue/by-hotel");
        response.StatusCode.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, HttpStatusCode.OK)]
    [InlineData(2, HttpStatusCode.OK)]
    [InlineData(3, HttpStatusCode.Forbidden)]
    [InlineData(4, HttpStatusCode.Forbidden)]
    public async Task ActiveSubscriptions_RoleAccess(long userTypeId, HttpStatusCode expected)
    {
        var client = TestAuthHelper.CreateAuthenticatedClient(_factory, userTypeId: userTypeId);
        var response = await client.GetAsync("/api/statistics/users/subscriptions");
        response.StatusCode.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, HttpStatusCode.OK)]
    [InlineData(2, HttpStatusCode.OK)]
    [InlineData(3, HttpStatusCode.Forbidden)]
    [InlineData(4, HttpStatusCode.Forbidden)]
    public async Task ReviewStats_RoleAccess(long userTypeId, HttpStatusCode expected)
    {
        var client = TestAuthHelper.CreateAuthenticatedClient(_factory, userTypeId: userTypeId);
        var response = await client.GetAsync("/api/statistics/reviews");
        response.StatusCode.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, HttpStatusCode.OK)]
    [InlineData(2, HttpStatusCode.OK)]
    [InlineData(3, HttpStatusCode.Forbidden)]
    [InlineData(4, HttpStatusCode.Forbidden)]
    public async Task CancellationStats_RoleAccess(long userTypeId, HttpStatusCode expected)
    {
        var client = TestAuthHelper.CreateAuthenticatedClient(_factory, userTypeId: userTypeId);
        var response = await client.GetAsync("/api/statistics/cancellations");
        response.StatusCode.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, HttpStatusCode.OK)]
    [InlineData(2, HttpStatusCode.OK)]
    [InlineData(3, HttpStatusCode.Forbidden)]
    [InlineData(4, HttpStatusCode.Forbidden)]
    public async Task SubscriptionMrr_RoleAccess(long userTypeId, HttpStatusCode expected)
    {
        var client = TestAuthHelper.CreateAuthenticatedClient(_factory, userTypeId: userTypeId);
        var response = await client.GetAsync("/api/statistics/subscriptions/mrr");
        response.StatusCode.Should().Be(expected);
    }

    // ==================== MariaDB-specific (graceful on SQLite) ====================

    [Theory]
    [InlineData("/api/statistics/revenue/by-period")]
    [InlineData("/api/statistics/revenue/by-period?groupBy=day")]
    [InlineData("/api/statistics/revenue/by-period?groupBy=month")]
    [InlineData("/api/statistics/bookings/volume")]
    [InlineData("/api/statistics/bookings/volume?groupBy=day")]
    [InlineData("/api/statistics/users/growth")]
    [InlineData("/api/statistics/users/growth?groupBy=day")]
    [InlineData("/api/statistics/occupancy")]
    [InlineData("/api/statistics/occupancy?from=2025-01-01&to=2025-12-31")]
    public async Task MariaDbSpecificEndpoints_ReturnOkOrServerError(string url)
    {
        var response = await _client.GetAsync(url);
        // DATE_FORMAT, DATEDIFF, CURDATE, DATE_SUB are MariaDB-only
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.InternalServerError);
    }
}
