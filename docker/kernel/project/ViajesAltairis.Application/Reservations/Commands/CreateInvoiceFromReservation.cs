using FluentValidation;
using MediatR;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Enums;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Reservations.Commands;

public record CreateInvoiceFromReservationCommand(long ReservationId, long UserId) : IRequest<InvoiceDetailResult>;

public class CreateInvoiceFromReservationHandler : IRequestHandler<CreateInvoiceFromReservationCommand, InvoiceDetailResult>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRepository<Invoice> _invoiceRepository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInvoiceFromReservationHandler(
        IReservationRepository reservationRepository,
        IRepository<Invoice> invoiceRepository,
        IDbConnectionFactory connectionFactory,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _invoiceRepository = invoiceRepository;
        _connectionFactory = connectionFactory;
        _unitOfWork = unitOfWork;
    }

    public async Task<InvoiceDetailResult> Handle(CreateInvoiceFromReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetWithLinesAsync(request.ReservationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation {request.ReservationId} not found");

        if (reservation.BookedByUserId != request.UserId && reservation.OwnerUserId != request.UserId)
            throw new InvalidOperationException("Reservation does not belong to user");

        if (reservation.StatusId != (long)ReservationStatusEnum.Confirmed &&
            reservation.StatusId != (long)ReservationStatusEnum.Completed)
            throw new InvalidOperationException("Invoice can only be generated for confirmed or completed reservations");

        using var connection = _connectionFactory.CreateConnection();

        // Check if invoice already exists — return existing (idempotent)
        var existing = await Dapper.SqlMapper.QuerySingleOrDefaultAsync<dynamic>(
            connection,
            """
            SELECT i.id, i.invoice_number, i.status_id, ist.name AS status, i.subtotal, i.tax_amount,
                   i.total_amount, c.iso_code AS currency, er.rate_to_eur AS exchange_rate_to_eur,
                   i.created_at AS issued_at, i.reservation_id
            FROM invoice i
            JOIN invoice_status ist ON ist.id = i.status_id
            JOIN reservation r ON r.id = i.reservation_id
            JOIN currency c ON c.id = r.currency_id
            JOIN exchange_rate er ON er.id = r.exchange_rate_id
            WHERE i.reservation_id = @ReservationId
            LIMIT 1
            """,
            new { request.ReservationId });

        if (existing is not null)
        {
            return new InvoiceDetailResult(
                (long)existing.id,
                (string)existing.invoice_number,
                (long)existing.status_id,
                (string)existing.status,
                (decimal)existing.subtotal,
                (decimal)existing.tax_amount,
                (decimal)existing.total_amount,
                (string)existing.currency,
                (decimal)existing.exchange_rate_to_eur,
                (DateTime)existing.issued_at,
                null,
                (long)existing.reservation_id);
        }

        // Compute period from reservation lines
        var periodStart = reservation.ReservationLines.Min(l => l.CheckInDate);
        var periodEnd = reservation.ReservationLines.Max(l => l.CheckOutDate);

        // Generate invoice number
        var maxId = await Dapper.SqlMapper.ExecuteScalarAsync<long?>(
            connection, "SELECT MAX(id) FROM invoice") ?? 0;
        var invoiceNumber = $"INV-{DateTime.UtcNow:yyyy}-{(maxId + 1):D6}";

        var invoice = new Invoice
        {
            InvoiceNumber = invoiceNumber,
            StatusId = (long)InvoiceStatusEnum.Created,
            ReservationId = request.ReservationId,
            Subtotal = reservation.Subtotal,
            TaxAmount = reservation.TaxAmount,
            DiscountAmount = reservation.DiscountAmount,
            TotalAmount = reservation.TotalPrice,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
        };

        await _invoiceRepository.AddAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Get currency and exchange rate
        var currencyInfo = await Dapper.SqlMapper.QuerySingleAsync<dynamic>(
            connection,
            """
            SELECT c.iso_code AS currency, er.rate_to_eur
            FROM reservation r
            JOIN currency c ON c.id = r.currency_id
            JOIN exchange_rate er ON er.id = r.exchange_rate_id
            WHERE r.id = @ReservationId
            """,
            new { request.ReservationId });

        return new InvoiceDetailResult(
            invoice.Id,
            invoice.InvoiceNumber,
            (long)InvoiceStatusEnum.Created,
            "created",
            invoice.Subtotal,
            invoice.TaxAmount,
            invoice.TotalAmount,
            (string)currencyInfo.currency,
            (decimal)currencyInfo.rate_to_eur,
            DateTime.UtcNow,
            null,
            invoice.ReservationId);
    }
}

public class CreateInvoiceFromReservationValidator : AbstractValidator<CreateInvoiceFromReservationCommand>
{
    public CreateInvoiceFromReservationValidator()
    {
        RuleFor(x => x.ReservationId).GreaterThan(0);
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}
