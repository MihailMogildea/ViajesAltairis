using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Hotels.Queries.GetHotelReviews;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Client.Api.Tests.Fixtures;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Hotels;

public class GetHotelReviewsHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsReviewsWithStats()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "v_hotel_review_detail", @"
            review_id INTEGER, hotel_id INTEGER, rating INTEGER, title TEXT, comment TEXT,
            reviewer_first_name TEXT, reviewer_last_name TEXT,
            review_created_at TEXT, response_comment TEXT, response_created_at TEXT, visible INTEGER");
        TestDbHelper.Execute(conn, @"
            INSERT INTO v_hotel_review_detail VALUES (1, 1, 5, 'Great', 'Loved it', 'Juan', 'Martínez', '2026-01-15', 'Thanks!', '2026-01-16', 1);
            INSERT INTO v_hotel_review_detail VALUES (2, 1, 3, 'OK', 'Average', 'Emma', 'Wilson', '2026-01-10', NULL, NULL, 1);
            INSERT INTO v_hotel_review_detail VALUES (3, 1, 4, 'Hidden', 'Test', 'Test', 'T', '2026-01-05', NULL, NULL, 0);
        ");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);

        var handler = new GetHotelReviewsHandler(factory);
        var result = await handler.Handle(new GetHotelReviewsQuery { HotelId = 1, Page = 1, PageSize = 10 }, CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.AverageRating.Should().Be(4m);
        result.Reviews.Should().HaveCount(2);
        result.Reviews.First().UserName.Should().Contain("Juan");
    }

    [Fact]
    public async Task Handle_Pagination_WorksCorrectly()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "v_hotel_review_detail", @"
            review_id INTEGER, hotel_id INTEGER, rating INTEGER, title TEXT, comment TEXT,
            reviewer_first_name TEXT, reviewer_last_name TEXT,
            review_created_at TEXT, response_comment TEXT, response_created_at TEXT, visible INTEGER");
        for (int i = 1; i <= 5; i++)
            TestDbHelper.Execute(conn, $"INSERT INTO v_hotel_review_detail VALUES ({i}, 1, 4, 'Review {i}', 'Comment', 'User', 'U', '2026-01-{i:D2}', NULL, NULL, 1)");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);

        var handler = new GetHotelReviewsHandler(factory);
        var result = await handler.Handle(new GetHotelReviewsQuery { HotelId = 1, Page = 2, PageSize = 2 }, CancellationToken.None);

        result.TotalCount.Should().Be(5);
        result.Reviews.Should().HaveCount(2);
    }
}
