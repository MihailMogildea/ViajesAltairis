using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Reservations.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Reservations.Queries;

public record GetReservationByIdQuery(long Id) : IRequest<ReservationAdminDto?>;

public class GetReservationByIdHandler : IRequestHandler<GetReservationByIdQuery, ReservationAdminDto?>
{
    private readonly IDbConnectionFactory _db;
    private readonly ICurrencyConverter _currencyConverter;
    private readonly ICurrentUserService _currentUser;

    public GetReservationByIdHandler(IDbConnectionFactory db, ICurrencyConverter currencyConverter, ICurrentUserService currentUser)
    {
        _db = db;
        _currencyConverter = currencyConverter;
        _currentUser = currentUser;
    }

    public async Task<ReservationAdminDto?> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        var row = await connection.QuerySingleOrDefaultAsync<ReservationAdminDto>(
            @"SELECT
                vrs.reservation_id AS Id, vrs.reservation_code AS ReservationCode,
                vrs.status_id AS StatusId, vrs.status_name AS StatusName,
                vrs.booked_by_user_id AS BookedByUserId,
                vrs.booked_by_first_name AS BookedByFirstName,
                vrs.booked_by_last_name AS BookedByLastName,
                r.owner_user_id AS OwnerUserId,
                vrs.owner_first_name AS OwnerFirstName, vrs.owner_last_name AS OwnerLastName,
                vrs.owner_email AS OwnerEmail, vrs.owner_phone AS OwnerPhone,
                r.owner_tax_id AS OwnerTaxId, r.owner_address AS OwnerAddress,
                r.owner_city AS OwnerCity, r.owner_postal_code AS OwnerPostalCode,
                r.owner_country AS OwnerCountry,
                vrs.subtotal AS Subtotal, vrs.tax_amount AS TaxAmount,
                vrs.margin_amount AS MarginAmount, vrs.discount_amount AS DiscountAmount,
                vrs.total_price AS TotalPrice,
                r.currency_id AS CurrencyId, vrs.currency_code AS CurrencyCode,
                r.exchange_rate_id AS ExchangeRateId,
                vrs.promo_code_id AS PromoCodeId, vrs.promo_code AS PromoCode,
                vrs.notes AS Notes, vrs.line_count AS LineCount,
                vrs.created_at AS CreatedAt, vrs.updated_at AS UpdatedAt
              FROM v_reservation_summary vrs
              JOIN reservation r ON r.id = vrs.reservation_id
              WHERE vrs.reservation_id = @Id",
            new { request.Id });

        if (row is null)
            return null;

        // Currency conversion for admin display
        var displayCurrency = _currentUser.CurrencyCode;
        if (row.CurrencyId > 0)
        {
            var targetCurrencyId = await connection.ExecuteScalarAsync<long?>(
                "SELECT id FROM currency WHERE iso_code = @Code", new { Code = displayCurrency });

            if (targetCurrencyId.HasValue && targetCurrencyId.Value != row.CurrencyId)
            {
                var (factor, _) = await _currencyConverter.ConvertAsync(1m, row.CurrencyId, targetCurrencyId.Value, cancellationToken);
                row.DisplaySubtotal = Math.Round(row.Subtotal * factor, 2);
                row.DisplayTaxAmount = Math.Round(row.TaxAmount * factor, 2);
                row.DisplayMarginAmount = Math.Round(row.MarginAmount * factor, 2);
                row.DisplayDiscountAmount = Math.Round(row.DiscountAmount * factor, 2);
                row.DisplayTotalPrice = Math.Round(row.TotalPrice * factor, 2);
                row.DisplayCurrencyCode = displayCurrency;
            }
        }

        return row;
    }
}
