using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.PaymentTransactionFees.Queries;

namespace ViajesAltairis.AdminApi.Features.PaymentTransactionFees;

[ApiController]
[Route("api/[controller]")]
public class PaymentTransactionFeesController : ControllerBase
{
    private readonly ISender _sender;
    public PaymentTransactionFeesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetPaymentTransactionFeesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetPaymentTransactionFeeByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }
}
