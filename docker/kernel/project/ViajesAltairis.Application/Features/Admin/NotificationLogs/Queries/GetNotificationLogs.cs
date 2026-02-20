using Dapper;
using MediatR;
using ViajesAltairis.Application.Features.Admin.NotificationLogs.Dtos;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.NotificationLogs.Queries;

public record GetNotificationLogsQuery : IRequest<IEnumerable<NotificationLogDto>>;

public class GetNotificationLogsHandler : IRequestHandler<GetNotificationLogsQuery, IEnumerable<NotificationLogDto>>
{
    private readonly IDbConnectionFactory _db;
    public GetNotificationLogsHandler(IDbConnectionFactory db) => _db = db;

    public async Task<IEnumerable<NotificationLogDto>> Handle(GetNotificationLogsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        return await connection.QueryAsync<NotificationLogDto>(
            @"SELECT id AS Id, user_id AS UserId, email_template_id AS EmailTemplateId,
                     recipient_email AS RecipientEmail, subject AS Subject, body AS Body, created_at AS CreatedAt
              FROM notification_log ORDER BY created_at DESC");
    }
}
