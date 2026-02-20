namespace ViajesAltairis.Application.Features.Admin.NotificationLogs.Dtos;

public record NotificationLogDto(long Id, long UserId, long EmailTemplateId, string RecipientEmail, string Subject, string Body, DateTime CreatedAt);
