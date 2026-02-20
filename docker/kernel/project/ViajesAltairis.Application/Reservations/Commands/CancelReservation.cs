using MediatR;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Enums;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Reservations.Commands;

public record CancelReservationCommand(long ReservationId, long? CancelledByUserId, string? Reason) : IRequest;

public class CancelReservationHandler : IRequestHandler<CancelReservationCommand>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService;
    private readonly IProviderReservationService _providerReservationService;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IRepository<Cancellation> _cancellationRepository;

    public CancelReservationHandler(
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork,
        IPaymentService paymentService,
        IProviderReservationService providerReservationService,
        IDbConnectionFactory connectionFactory,
        IRepository<Cancellation> cancellationRepository)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
        _providerReservationService = providerReservationService;
        _connectionFactory = connectionFactory;
        _cancellationRepository = cancellationRepository;
    }

    public async Task Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetWithLinesAsync(request.ReservationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation {request.ReservationId} not found");

        if (reservation.StatusId == (long)ReservationStatusEnum.Completed
            || reservation.StatusId == (long)ReservationStatusEnum.Cancelled)
            throw new InvalidOperationException("Reservation cannot be cancelled in its current status");

        using var connection = _connectionFactory.CreateConnection();

        // Determine cancellation penalty from strictest hotel policy
        var earliestCheckIn = reservation.ReservationLines.Min(l => l.CheckInDate);
        var hoursUntilCheckIn = (earliestCheckIn.ToDateTime(TimeOnly.MinValue) - DateTime.UtcNow).TotalHours;

        var strictestPolicy = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
            connection,
            """
            SELECT cp.free_cancellation_hours, cp.penalty_percentage
            FROM cancellation_policy cp
            JOIN hotel h ON h.id = cp.hotel_id
            JOIN hotel_provider hp ON hp.hotel_id = h.id
            JOIN hotel_provider_room_type hprt ON hprt.hotel_provider_id = hp.id
            JOIN reservation_line rl ON rl.hotel_provider_room_type_id = hprt.id
            WHERE rl.reservation_id = @ReservationId AND cp.enabled = 1
            ORDER BY cp.penalty_percentage DESC
            LIMIT 1
            """,
            new { ReservationId = request.ReservationId });

        decimal penaltyPct = 0;
        if (strictestPolicy is not null && hoursUntilCheckIn < (int)strictestPolicy.free_cancellation_hours)
            penaltyPct = (decimal)strictestPolicy.penalty_percentage;

        decimal penaltyAmount = reservation.TotalPrice * penaltyPct / 100;
        decimal refundAmount = reservation.TotalPrice - penaltyAmount;

        // Create cancellation record
        var cancellation = new Cancellation
        {
            ReservationId = request.ReservationId,
            CancelledByUserId = request.CancelledByUserId ?? reservation.BookedByUserId,
            Reason = request.Reason,
            PenaltyPercentage = penaltyPct,
            PenaltyAmount = penaltyAmount,
            RefundAmount = refundAmount,
            CurrencyId = reservation.CurrencyId,
        };
        await _cancellationRepository.AddAsync(cancellation, cancellationToken);

        // Cancel with external providers if confirmed or beyond
        if (reservation.StatusId >= (long)ReservationStatusEnum.Confirmed)
        {
            foreach (var line in reservation.ReservationLines)
            {
                var providerInfo = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
                    connection,
                    """
                    SELECT hp.provider_id, pt.name AS provider_type
                    FROM hotel_provider_room_type hprt
                    JOIN hotel_provider hp ON hp.id = hprt.hotel_provider_id
                    JOIN provider p ON p.id = hp.provider_id
                    JOIN provider_type pt ON pt.id = p.type_id
                    WHERE hprt.id = @Id
                    """,
                    new { Id = line.HotelProviderRoomTypeId });

                if (providerInfo is not null && (string)providerInfo.provider_type != "internal")
                {
                    // Best-effort cancel with provider
                    await _providerReservationService.CancelBookingAsync(
                        (long)providerInfo.provider_id, reservation.ReservationCode, cancellationToken);
                }
            }

            // Refund if paid and refund amount > 0
            if (refundAmount > 0)
            {
                var paymentRef = await Dapper.SqlMapper.ExecuteScalarAsync<string?>(
                    connection,
                    """
                    SELECT transaction_reference FROM payment_transaction
                    WHERE reservation_id = @ReservationId AND status = 'completed'
                    ORDER BY created_at DESC LIMIT 1
                    """,
                    new { ReservationId = request.ReservationId });

                if (paymentRef is not null)
                {
                    await _paymentService.ProcessRefundAsync(request.ReservationId, paymentRef, cancellationToken);
                }
            }
        }

        // Update status to Cancelled
        reservation.StatusId = (long)ReservationStatusEnum.Cancelled;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
