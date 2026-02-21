using FluentValidation;
using MediatR;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Enums;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Reservations.Commands;

public record CreateDraftReservationCommand(
    long UserId,
    string CurrencyCode,
    string? PromoCode,
    long? OwnerUserId = null,
    string? OwnerFirstName = null,
    string? OwnerLastName = null,
    string? OwnerEmail = null,
    string? OwnerPhone = null,
    string? OwnerTaxId = null,
    string? OwnerAddress = null,
    string? OwnerCity = null,
    string? OwnerPostalCode = null,
    string? OwnerCountry = null
) : IRequest<long>;

public class CreateDraftReservationHandler : IRequestHandler<CreateDraftReservationCommand, long>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDraftReservationHandler(
        IReservationRepository reservationRepository,
        IRepository<User> userRepository,
        IDbConnectionFactory connectionFactory,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _userRepository = userRepository;
        _connectionFactory = connectionFactory;
        _unitOfWork = unitOfWork;
    }

    public async Task<long> Handle(CreateDraftReservationCommand request, CancellationToken cancellationToken)
    {
        // Determine owner: on-behalf (OwnerUserId set), walk-in (owner fields set), or self-booking (default)
        var booker = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"User {request.UserId} not found");

        long? ownerUserId;
        string ownerFirstName, ownerLastName;
        string? ownerEmail, ownerPhone, ownerTaxId, ownerAddress, ownerCity, ownerPostalCode, ownerCountry;

        if (request.OwnerUserId.HasValue)
        {
            // Booking on behalf of an existing client
            var owner = await _userRepository.GetByIdAsync(request.OwnerUserId.Value, cancellationToken)
                ?? throw new KeyNotFoundException($"Owner user {request.OwnerUserId.Value} not found");
            ownerUserId = owner.Id;
            ownerFirstName = owner.FirstName;
            ownerLastName = owner.LastName;
            ownerEmail = owner.Email;
            ownerPhone = owner.Phone;
            ownerTaxId = owner.TaxId;
            ownerAddress = owner.Address;
            ownerCity = owner.City;
            ownerPostalCode = owner.PostalCode;
            ownerCountry = owner.Country;
        }
        else if (HasAnyOwnerField(request))
        {
            // Walk-in guest — use provided fields
            ownerUserId = null;
            ownerFirstName = request.OwnerFirstName!;
            ownerLastName = request.OwnerLastName!;
            ownerEmail = request.OwnerEmail;
            ownerPhone = request.OwnerPhone;
            ownerTaxId = request.OwnerTaxId;
            ownerAddress = request.OwnerAddress;
            ownerCity = request.OwnerCity;
            ownerPostalCode = request.OwnerPostalCode;
            ownerCountry = request.OwnerCountry;
        }
        else
        {
            // Self-booking (legacy path)
            ownerUserId = booker.Id;
            ownerFirstName = booker.FirstName;
            ownerLastName = booker.LastName;
            ownerEmail = booker.Email;
            ownerPhone = booker.Phone;
            ownerTaxId = booker.TaxId;
            ownerAddress = booker.Address;
            ownerCity = booker.City;
            ownerPostalCode = booker.PostalCode;
            ownerCountry = booker.Country;
        }

        // Lookup currency and latest exchange rate via Dapper
        using var connection = _connectionFactory.CreateConnection();
        var currency = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
            connection,
            "SELECT id FROM currency WHERE iso_code = @Code",
            new { Code = request.CurrencyCode });

        if (currency is null)
            throw new KeyNotFoundException($"Currency '{request.CurrencyCode}' not found or disabled");

        long currencyId = (long)currency.id;

        var exchangeRate = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
            connection,
            """
            SELECT id FROM exchange_rate
            WHERE currency_id = @CurrencyId AND valid_from <= NOW() AND valid_to >= NOW()
            ORDER BY valid_from DESC LIMIT 1
            """,
            new { CurrencyId = currencyId });

        if (exchangeRate is null)
            throw new InvalidOperationException($"No active exchange rate found for {request.CurrencyCode}");

        long exchangeRateId = (long)exchangeRate.id;

        // Lookup promo code if provided
        long? promoCodeId = null;
        if (!string.IsNullOrWhiteSpace(request.PromoCode))
        {
            var promo = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
                connection,
                """
                SELECT id FROM promo_code
                WHERE code = @Code AND enabled = 1 AND valid_from <= CURDATE() AND valid_to >= CURDATE()
                  AND (max_uses IS NULL OR current_uses < max_uses)
                """,
                new { Code = request.PromoCode });

            if (promo is null)
                throw new KeyNotFoundException($"Promo code '{request.PromoCode}' not found or expired");
            promoCodeId = (long)promo.id;
        }

        var reservation = new Reservation
        {
            ReservationCode = GenerateCode(),
            StatusId = (long)ReservationStatusEnum.Draft,
            BookedByUserId = request.UserId,
            OwnerUserId = ownerUserId,
            OwnerFirstName = ownerFirstName,
            OwnerLastName = ownerLastName,
            OwnerEmail = ownerEmail,
            OwnerPhone = ownerPhone,
            OwnerTaxId = ownerTaxId,
            OwnerAddress = ownerAddress,
            OwnerCity = ownerCity,
            OwnerPostalCode = ownerPostalCode,
            OwnerCountry = ownerCountry,
            Subtotal = 0,
            TaxAmount = 0,
            MarginAmount = 0,
            DiscountAmount = 0,
            TotalPrice = 0,
            CurrencyId = currencyId,
            ExchangeRateId = exchangeRateId,
            PromoCodeId = promoCodeId,
        };

        await _reservationRepository.AddAsync(reservation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }

    private static string GenerateCode()
    {
        return $"ALT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
    }

    private static bool HasAnyOwnerField(CreateDraftReservationCommand c) =>
        c.OwnerFirstName is not null || c.OwnerLastName is not null ||
        c.OwnerEmail is not null || c.OwnerPhone is not null ||
        c.OwnerTaxId is not null || c.OwnerAddress is not null ||
        c.OwnerCity is not null || c.OwnerPostalCode is not null ||
        c.OwnerCountry is not null;
}

public class CreateDraftReservationValidator : AbstractValidator<CreateDraftReservationCommand>
{
    public CreateDraftReservationValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.CurrencyCode).NotEmpty().Length(3);

        // Walk-in guest: if no OwnerUserId but any owner field is provided, require first+last name
        When(x => x.OwnerUserId is null && (
            x.OwnerFirstName is not null || x.OwnerLastName is not null ||
            x.OwnerEmail is not null || x.OwnerPhone is not null ||
            x.OwnerTaxId is not null || x.OwnerAddress is not null ||
            x.OwnerCity is not null || x.OwnerPostalCode is not null ||
            x.OwnerCountry is not null), () =>
        {
            RuleFor(x => x.OwnerFirstName).NotEmpty();
            RuleFor(x => x.OwnerLastName).NotEmpty();
        });
    }
}
