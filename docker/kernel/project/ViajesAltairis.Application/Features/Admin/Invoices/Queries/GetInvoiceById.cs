using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Invoices.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Invoices.Queries;

public record GetInvoiceByIdQuery(long Id) : IRequest<InvoiceDto?>;

public class GetInvoiceByIdHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetInvoiceByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<InvoiceDto?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<InvoiceDto>(
            @"SELECT id AS Id, invoice_number AS InvoiceNumber, status_id AS StatusId,
                     reservation_id AS ReservationId, business_partner_id AS BusinessPartnerId,
                     subtotal AS Subtotal, tax_amount AS TaxAmount, discount_amount AS DiscountAmount,
                     total_amount AS TotalAmount, period_start AS PeriodStart, period_end AS PeriodEnd,
                     created_at AS CreatedAt, updated_at AS UpdatedAt
              FROM invoice WHERE id = @Id",
            new { request.Id });
    }
}
