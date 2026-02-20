using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.BusinessPartners.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.BusinessPartners.Queries;

public record GetBusinessPartnersQuery : IRequest<IEnumerable<BusinessPartnerDto>>;

public class GetBusinessPartnersHandler : IRequestHandler<GetBusinessPartnersQuery, IEnumerable<BusinessPartnerDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetBusinessPartnersHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<BusinessPartnerDto>> Handle(GetBusinessPartnersQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<BusinessPartnerDto>(
            "SELECT id AS Id, company_name AS CompanyName, tax_id AS TaxId, discount AS Discount, address AS Address, city AS City, postal_code AS PostalCode, country AS Country, contact_email AS ContactEmail, contact_phone AS ContactPhone, enabled AS Enabled, created_at AS CreatedAt FROM business_partner ORDER BY company_name");
    }
}
