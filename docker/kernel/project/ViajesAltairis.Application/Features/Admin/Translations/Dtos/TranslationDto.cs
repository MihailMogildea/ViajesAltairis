namespace ViajesAltairis.Application.Features.Admin.Translations.Dtos;

public record TranslationDto(long Id, string EntityType, long EntityId, string Field, long LanguageId, string Value, DateTime CreatedAt);
