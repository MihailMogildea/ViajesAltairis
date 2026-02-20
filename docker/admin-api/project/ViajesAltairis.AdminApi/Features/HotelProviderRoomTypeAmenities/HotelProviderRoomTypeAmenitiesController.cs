using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeAmenities.Commands;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeAmenities.Dtos;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeAmenities.Queries;

namespace ViajesAltairis.AdminApi.Features.HotelProviderRoomTypeAmenities;

[ApiController]
[Route("api/[controller]")]
public class HotelProviderRoomTypeAmenitiesController : ControllerBase
{
    private readonly ISender _sender;
    public HotelProviderRoomTypeAmenitiesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] long? hotelProviderRoomTypeId)
        => Ok(await _sender.Send(new GetHotelProviderRoomTypeAmenitiesQuery(hotelProviderRoomTypeId)));

    [HttpPost]
    public async Task<IActionResult> Assign(AssignHotelProviderRoomTypeAmenityRequest request)
    {
        var result = await _sender.Send(new AssignHotelProviderRoomTypeAmenityCommand(request.HotelProviderRoomTypeId, request.AmenityId));
        return Created($"api/hotelproviderroomtypeamenities/{result.Id}", result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Remove(long id)
    {
        await _sender.Send(new RemoveHotelProviderRoomTypeAmenityCommand(id));
        return NoContent();
    }
}
