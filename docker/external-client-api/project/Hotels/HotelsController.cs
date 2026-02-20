using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.ExternalClient.Hotels.Dtos;
using ViajesAltairis.Application.Features.ExternalClient.Hotels.Queries;

namespace ViajesAltairis.ExternalClientApi.Hotels;

[ApiController]
[Route("api/hotels")]
[Authorize]
public class HotelsController : ControllerBase
{
    private readonly IMediator _mediator;

    public HotelsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(SearchHotelsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string? city,
        [FromQuery] string? country,
        [FromQuery] int? minStars,
        [FromQuery] int? maxStars,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new SearchHotelsQuery(city, country, minStars, maxStars, page, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(HotelDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetail(long id)
    {
        var result = await _mediator.Send(new GetHotelDetailQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{id:long}/availability")]
    [ProducesResponseType(typeof(GetAvailabilityResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailability(
        long id,
        [FromQuery] DateOnly checkIn,
        [FromQuery] DateOnly checkOut)
    {
        var result = await _mediator.Send(new GetAvailabilityQuery(id, checkIn, checkOut));
        return Ok(result);
    }
}
