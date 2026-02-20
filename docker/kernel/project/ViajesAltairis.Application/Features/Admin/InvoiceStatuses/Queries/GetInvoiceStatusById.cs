using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.InvoiceStatuses.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.InvoiceStatuses.Queries;

public record GetInvoiceStatusByIdQuery(long Id) : IRequest<InvoiceStatusDto?>;

public class GetInvoiceStatusByIdHandler : IRequestHandler<GetInvoiceStatusByIdQuery, InvoiceStatusDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetInvoiceStatusByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<InvoiceStatusDto?> Handle(GetInvoiceStatusByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<InvoiceStatusDto>(
            "SELECT id AS Id, name AS Name, created_at AS CreatedAt FROM invoice_status WHERE id = @Id",
            new { request.Id });
    }
}
