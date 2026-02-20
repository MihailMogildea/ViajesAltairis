using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Hotels.Queries.GetCancellationPolicy;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Client.Api.Tests.Fixtures;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Hotels;

public class GetCancellationPolicyHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPoliciesOrdered()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "cancellation_policy", @"
            id INTEGER PRIMARY KEY, hotel_id INTEGER, free_cancellation_hours INTEGER,
            penalty_percentage REAL, enabled INTEGER");
        TestDbHelper.Execute(conn, @"
            INSERT INTO cancellation_policy VALUES (1, 1, 48, 0, 1);
            INSERT INTO cancellation_policy VALUES (2, 1, 24, 50, 1);
            INSERT INTO cancellation_policy VALUES (3, 1, 0, 100, 1);
            INSERT INTO cancellation_policy VALUES (4, 1, 72, 0, 0);
        ");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);

        var handler = new GetCancellationPolicyHandler(factory);
        var result = await handler.Handle(new GetCancellationPolicyQuery { HotelId = 1 }, CancellationToken.None);

        result.Policies.Should().HaveCount(3);
        result.Policies.First().HoursBeforeCheckIn.Should().Be(48);
    }

    [Fact]
    public async Task Handle_NoPolicies_ReturnsEmptyList()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "cancellation_policy", @"
            id INTEGER PRIMARY KEY, hotel_id INTEGER, free_cancellation_hours INTEGER,
            penalty_percentage REAL, enabled INTEGER");

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);

        var handler = new GetCancellationPolicyHandler(factory);
        var result = await handler.Handle(new GetCancellationPolicyQuery { HotelId = 999 }, CancellationToken.None);

        result.Policies.Should().BeEmpty();
    }
}
