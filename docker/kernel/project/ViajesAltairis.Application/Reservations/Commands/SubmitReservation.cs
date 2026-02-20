using FluentValidation;
using MediatR;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Enums;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Reservations.Commands;

public record SubmitReservationCommand(
    long ReservationId,
    long PaymentMethodId,
    string? CardNumber,
    string? CardExpiry,
    string? CardCvv,
    string? CardHolderName) : IRequest<SubmitResult>;

public class SubmitReservationHandler : IRequestHandler<SubmitReservationCommand, SubmitResult>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService;
    private readonly IProviderReservationService _providerReservationService;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IRepository<PaymentTransaction> _paymentTransactionRepository;

    private const int MaxProviderRetries = 3;

    public SubmitReservationHandler(
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork,
        IPaymentService paymentService,
        IProviderReservationService providerReservationService,
        IDbConnectionFactory connectionFactory,
        IRepository<PaymentTransaction> paymentTransactionRepository)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
        _providerReservationService = providerReservationService;
        _connectionFactory = connectionFactory;
        _paymentTransactionRepository = paymentTransactionRepository;
    }

    public async Task<SubmitResult> Handle(SubmitReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetWithLinesAsync(request.ReservationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation {request.ReservationId} not found");

        if (reservation.StatusId != (long)ReservationStatusEnum.Draft)
            throw new InvalidOperationException("Can only submit draft reservations");

        if (!reservation.ReservationLines.Any())
            throw new InvalidOperationException("Cannot submit a reservation with no lines");

        using var connection = _connectionFactory.CreateConnection();

        // Validate payment method min_days_before_checkin
        var paymentMethod = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
            connection,
            "SELECT min_days_before_checkin FROM payment_method WHERE id = @Id AND enabled = 1",
            new { Id = request.PaymentMethodId });

        if (paymentMethod is null)
            throw new KeyNotFoundException("Payment method not found or disabled");

        var earliestCheckIn = reservation.ReservationLines.Min(l => l.CheckInDate);
        var daysUntilCheckIn = (earliestCheckIn.ToDateTime(TimeOnly.MinValue) - DateTime.UtcNow.Date).Days;
        if (daysUntilCheckIn < (int)paymentMethod.min_days_before_checkin)
            throw new InvalidOperationException(
                $"Payment method requires at least {paymentMethod.min_days_before_checkin} days before check-in");

        // Re-validate promo code if one was applied
        if (reservation.PromoCodeId.HasValue)
        {
            var promo = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
                connection,
                "SELECT valid_to, max_uses, current_uses FROM promo_code WHERE id = @Id AND enabled = 1",
                new { Id = reservation.PromoCodeId.Value });

            if (promo is null)
                throw new InvalidOperationException("Promo code is no longer valid");

            var validTo = (DateTime)promo.valid_to;
            if (validTo.Date < DateTime.UtcNow.Date)
                throw new InvalidOperationException("Promo code has expired");

            if (promo.max_uses is not null && (int)promo.current_uses >= (int)promo.max_uses)
                throw new InvalidOperationException("Promo code has reached its maximum usage");
        }

        // Get currency code for the payment request
        var currencyCode = await Dapper.SqlMapper.ExecuteScalarAsync<string>(
            connection,
            "SELECT iso_code FROM currency WHERE id = @Id",
            new { Id = reservation.CurrencyId })
            ?? throw new InvalidOperationException("Currency not found");

        // Process payment
        var paymentRequest = new PaymentRequest(
            request.ReservationId,
            request.PaymentMethodId,
            reservation.TotalPrice,
            currencyCode,
            request.CardNumber,
            request.CardExpiry,
            request.CardCvv,
            request.CardHolderName);

        var paymentResult = await _paymentService.ProcessPaymentAsync(paymentRequest, cancellationToken);
        if (!paymentResult.Success)
            throw new InvalidOperationException($"Payment failed: {paymentResult.FailureReason}");

        // Record payment transaction
        var paymentTransaction = new PaymentTransaction
        {
            ReservationId = request.ReservationId,
            PaymentMethodId = request.PaymentMethodId,
            TransactionReference = paymentResult.PaymentReference!,
            Amount = reservation.TotalPrice,
            CurrencyId = reservation.CurrencyId,
            ExchangeRateId = reservation.ExchangeRateId,
            Status = "completed"
        };
        await _paymentTransactionRepository.AddAsync(paymentTransaction, cancellationToken);

        // Increment promo code usage
        if (reservation.PromoCodeId.HasValue)
        {
            await Dapper.SqlMapper.ExecuteAsync(
                connection,
                "UPDATE promo_code SET current_uses = current_uses + 1 WHERE id = @Id",
                new { Id = reservation.PromoCodeId.Value });
        }

        // For each line with an external provider, create booking
        foreach (var line in reservation.ReservationLines)
        {
            var providerInfo = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
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
                // Refund payment on total failure
                await _paymentService.ProcessRefundAsync(
                    request.ReservationId, paymentResult.PaymentReference!, cancellationToken);
                throw new InvalidOperationException("Provider booking failed after retries. Payment refunded.");
            }
        }

        // Update status to Confirmed
        reservation.StatusId = (long)ReservationStatusEnum.Confirmed;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubmitResult(reservation.Id, "Confirmed", reservation.TotalPrice, currencyCode);
    }
}

public class SubmitReservationValidator : AbstractValidator<SubmitReservationCommand>
{
    public SubmitReservationValidator()
    {
        RuleFor(x => x.ReservationId).GreaterThan(0);
        RuleFor(x => x.PaymentMethodId).GreaterThan(0);
    }
}
