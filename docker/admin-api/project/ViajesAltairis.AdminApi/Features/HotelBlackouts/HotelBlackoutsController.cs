using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.HotelBlackouts.Commands;
using ViajesAltairis.Application.Features.Admin.HotelBlackouts.Dtos;
using ViajesAltairis.Application.Features.Admin.HotelBlackouts.Queries;

namespace ViajesAltairis.AdminApi.Features.HotelBlackouts;

[ApiController]
[Route("api/[controller]")]
public class HotelBlackoutsController : ControllerBase
{
    private readonly ISender _sender;
    public HotelBlackoutsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetHotelBlackoutsQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetHotelBlackoutByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateHotelBlackoutRequest request)
    {
        var result = await _sender.Send(new CreateHotelBlackoutCommand(request.HotelId, request.StartDate, request.EndDate, request.Reason));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateHotelBlackoutRequest request)
    {
        var result = await _sender.Send(new UpdateHotelBlackoutCommand(id, request.HotelId, request.StartDate, request.EndDate, request.Reason));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteHotelBlackoutCommand(id));
        return NoContent();
    }
}
