using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.InvoiceStatuses.Queries;

namespace ViajesAltairis.AdminApi.Features.InvoiceStatuses;

[ApiController]
[Route("api/[controller]")]
public class InvoiceStatusesController : ControllerBase
{
    private readonly ISender _sender;
    public InvoiceStatusesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetInvoiceStatusesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetInvoiceStatusByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }
}
