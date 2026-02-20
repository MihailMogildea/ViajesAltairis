using MediatR;

namespace ViajesAltairis.Application.Features.Client.Reference.Queries.GetCountries;

public class GetCountriesQuery : IRequest<GetCountriesResponse>
{
}

public class GetCountriesResponse
{
    public List<CountryDto> Countries { get; set; } = new();
}

public class CountryDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
