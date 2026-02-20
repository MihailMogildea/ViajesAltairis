using ViajesAltairis.Application.Features.ExternalClient.Hotels.Queries;
using ViajesAltairis.ExternalClient.Api.Tests.Helpers;

namespace ViajesAltairis.ExternalClient.Api.Tests.Hotels;

public class SearchHotelsHandlerTests : IDisposable
{
    private readonly SqliteTestDatabase _db;
    private readonly SearchHotelsHandler _handler;

    public SearchHotelsHandlerTests()
    {
        _db = new SqliteTestDatabase();
        _db.CreateSchema().SeedReferenceData().CreateHotelViews();
        _handler = new SearchHotelsHandler(_db);

        _db.Execute("""
            INSERT INTO hotel (id, name, stars, enabled, city_id) VALUES
                (1, 'Grand Hotel Barcelona', 5, 1, 1),
                (2, 'Budget Inn Barcelona', 2, 1, 1),
                (3, 'Le Petit Paris', 4, 1, 2),
                (4, 'Closed Hotel', 3, 0, 1);
            INSERT INTO cancellation_policy (id, hotel_id, free_cancellation_hours, enabled) VALUES
                (1, 1, 48, 1);
            INSERT INTO user (id, email, password_hash, enabled, user_type_id, first_name, last_name)
                VALUES (1, 'reviewer@test.com', 'hash', 1, 1, 'Test', 'User');
            INSERT INTO review (id, hotel_id, user_id, rating, visible) VALUES
                (1, 1, 1, 9, 1), (2, 1, 1, 8, 1);
            """);
    }

    [Fact]
    public async Task Search_ReturnsOnlyEnabledHotels()
    {
        var query = new SearchHotelsQuery(null, null, null, null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.TotalCount.Should().Be(3);
        result.Hotels.Should().HaveCount(3);
        result.Hotels.Should().NotContain(h => h.HotelName == "Closed Hotel");
    }

    [Fact]
    public async Task Search_FiltersByCity()
    {
        var query = new SearchHotelsQuery("Barcelona", null, null, null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.Hotels.Should().AllSatisfy(h => h.CityName.Should().Be("Barcelona"));
    }

    [Fact]
    public async Task Search_FiltersByStarRange()
    {
        var query = new SearchHotelsQuery(null, null, MinStars: 4, MaxStars: 5);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.Hotels.Should().AllSatisfy(h => h.Stars.Should().BeGreaterThanOrEqualTo(4));
    }

    [Fact]
    public async Task Search_IncludesRatingAndReviewCount()
    {
        var query = new SearchHotelsQuery(null, null, MinStars: 5, null);

        var result = await _handler.Handle(query, CancellationToken.None);

        var grand = result.Hotels.Should().ContainSingle().Subject;
        grand.AvgRating.Should().Be(8.5m);
        grand.ReviewCount.Should().Be(2);
        grand.FreeCancellationHours.Should().Be(48);
    }

    [Fact]
    public async Task Search_Pagination()
    {
        var query = new SearchHotelsQuery(null, null, null, null, Page: 1, PageSize: 2);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.TotalCount.Should().Be(3);
        result.Hotels.Should().HaveCount(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(2);
    }

    public void Dispose() => _db.Dispose();
}
