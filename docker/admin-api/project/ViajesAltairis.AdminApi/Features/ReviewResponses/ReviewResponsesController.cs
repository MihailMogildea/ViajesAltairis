using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.ReviewResponses.Commands;
using ViajesAltairis.Application.Features.Admin.ReviewResponses.Queries;

namespace ViajesAltairis.AdminApi.Features.ReviewResponses;

[ApiController]
[Route("api/[controller]")]
public class ReviewResponsesController : ControllerBase
{
    private readonly ISender _sender;
    public ReviewResponsesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetReviewResponsesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetReviewResponseByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteReviewResponseCommand(id));
        return NoContent();
    }
}
