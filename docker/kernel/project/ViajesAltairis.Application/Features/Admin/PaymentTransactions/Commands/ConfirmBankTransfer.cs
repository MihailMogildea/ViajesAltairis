using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Enums;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PaymentTransactions.Commands;

public record ConfirmBankTransferCommand(long TransactionId) : IRequest<Unit>;

public class ConfirmBankTransferHandler : IRequestHandler<ConfirmBankTransferCommand, Unit>
{
    private readonly IRepository<PaymentTransaction> _transactionRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProviderReservationService _providerReservationService;
    private readonly IPaymentService _paymentService;
    private readonly IDbConnectionFactory _connectionFactory;

    private const int MaxProviderRetries = 3;

    public ConfirmBankTransferHandler(
        IRepository<PaymentTransaction> transactionRepository,
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork,
        IProviderReservationService providerReservationService,
        IPaymentService paymentService,
        IDbConnectionFactory connectionFactory)
    {
        _transactionRepository = transactionRepository;
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _providerReservationService = providerReservationService;
        _paymentService = paymentService;
        _connectionFactory = connectionFactory;
    }

    public async Task<Unit> Handle(ConfirmBankTransferCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(request.TransactionId, cancellationToken)
            ?? throw new KeyNotFoundException($"Transaction {request.TransactionId} not found");

        if (transaction.StatusId != 1) // 1 = pending
            throw new InvalidOperationException("Only pending transactions can be confirmed");

        var reservation = await _reservationRepository.GetWithLinesAsync(transaction.ReservationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation {transaction.ReservationId} not found");

        if (reservation.StatusId != (long)ReservationStatusEnum.Pending)
            throw new InvalidOperationException("Reservation is not in Pending status");

        using var connection = _connectionFactory.CreateConnection();

        // Execute provider bookings for each line
        foreach (var line in reservation.ReservationLines)
        {
            var providerInfo = await SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
                connection,
                """
                SELECT hp.provider_id, hp.hotel_id, pt.name AS provider_type
                FROM hotel_provider_room_type hprt
                JOIN hotel_provider hp ON hp.id = hprt.hotel_provider_id
                JOIN provider p ON p.id = hp.provider_id
                JOIN provider_type pt ON pt.id = p.type_id
                WHERE hprt.id = @Id
                """,
                new { Id = line.HotelProviderRoomTypeId });

            if (providerInfo is null || (string)providerInfo.provider_type == "internal")
                continue;

            var bookingRequest = new ProviderBookingRequest(
                (long)providerInfo.provider_id,
                (long)providerInfo.hotel_id,
                line.HotelProviderRoomTypeId,
                line.CheckInDate.ToDateTime(TimeOnly.MinValue),
                line.CheckOutDate.ToDateTime(TimeOnly.MinValue),
                line.NumGuests);

            var booked = false;
            for (int attempt = 0; attempt < MaxProviderRetries; attempt++)
            {
                var result = await _providerReservationService.CreateBookingAsync(bookingRequest, cancellationToken);
                if (result.Success)
                {
                    booked = true;
                    break;
                }
            }

            if (!booked)
            {
                await _paymentService.ProcessRefundAsync(
                    transaction.ReservationId, transaction.TransactionReference, cancellationToken);
                throw new InvalidOperationException("Provider booking failed after retries. Payment refunded.");
            }
        }

        // Mark transaction as completed and reservation as confirmed
        transaction.StatusId = 2; // completed
        reservation.StatusId = (long)ReservationStatusEnum.Confirmed;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
