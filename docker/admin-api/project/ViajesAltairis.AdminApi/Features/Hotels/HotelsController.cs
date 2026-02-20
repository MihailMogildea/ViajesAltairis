using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.Hotels.Commands;
using ViajesAltairis.Application.Features.Admin.Hotels.Dtos;
using ViajesAltairis.Application.Features.Admin.Hotels.Queries;
using ViajesAltairis.Application.Features.Admin.Shared;

namespace ViajesAltairis.AdminApi.Features.Hotels;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly ISender _sender;
    public HotelsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetHotelsQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetHotelByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateHotelRequest request)
    {
        var result = await _sender.Send(new CreateHotelCommand(request.CityId, request.Name, request.Stars, request.Address, request.Email, request.Phone, request.CheckInTime, request.CheckOutTime, request.Latitude, request.Longitude, request.Margin));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateHotelRequest request)
    {
        var result = await _sender.Send(new UpdateHotelCommand(id, request.CityId, request.Name, request.Stars, request.Address, request.Email, request.Phone, request.CheckInTime, request.CheckOutTime, request.Latitude, request.Longitude, request.Margin));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteHotelCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:long}/enabled")]
    public async Task<IActionResult> SetEnabled(long id, SetEnabledRequest request)
    {
        await _sender.Send(new SetHotelEnabledCommand(id, request.Enabled));
        return NoContent();
    }
}
