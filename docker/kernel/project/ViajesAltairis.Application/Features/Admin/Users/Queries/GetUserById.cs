using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.Users.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Users.Queries;

public record GetUserByIdQuery(long Id) : IRequest<UserDto?>;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetUserByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<UserDto>(
            "SELECT id AS Id, user_type_id AS UserTypeId, email AS Email, first_name AS FirstName, last_name AS LastName, phone AS Phone, tax_id AS TaxId, address AS Address, city AS City, postal_code AS PostalCode, country AS Country, language_id AS LanguageId, business_partner_id AS BusinessPartnerId, provider_id AS ProviderId, discount AS Discount, enabled AS Enabled, created_at AS CreatedAt FROM user WHERE id = @Id",
            new { request.Id });
    }
}
