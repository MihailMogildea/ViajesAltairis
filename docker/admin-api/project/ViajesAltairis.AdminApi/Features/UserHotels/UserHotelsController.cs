using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.UserHotels.Commands;
using ViajesAltairis.Application.Features.Admin.UserHotels.Dtos;
using ViajesAltairis.Application.Features.Admin.UserHotels.Queries;

namespace ViajesAltairis.AdminApi.Features.UserHotels;

[ApiController]
[Route("api/[controller]")]
public class UserHotelsController : ControllerBase
{
    private readonly ISender _sender;
    public UserHotelsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] long? userId) => Ok(await _sender.Send(new GetUserHotelsQuery(userId)));

    [HttpPost]
    public async Task<IActionResult> Assign(AssignUserHotelRequest request)
    {
        var result = await _sender.Send(new AssignUserHotelCommand(request.UserId, request.HotelId));
        return Created($"api/UserHotels/{result.Id}", result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Remove(long id)
    {
        await _sender.Send(new RemoveUserHotelCommand(id));
        return NoContent();
    }
}
