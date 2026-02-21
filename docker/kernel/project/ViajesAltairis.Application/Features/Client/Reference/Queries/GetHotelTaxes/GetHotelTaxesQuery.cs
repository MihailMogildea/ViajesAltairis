using MediatR;

namespace ViajesAltairis.Application.Features.Client.Reference.Queries.GetHotelTaxes;

public class GetHotelTaxesQuery : IRequest<List<HotelTaxDto>>
{
    public long HotelId { get; set; }
}

public class HotelTaxDto
{
    public string TaxTypeName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public bool IsPercentage { get; set; }
}
