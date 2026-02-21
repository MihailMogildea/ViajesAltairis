using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Reservations.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Reservations.Queries;

public record GetReservationsQuery : IRequest<IEnumerable<ReservationAdminDto>>;

public class GetReservationsHandler : IRequestHandler<GetReservationsQuery, IEnumerable<ReservationAdminDto>>
{
    private readonly IDbConnectionFactory _db;
    private readonly ICurrencyConverter _currencyConverter;
    private readonly ICurrentUserService _currentUser;

    public GetReservationsHandler(IDbConnectionFactory db, ICurrencyConverter currencyConverter, ICurrentUserService currentUser)
    {
        _db = db;
        _currencyConverter = currencyConverter;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<ReservationAdminDto>> Handle(GetReservationsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        var partnerId = _currentUser.BusinessPartnerId;
        var sql = @"SELECT
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
              JOIN user ub ON ub.id = r.booked_by_user_id"
            + (partnerId.HasValue ? " WHERE ub.business_partner_id = @PartnerId" : "")
            + " ORDER BY vrs.created_at DESC";
        var rows = (await connection.QueryAsync<ReservationAdminDto>(sql, new { PartnerId = partnerId })).ToList();

        // Currency conversion for admin display
        var displayCurrency = _currentUser.CurrencyCode;
        if (rows.Count > 0)
        {
            var targetCurrencyId = await connection.ExecuteScalarAsync<long?>(
                "SELECT id FROM currency WHERE iso_code = @Code", new { Code = displayCurrency });

            if (targetCurrencyId.HasValue)
            {
                var currencyGroups = rows.GroupBy(r => r.CurrencyId)
                    .Where(g => g.Key != targetCurrencyId.Value);

                foreach (var group in currencyGroups)
                {
                    var (factor, _) = await _currencyConverter.ConvertAsync(1m, group.Key, targetCurrencyId.Value, cancellationToken);
                    foreach (var r in group)
                    {
                        r.DisplaySubtotal = Math.Round(r.Subtotal * factor, 2);
                        r.DisplayTaxAmount = Math.Round(r.TaxAmount * factor, 2);
                        r.DisplayMarginAmount = Math.Round(r.MarginAmount * factor, 2);
                        r.DisplayDiscountAmount = Math.Round(r.DiscountAmount * factor, 2);
                        r.DisplayTotalPrice = Math.Round(r.TotalPrice * factor, 2);
                        r.DisplayCurrencyCode = displayCurrency;
                    }
                }
            }
        }

        return rows;
    }
}
