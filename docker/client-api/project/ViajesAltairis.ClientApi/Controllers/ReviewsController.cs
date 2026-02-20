using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Client.Reviews.Commands.SubmitReview;

namespace ViajesAltairis.ClientApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Client")]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] SubmitReviewCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(null, new { id = result }, result);
    }
}
