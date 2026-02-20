using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.EmailTemplates.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.EmailTemplates.Queries;

public record GetEmailTemplateByIdQuery(long Id) : IRequest<EmailTemplateDto?>;

public class GetEmailTemplateByIdHandler : IRequestHandler<GetEmailTemplateByIdQuery, EmailTemplateDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetEmailTemplateByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<EmailTemplateDto?> Handle(GetEmailTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<EmailTemplateDto>(
            "SELECT id AS Id, name AS Name FROM email_template WHERE id = @Id",
            new { request.Id });
    }
}
