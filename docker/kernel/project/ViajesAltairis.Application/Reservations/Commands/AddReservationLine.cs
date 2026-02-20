using FluentValidation;
using MediatR;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Enums;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Reservations.Commands;

public record AddReservationLineCommand(
    long ReservationId,
    long RoomConfigurationId,
    long BoardTypeId,
    DateTime CheckIn,
    DateTime CheckOut,
    int GuestCount) : IRequest<long>;

public class AddReservationLineHandler : IRequestHandler<AddReservationLineCommand, long>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrencyConverter _currencyConverter;

    public AddReservationLineHandler(
        IReservationRepository reservationRepository,
        IDbConnectionFactory connectionFactory,
        IUnitOfWork unitOfWork,
        ICurrencyConverter currencyConverter)
    {
        _reservationRepository = reservationRepository;
        _connectionFactory = connectionFactory;
        _unitOfWork = unitOfWork;
        _currencyConverter = currencyConverter;
    }

    public async Task<long> Handle(AddReservationLineCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetWithLinesAsync(request.ReservationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation {request.ReservationId} not found");

        if (reservation.StatusId != (long)ReservationStatusEnum.Draft)
            throw new InvalidOperationException("Can only add lines to draft reservations");

        using var connection = _connectionFactory.CreateConnection();

        // Load room configuration with hotel geography
        var roomConfig = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
            connection,
            """
            SELECT hprt.id, hprt.price_per_night, hprt.quantity, hprt.hotel_provider_id,
                   hprt.currency_id,
                   hp.provider_id, hp.hotel_id, p.margin AS provider_margin, h.margin AS hotel_margin,
                   h.city_id, c.administrative_division_id, ad.country_id
            FROM hotel_provider_room_type hprt
            JOIN hotel_provider hp ON hp.id = hprt.hotel_provider_id
            JOIN provider p ON p.id = hp.provider_id
            JOIN hotel h ON h.id = hp.hotel_id
            JOIN city c ON c.id = h.city_id
            JOIN administrative_division ad ON ad.id = c.administrative_division_id
            WHERE hprt.id = @Id AND hprt.enabled = 1
            """,
            new { Id = request.RoomConfigurationId });

        if (roomConfig is null)
            throw new KeyNotFoundException($"Room configuration {request.RoomConfigurationId} not found or disabled");

        // Load board price
        var board = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
            connection,
            """
            SELECT price_per_night FROM hotel_provider_room_type_board
            WHERE hotel_provider_room_type_id = @RoomConfigId AND board_type_id = @BoardTypeId
            """,
            new { RoomConfigId = request.RoomConfigurationId, BoardTypeId = request.BoardTypeId });

        if (board is null)
            throw new KeyNotFoundException($"Board type {request.BoardTypeId} not available for room configuration {request.RoomConfigurationId}");

        // Check availability
        var checkIn = DateOnly.FromDateTime(request.CheckIn);
        var checkOut = DateOnly.FromDateTime(request.CheckOut);
        var numNights = (checkOut.ToDateTime(TimeOnly.MinValue) - checkIn.ToDateTime(TimeOnly.MinValue)).Days;

        var bookedCount = await Dapper.SqlMapper.ExecuteScalarAsync<int>(
            connection,
            """
            SELECT COALESCE(SUM(rl.num_rooms), 0) FROM reservation_line rl
            JOIN reservation r ON r.id = rl.reservation_id
            WHERE rl.hotel_provider_room_type_id = @RoomConfigId
              AND r.status_id NOT IN (@Completed, @Cancelled)
              AND rl.check_in_date < @CheckOut AND rl.check_out_date > @CheckIn
            """,
            new
            {
                RoomConfigId = request.RoomConfigurationId,
                CheckIn = checkIn,
                CheckOut = checkOut,
                Completed = (long)ReservationStatusEnum.Completed,
                Cancelled = (long)ReservationStatusEnum.Cancelled
            });

        int availableQuantity = (int)roomConfig.quantity;
        if (bookedCount >= availableQuantity)
            throw new InvalidOperationException("No rooms available for the selected dates");

        // Calculate base pricing (convert from provider currency to reservation currency)
        decimal pricePerNight = (decimal)roomConfig.price_per_night;
        decimal boardPricePerNight = (decimal)board.price_per_night;

        long providerCurrencyId = (long)roomConfig.currency_id;
        if (providerCurrencyId != reservation.CurrencyId)
        {
            var (convertedRoom, _) = await _currencyConverter.ConvertAsync(
                pricePerNight, providerCurrencyId, reservation.CurrencyId, cancellationToken);
            var (convertedBoard, _) = await _currencyConverter.ConvertAsync(
                boardPricePerNight, providerCurrencyId, reservation.CurrencyId, cancellationToken);
            pricePerNight = convertedRoom;
            boardPricePerNight = convertedBoard;
        }

        decimal subtotal = (pricePerNight + boardPricePerNight) * numNights;

        // Margins (additive)
        decimal providerMargin = (decimal)roomConfig.provider_margin;
        decimal hotelMargin = (decimal)roomConfig.hotel_margin;

        // Seasonal margin: hotel → city → administrative_division → seasonal_margin (MM-DD comparison)
        var checkInMmDd = checkIn.ToString("MM-dd");
        var checkOutMmDd = checkOut.ToString("MM-dd");

        var seasonalMargin = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
            connection,
            """
            SELECT sm.margin FROM seasonal_margin sm
            WHERE sm.administrative_division_id = @AdminDivId
              AND (
                CASE WHEN sm.start_month_day <= sm.end_month_day THEN
                  @CheckInMmDd <= sm.end_month_day AND @CheckOutMmDd >= sm.start_month_day
                ELSE
                  @CheckInMmDd >= sm.start_month_day OR @CheckOutMmDd <= sm.end_month_day
                END
              )
            ORDER BY sm.margin DESC LIMIT 1
            """,
            new { AdminDivId = (long)roomConfig.administrative_division_id, CheckInMmDd = checkInMmDd, CheckOutMmDd = checkOutMmDd });

        decimal seasonalMarginPct = seasonalMargin is not null ? (decimal)seasonalMargin.margin : 0;
        decimal totalMarginPct = providerMargin + hotelMargin + seasonalMarginPct;
        decimal marginAmount = subtotal * totalMarginPct / 100;

        // Discount stack
        var userInfo = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
            connection,
            "SELECT discount, business_partner_id FROM user WHERE id = @UserId",
            new { UserId = reservation.BookedByUserId });

        decimal userDiscountPct = userInfo is not null ? (decimal)userInfo.discount : 0;

        decimal bpDiscountPct = 0;
        if (userInfo?.business_partner_id is not null)
        {
            var bp = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
                connection,
                "SELECT discount FROM business_partner WHERE id = @Id AND enabled = 1",
                new { Id = (long)userInfo.business_partner_id });
            if (bp is not null)
                bpDiscountPct = (decimal)bp.discount;
        }

        var subscription = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
            connection,
            """
            SELECT st.discount FROM user_subscription us
            JOIN subscription_type st ON st.id = us.subscription_type_id
            WHERE us.user_id = @UserId AND us.active = 1
              AND us.start_date <= CURDATE() AND (us.end_date IS NULL OR us.end_date >= CURDATE())
            ORDER BY st.discount DESC LIMIT 1
            """,
            new { UserId = reservation.BookedByUserId });

        decimal subscriptionDiscountPct = subscription is not null ? (decimal)subscription.discount : 0;

        decimal promoDiscountPct = 0;
        decimal promoFixedAmount = 0;
        if (reservation.PromoCodeId.HasValue)
        {
            var promo = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
                connection,
                "SELECT discount_percentage, discount_amount FROM promo_code WHERE id = @Id AND enabled = 1",
                new { Id = reservation.PromoCodeId.Value });
            if (promo is not null)
            {
                promoDiscountPct = promo.discount_percentage is not null ? (decimal)promo.discount_percentage : 0;
                promoFixedAmount = promo.discount_amount is not null ? (decimal)promo.discount_amount : 0;
            }
        }

        decimal totalDiscountPct = userDiscountPct + bpDiscountPct + subscriptionDiscountPct + promoDiscountPct;
        decimal discountAmount = (subtotal + marginAmount) * totalDiscountPct / 100;

        // Tax: hierarchical lookup (city → administrative_division → country), most specific wins per tax type
        var taxes = await Dapper.SqlMapper.QueryAsync<dynamic>(
            connection,
            """
            SELECT t.tax_type_id, t.rate, t.is_percentage,
                   CASE WHEN t.city_id IS NOT NULL THEN 1
                        WHEN t.administrative_division_id IS NOT NULL THEN 2
                        ELSE 3 END AS specificity
            FROM tax t
            WHERE t.enabled = 1
              AND (t.city_id = @CityId OR t.administrative_division_id = @AdminDivId OR t.country_id = @CountryId)
            ORDER BY t.tax_type_id, specificity
            """,
            new { CityId = (long)roomConfig.city_id, AdminDivId = (long)roomConfig.administrative_division_id, CountryId = (long)roomConfig.country_id });

        decimal taxableAmount = subtotal + marginAmount - discountAmount;
        decimal taxAmount = 0;
        var seenTaxTypes = new HashSet<long>();
        foreach (var tax in taxes)
        {
            if (!seenTaxTypes.Add((long)tax.tax_type_id)) continue;
            taxAmount += (bool)tax.is_percentage
                ? taxableAmount * (decimal)tax.rate / 100
                : (decimal)tax.rate;
        }

        decimal totalPrice = subtotal + marginAmount - discountAmount + taxAmount;

        var line = new ReservationLine
        {
            ReservationId = request.ReservationId,
            HotelProviderRoomTypeId = request.RoomConfigurationId,
            BoardTypeId = request.BoardTypeId,
            CheckInDate = checkIn,
            CheckOutDate = checkOut,
            NumRooms = 1,
            NumGuests = (byte)request.GuestCount,
            PricePerNight = pricePerNight,
            BoardPricePerNight = boardPricePerNight,
            NumNights = numNights,
            Subtotal = subtotal,
            TaxAmount = taxAmount,
            MarginAmount = marginAmount,
            DiscountAmount = discountAmount,
            TotalPrice = totalPrice,
            CurrencyId = reservation.CurrencyId,
            ExchangeRateId = reservation.ExchangeRateId,
        };

        reservation.ReservationLines.Add(line);

        // Recalculate header totals (promo fixed amount applied at header level)
        reservation.Subtotal = reservation.ReservationLines.Sum(l => l.Subtotal);
        reservation.TaxAmount = reservation.ReservationLines.Sum(l => l.TaxAmount);
        reservation.MarginAmount = reservation.ReservationLines.Sum(l => l.MarginAmount);
        reservation.DiscountAmount = reservation.ReservationLines.Sum(l => l.DiscountAmount) + promoFixedAmount;
        reservation.TotalPrice = reservation.ReservationLines.Sum(l => l.TotalPrice) - promoFixedAmount;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return line.Id;
    }
}

public class AddReservationLineValidator : AbstractValidator<AddReservationLineCommand>
{
    public AddReservationLineValidator()
    {
        RuleFor(x => x.ReservationId).GreaterThan(0);
        RuleFor(x => x.RoomConfigurationId).GreaterThan(0);
        RuleFor(x => x.BoardTypeId).GreaterThan(0);
        RuleFor(x => x.CheckIn).GreaterThan(DateTime.UtcNow.Date);
        RuleFor(x => x.CheckOut).GreaterThan(x => x.CheckIn);
        RuleFor(x => x.GuestCount).InclusiveBetween(1, 255);
    }
}
