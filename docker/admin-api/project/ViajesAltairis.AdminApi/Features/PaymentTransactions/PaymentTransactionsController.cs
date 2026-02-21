using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.PaymentTransactions.Commands;
using ViajesAltairis.Application.Features.Admin.PaymentTransactions.Queries;

namespace ViajesAltairis.AdminApi.Features.PaymentTransactions;

[ApiController]
[Route("api/[controller]")]
public class PaymentTransactionsController : ControllerBase
{
    private readonly ISender _sender;
    public PaymentTransactionsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] long? statusId)
        => Ok(await _sender.Send(new GetPaymentTransactionsQuery(from, to, statusId)));

    [HttpGet("statuses")]
    public async Task<IActionResult> GetStatuses()
        => Ok(await _sender.Send(new GetPaymentTransactionStatusesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetPaymentTransactionByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:long}/confirm")]
    public async Task<IActionResult> Confirm(long id)
    {
        await _sender.Send(new ConfirmBankTransferCommand(id));
        return NoContent();
    }
}
