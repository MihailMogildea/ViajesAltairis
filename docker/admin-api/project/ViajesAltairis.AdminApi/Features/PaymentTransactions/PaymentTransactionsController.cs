using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.PaymentTransactions.Queries;

namespace ViajesAltairis.AdminApi.Features.PaymentTransactions;

[ApiController]
[Route("api/[controller]")]
public class PaymentTransactionsController : ControllerBase
{
    private readonly ISender _sender;
    public PaymentTransactionsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetPaymentTransactionsQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetPaymentTransactionByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }
}
