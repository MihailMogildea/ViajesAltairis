using MediatR;

namespace ViajesAltairis.Application.Features.Client.Reference.Queries.GetWebTranslations;

public class GetWebTranslationsQuery : IRequest<Dictionary<string, string>>
{
}
