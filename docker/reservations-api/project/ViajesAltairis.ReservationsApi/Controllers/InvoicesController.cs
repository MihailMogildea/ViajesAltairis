using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Application.Reservations.Commands;
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

    [HttpGet("{id:long}/pdf")]
    public async Task<IActionResult> GetPdf(long id, [FromQuery] long userId, [FromQuery] long? languageId = null)
    {
        var result = await _mediator.Send(new GetInvoicePdfQuery(id, userId, languageId));
        if (result is null)
            return NotFound();
        return File(result.PdfBytes, "application/pdf", result.FileName);
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceDetailResult>> Create([FromBody] CreateInvoiceRequest request)
    {
        var result = await _mediator.Send(new CreateInvoiceFromReservationCommand(request.ReservationId, request.UserId));
        return Ok(result);
    }
}

public class CreateInvoiceRequest
{
    public long ReservationId { get; set; }
    public long UserId { get; set; }
}
