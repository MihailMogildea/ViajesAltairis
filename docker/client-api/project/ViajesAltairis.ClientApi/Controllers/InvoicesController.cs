using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Client.Invoices.Queries.GetInvoiceDetail;
using ViajesAltairis.Application.Features.Client.Invoices.Queries.GetMyInvoices;

namespace ViajesAltairis.ClientApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Client")]
public class InvoicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvoicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyInvoices([FromQuery] GetMyInvoicesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(long id)
    {
        var result = await _mediator.Send(new GetInvoiceDetailQuery { InvoiceId = id });
        return Ok(result);
    }
}
