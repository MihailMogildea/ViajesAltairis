namespace ViajesAltairis.Application.Features.Admin.WebTranslations.Dtos;

public record CreateWebTranslationRequest(string TranslationKey, long LanguageId, string Value);
