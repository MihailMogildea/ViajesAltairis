using MediatR;

namespace ViajesAltairis.Application.Features.Client.Reference.Queries.GetLanguages;

public class GetLanguagesQuery : IRequest<GetLanguagesResponse>
{
}

public class GetLanguagesResponse
{
    public List<LanguageDto> Languages { get; set; } = new();
}

public class LanguageDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
