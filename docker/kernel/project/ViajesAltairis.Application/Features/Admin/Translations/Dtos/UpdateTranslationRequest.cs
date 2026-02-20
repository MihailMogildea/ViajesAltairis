namespace ViajesAltairis.Application.Features.Admin.Translations.Dtos;

public record UpdateTranslationRequest(string EntityType, long EntityId, string Field, long LanguageId, string Value);
