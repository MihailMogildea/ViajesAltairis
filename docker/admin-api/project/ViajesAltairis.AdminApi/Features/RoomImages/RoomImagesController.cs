using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.RoomImages.Commands;
using ViajesAltairis.Application.Features.Admin.RoomImages.Dtos;
using ViajesAltairis.Application.Features.Admin.RoomImages.Queries;

namespace ViajesAltairis.AdminApi.Features.RoomImages;

[ApiController]
[Route("api/[controller]")]
public class RoomImagesController : ControllerBase
{
    private readonly ISender _sender;
    public RoomImagesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetRoomImagesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetRoomImageByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoomImageRequest request)
    {
        var result = await _sender.Send(new CreateRoomImageCommand(request.HotelProviderRoomTypeId, request.Url, request.AltText, request.SortOrder));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateRoomImageRequest request)
    {
        var result = await _sender.Send(new UpdateRoomImageCommand(id, request.HotelProviderRoomTypeId, request.Url, request.AltText, request.SortOrder));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteRoomImageCommand(id));
        return NoContent();
    }
}
