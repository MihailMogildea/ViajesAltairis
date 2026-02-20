namespace ViajesAltairis.Application.Features.Admin.WebTranslations.Dtos;

public record WebTranslationDto(long Id, string TranslationKey, long LanguageId, string Value, DateTime CreatedAt);
