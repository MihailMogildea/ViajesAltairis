using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Client.PromoCodes.Queries.ValidatePromoCode;

namespace ViajesAltairis.ClientApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Client")]
public class PromoCodesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PromoCodesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("validate")]
    public async Task<IActionResult> Validate([FromQuery] ValidatePromoCodeQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
