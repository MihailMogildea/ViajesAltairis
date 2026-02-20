using FluentAssertions;
using NSubstitute;
using ViajesAltairis.Application.Features.Client.Reviews.Commands.SubmitReview;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Client.Api.Tests.Fixtures;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Client.Api.Tests.Handlers.Reviews;

public class SubmitReviewHandlerTests
{
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly IReservationApiClient _reservationApi = Substitute.For<IReservationApiClient>();
    private readonly IRepository<Review> _reviewRepo = Substitute.For<IRepository<Review>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    public SubmitReviewHandlerTests()
    {
        _currentUser.UserId.Returns(8L);
    }

    [Fact]
    public async Task Handle_ValidReview_CreatesReview()
    {
        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "review", "id INTEGER PRIMARY KEY, reservation_id INTEGER");

        _reservationApi.GetReservationLineInfoAsync(1, Arg.Any<CancellationToken>())
            .Returns(new ReservationLineInfoResult(1, 100, 1, 8));

        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);

        var handler = new SubmitReviewHandler(_currentUser, _reservationApi, factory, _reviewRepo, _unitOfWork);
        await handler.Handle(new SubmitReviewCommand
        {
            ReservationLineId = 1, Rating = 5, Title = "Great", Comment = "Loved it"
        }, CancellationToken.None);

        await _reviewRepo.Received(1).AddAsync(Arg.Is<Review>(r => r.Rating == 5 && r.HotelId == 1), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NotOwnReservation_ThrowsUnauthorized()
    {
        _reservationApi.GetReservationLineInfoAsync(1, Arg.Any<CancellationToken>())
            .Returns(new ReservationLineInfoResult(1, 100, 1, 99)); // different user

        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "review", "id INTEGER PRIMARY KEY, reservation_id INTEGER");
        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);

        var handler = new SubmitReviewHandler(_currentUser, _reservationApi, factory, _reviewRepo, _unitOfWork);
        var act = () => handler.Handle(new SubmitReviewCommand { ReservationLineId = 1, Rating = 5 }, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Handle_LineNotFound_ThrowsKeyNotFound()
    {
        _reservationApi.GetReservationLineInfoAsync(999, Arg.Any<CancellationToken>())
            .Returns((ReservationLineInfoResult?)null);

        using var conn = TestDbHelper.CreateConnection();
        TestDbHelper.CreateTable(conn, "review", "id INTEGER PRIMARY KEY, reservation_id INTEGER");
        var factory = Substitute.For<IDbConnectionFactory>();
        factory.CreateConnection().Returns(conn);

        var handler = new SubmitReviewHandler(_currentUser, _reservationApi, factory, _reviewRepo, _unitOfWork);
        var act = () => handler.Handle(new SubmitReviewCommand { ReservationLineId = 999, Rating = 5 }, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
