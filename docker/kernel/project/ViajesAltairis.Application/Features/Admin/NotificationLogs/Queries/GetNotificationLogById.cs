using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.NotificationLogs.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.NotificationLogs.Queries;

public record GetNotificationLogByIdQuery(long Id) : IRequest<NotificationLogDto?>;

public class GetNotificationLogByIdHandler : IRequestHandler<GetNotificationLogByIdQuery, NotificationLogDto?>
{
    private readonly IDbConnectionFactory _db;
    public GetNotificationLogByIdHandler(IDbConnectionFactory db) => _db = db;

    public async Task<NotificationLogDto?> Handle(GetNotificationLogByIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<NotificationLogDto>(
            @"SELECT id AS Id, user_id AS UserId, email_template_id AS EmailTemplateId,
                     recipient_email AS RecipientEmail, subject AS Subject, body AS Body, created_at AS CreatedAt
              FROM notification_log WHERE id = @Id",
            new { request.Id });
    }
}
