using MediatR;

namespace ViajesAltairis.Application.Features.Client.Reference.Queries.GetPaymentMethods;

public class GetPaymentMethodsQuery : IRequest<GetPaymentMethodsResponse>
{
}

public class GetPaymentMethodsResponse
{
    public List<PaymentMethodDto> PaymentMethods { get; set; } = new();
}

public class PaymentMethodDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int MinDaysBeforeCheckin { get; set; }
}
