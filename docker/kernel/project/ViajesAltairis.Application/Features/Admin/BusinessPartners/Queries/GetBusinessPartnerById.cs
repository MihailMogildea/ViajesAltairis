using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.BusinessPartners.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.BusinessPartners.Queries;

public record GetBusinessPartnerByIdQuery(long Id) : IRequest<BusinessPartnerDto?>;

public class GetBusinessPartnerByIdHandler : IRequestHandler<GetBusinessPartnerByIdQuery, BusinessPartnerDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetBusinessPartnerByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<BusinessPartnerDto?> Handle(GetBusinessPartnerByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<BusinessPartnerDto>(
            "SELECT id AS Id, company_name AS CompanyName, tax_id AS TaxId, discount AS Discount, address AS Address, city AS City, postal_code AS PostalCode, country AS Country, contact_email AS ContactEmail, contact_phone AS ContactPhone, enabled AS Enabled, created_at AS CreatedAt FROM business_partner WHERE id = @Id",
            new { request.Id });
    }
}
