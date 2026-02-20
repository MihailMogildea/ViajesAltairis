using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.Reviews.Commands;
using ViajesAltairis.Application.Features.Admin.Reviews.Dtos;
using ViajesAltairis.Application.Features.Admin.Reviews.Queries;

namespace ViajesAltairis.AdminApi.Features.Reviews;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly ISender _sender;
    public ReviewsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetReviewsQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetReviewByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:long}/visible")]
    public async Task<IActionResult> SetVisible(long id, SetVisibleRequest request)
    {
        await _sender.Send(new SetReviewVisibleCommand(id, request.Visible));
        return NoContent();
    }
}
