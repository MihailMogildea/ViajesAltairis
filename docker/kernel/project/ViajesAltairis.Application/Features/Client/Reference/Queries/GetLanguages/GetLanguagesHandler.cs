using Dapper;
using MediatR;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Reference.Queries.GetLanguages;

public class GetLanguagesHandler : IRequestHandler<GetLanguagesQuery, GetLanguagesResponse>
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetLanguagesHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<GetLanguagesResponse> Handle(GetLanguagesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = "SELECT id AS Id, iso_code AS Code, name AS Name FROM language ORDER BY name";

        var languages = (await connection.QueryAsync<LanguageDto>(sql)).ToList();

        return new GetLanguagesResponse { Languages = languages };
    }
}
