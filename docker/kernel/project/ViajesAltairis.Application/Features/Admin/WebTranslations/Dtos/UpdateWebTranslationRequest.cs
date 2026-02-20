namespace ViajesAltairis.Application.Features.Admin.WebTranslations.Dtos;

public record UpdateWebTranslationRequest(string TranslationKey, long LanguageId, string Value);
