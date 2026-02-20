using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Reviews.Commands.SubmitReview;

public class SubmitReviewHandler : IRequestHandler<SubmitReviewCommand, long>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IReservationApiClient _reservationApi;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IRepository<Review> _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitReviewHandler(
        ICurrentUserService currentUser,
        IReservationApiClient reservationApi,
        IDbConnectionFactory connectionFactory,
        IRepository<Review> reviewRepository,
        IUnitOfWork unitOfWork)
    {
        _currentUser = currentUser;
        _reservationApi = reservationApi;
        _connectionFactory = connectionFactory;
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<long> Handle(SubmitReviewCommand request, CancellationToken cancellationToken)
    {
        var lineInfo = await _reservationApi.GetReservationLineInfoAsync(request.ReservationLineId, cancellationToken);
        if (lineInfo == null)
            throw new KeyNotFoundException("Reservation line not found.");

        if (lineInfo.UserId != _currentUser.UserId!.Value)
            throw new UnauthorizedAccessException("This reservation does not belong to you.");

        using var connection = _connectionFactory.CreateConnection();
        var existingReview = await connection.ExecuteScalarAsync<long?>(
            "SELECT id FROM review WHERE reservation_id = @ReservationId",
            new { lineInfo.ReservationId });

        if (existingReview.HasValue)
            throw new InvalidOperationException("A review already exists for this reservation.");

        var review = new Review
        {
            ReservationId = lineInfo.ReservationId,
            UserId = _currentUser.UserId.Value,
            HotelId = lineInfo.HotelId,
            Rating = (byte)request.Rating,
            Title = request.Title,
            Comment = request.Comment,
            Visible = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _reviewRepository.AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return review.Id;
    }
}
