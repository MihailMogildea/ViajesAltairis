using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Users.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Users.Queries;

public record GetUsersQuery : IRequest<IEnumerable<UserDto>>;

public class GetUsersHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetUsersHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<UserDto>(
            "SELECT id AS Id, user_type_id AS UserTypeId, email AS Email, first_name AS FirstName, last_name AS LastName, phone AS Phone, tax_id AS TaxId, address AS Address, city AS City, postal_code AS PostalCode, country AS Country, language_id AS LanguageId, business_partner_id AS BusinessPartnerId, provider_id AS ProviderId, discount AS Discount, enabled AS Enabled, created_at AS CreatedAt FROM user ORDER BY last_name, first_name");
    }
}
