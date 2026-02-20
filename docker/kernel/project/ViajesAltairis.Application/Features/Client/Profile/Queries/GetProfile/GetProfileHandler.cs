using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Profile.Queries.GetProfile;

public class GetProfileHandler : IRequestHandler<GetProfileQuery, GetProfileResponse>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ICurrentUserService _currentUser;

    public GetProfileHandler(IDbConnectionFactory connectionFactory, ICurrentUserService currentUser)
    {
        _connectionFactory = connectionFactory;
        _currentUser = currentUser;
    }

    public async Task<GetProfileResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                u.id AS Id,
                u.email AS Email,
                u.first_name AS FirstName,
                u.last_name AS LastName,
                u.phone AS Phone,
                u.country AS Country,
                COALESCE(l.iso_code, 'en') AS PreferredLanguage,
                COALESCE(c.iso_code, 'EUR') AS PreferredCurrency,
                u.discount AS Discount,
                u.created_at AS CreatedAt
            FROM user u
            LEFT JOIN language l ON l.id = u.language_id
            LEFT JOIN currency c ON c.id = (
                SELECT st.currency_id
                FROM user_subscription us
                JOIN subscription_type st ON st.id = us.subscription_type_id
                WHERE us.user_id = u.id AND us.active = TRUE
                LIMIT 1
            )
            WHERE u.id = @UserId
            """;

        var profile = await connection.QuerySingleOrDefaultAsync<GetProfileResponse>(sql, new { UserId = _currentUser.UserId });

        if (profile == null)
            throw new KeyNotFoundException("User profile not found.");

        return profile;
    }
}
