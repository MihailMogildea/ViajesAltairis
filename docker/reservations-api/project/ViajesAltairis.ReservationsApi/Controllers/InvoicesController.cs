using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Application.Reservations.Queries;

namespace ViajesAltairis.ReservationsApi.Controllers;

[ApiController]
[Route("api/invoices")]
public class InvoicesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InvoicesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<InvoiceListResult>> GetByUser(
        [FromQuery] long userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetInvoicesByUserQuery(userId, page, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<InvoiceDetailResult>> GetById(long id, [FromQuery] long userId)
    {
        var result = await _mediator.Send(new GetInvoiceByIdQuery(id, userId));
        if (result is null)
            return NotFound();
        return Ok(result);
    }
}
