using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Client.Hotels.Queries.GetCancellationPolicy;
using ViajesAltairis.Application.Features.Client.Hotels.Queries.GetHotelDetail;
using ViajesAltairis.Application.Features.Client.Hotels.Queries.GetHotelReviews;
using ViajesAltairis.Application.Features.Client.Hotels.Queries.GetRoomAvailability;
using ViajesAltairis.Application.Features.Client.Hotels.Queries.SearchHotels;

namespace ViajesAltairis.ClientApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class HotelsController : ControllerBase
{
    private readonly IMediator _mediator;

    public HotelsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] SearchHotelsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(long id)
    {
        var result = await _mediator.Send(new GetHotelDetailQuery { HotelId = id });
        return Ok(result);
    }

    [HttpGet("{id}/availability")]
    public async Task<IActionResult> GetAvailability(long id, [FromQuery] GetRoomAvailabilityQuery query)
    {
        query.HotelId = id;
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}/reviews")]
    public async Task<IActionResult> GetReviews(long id, [FromQuery] GetHotelReviewsQuery query)
    {
        query.HotelId = id;
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}/cancellation-policy")]
    public async Task<IActionResult> GetCancellationPolicy(long id)
    {
        var result = await _mediator.Send(new GetCancellationPolicyQuery { HotelId = id });
        return Ok(result);
    }
}
