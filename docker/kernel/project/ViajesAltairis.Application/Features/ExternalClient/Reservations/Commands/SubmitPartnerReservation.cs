using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;

public record SubmitPartnerReservationCommand(
    long ReservationId, long PaymentMethodId,
    string? CardNumber, string? CardExpiry, string? CardCvv, string? CardHolderName) : IRequest<SubmitResult>
{
    public long BusinessPartnerId { get; init; }
}

public class SubmitPartnerReservationHandler : IRequestHandler<SubmitPartnerReservationCommand, SubmitResult>
{
    private readonly IDbConnectionFactory _db;
    private readonly IReservationApiClient _reservationApi;

    public SubmitPartnerReservationHandler(IDbConnectionFactory db, IReservationApiClient reservationApi)
    {
        _db = db;
        _reservationApi = reservationApi;
    }

    public async Task<SubmitResult> Handle(SubmitPartnerReservationCommand request, CancellationToken cancellationToken)
    {
        await VerifyOwnership(request.ReservationId, request.BusinessPartnerId);

        return await _reservationApi.SubmitAsync(
            request.ReservationId,
            request.PaymentMethodId,
            request.CardNumber, request.CardExpiry,
            request.CardCvv, request.CardHolderName,
            cancellationToken);
    }

    private async Task VerifyOwnership(long reservationId, long businessPartnerId)
    {
        using var connection = _db.CreateConnection();
        const string sql = @"
            SELECT COUNT(*) FROM reservation r
            JOIN user u ON u.id = r.booked_by_user_id
            WHERE r.id = @ReservationId AND u.business_partner_id = @BusinessPartnerId";

        var count = await connection.ExecuteScalarAsync<int>(sql, new { ReservationId = reservationId, BusinessPartnerId = businessPartnerId });
        if (count == 0)
            throw new InvalidOperationException("Reservation not found or does not belong to your organization.");
    }
}
