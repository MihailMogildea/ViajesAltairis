using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.Billing.Queries;
using ViajesAltairis.Application.Features.Admin.Invoices.Commands;
using ViajesAltairis.Application.Features.Admin.Invoices.Dtos;
using ViajesAltairis.Application.Features.Admin.Invoices.Queries;

namespace ViajesAltairis.AdminApi.Features.Invoices;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly ISender _sender;
    public InvoicesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetInvoicesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetInvoiceByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:long}/status")]
    public async Task<IActionResult> SetStatus(long id, SetStatusRequest request)
    {
        await _sender.Send(new SetInvoiceStatusCommand(id, request.StatusId));
        return NoContent();
    }

    [HttpGet("billing-export")]
    public async Task<IActionResult> BillingExport([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] string channel)
    {
        var result = await _sender.Send(new ExportBillingZipQuery(from, to, channel));
        return File(result.ZipBytes, "application/zip", result.FileName);
    }
}
