using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.InvoiceStatuses.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.InvoiceStatuses.Queries;

public record GetInvoiceStatusesQuery : IRequest<IEnumerable<InvoiceStatusDto>>;

public class GetInvoiceStatusesHandler : IRequestHandler<GetInvoiceStatusesQuery, IEnumerable<InvoiceStatusDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetInvoiceStatusesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<InvoiceStatusDto>> Handle(GetInvoiceStatusesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<InvoiceStatusDto>(
            "SELECT id AS Id, name AS Name, created_at AS CreatedAt FROM invoice_status ORDER BY name");
    }
}
