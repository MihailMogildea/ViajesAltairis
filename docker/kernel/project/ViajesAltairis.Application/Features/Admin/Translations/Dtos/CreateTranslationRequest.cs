namespace ViajesAltairis.Application.Features.Admin.Translations.Dtos;

public record CreateTranslationRequest(string EntityType, long EntityId, string Field, long LanguageId, string Value);
