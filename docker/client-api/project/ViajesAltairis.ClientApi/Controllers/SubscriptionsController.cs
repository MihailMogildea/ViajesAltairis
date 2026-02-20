using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Client.Subscriptions.Commands.Subscribe;
using ViajesAltairis.Application.Features.Client.Subscriptions.Queries.GetMySubscription;
using ViajesAltairis.Application.Features.Client.Subscriptions.Queries.GetSubscriptionPlans;

namespace ViajesAltairis.ClientApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Client")]
public class SubscriptionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubscriptionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("plans")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPlans()
    {
        var result = await _mediator.Send(new GetSubscriptionPlansQuery());
        return Ok(result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMySubscription()
    {
        var result = await _mediator.Send(new GetMySubscriptionQuery());
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
