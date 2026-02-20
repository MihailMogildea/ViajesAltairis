using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.EmailTemplates.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.EmailTemplates.Queries;

public record GetEmailTemplatesQuery : IRequest<IEnumerable<EmailTemplateDto>>;

public class GetEmailTemplatesHandler : IRequestHandler<GetEmailTemplatesQuery, IEnumerable<EmailTemplateDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetEmailTemplatesHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<EmailTemplateDto>> Handle(GetEmailTemplatesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<EmailTemplateDto>(
            "SELECT id AS Id, name AS Name FROM email_template ORDER BY name");
    }
}
