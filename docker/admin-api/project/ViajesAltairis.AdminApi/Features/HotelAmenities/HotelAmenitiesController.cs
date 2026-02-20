using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.HotelAmenities.Commands;
using ViajesAltairis.Application.Features.Admin.HotelAmenities.Dtos;
using ViajesAltairis.Application.Features.Admin.HotelAmenities.Queries;

namespace ViajesAltairis.AdminApi.Features.HotelAmenities;

[ApiController]
[Route("api/[controller]")]
public class HotelAmenitiesController : ControllerBase
{
    private readonly ISender _sender;
    public HotelAmenitiesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] long? hotelId)
        => Ok(await _sender.Send(new GetHotelAmenitiesQuery(hotelId)));

    [HttpPost]
    public async Task<IActionResult> Assign(AssignHotelAmenityRequest request)
    {
        var result = await _sender.Send(new AssignHotelAmenityCommand(request.HotelId, request.AmenityId));
        return Created($"api/hotelamenities/{result.Id}", result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Remove(long id)
    {
        await _sender.Send(new RemoveHotelAmenityCommand(id));
        return NoContent();
    }
}
