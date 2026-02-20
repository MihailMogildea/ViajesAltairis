using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Commands;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Dtos;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Queries;

namespace ViajesAltairis.AdminApi.Features.HotelProviderRoomTypeBoards;

[ApiController]
[Route("api/[controller]")]
public class HotelProviderRoomTypeBoardsController : ControllerBase
{
    private readonly ISender _sender;
    public HotelProviderRoomTypeBoardsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetHotelProviderRoomTypeBoardsQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetHotelProviderRoomTypeBoardByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateHotelProviderRoomTypeBoardRequest request)
    {
        var result = await _sender.Send(new CreateHotelProviderRoomTypeBoardCommand(request.HotelProviderRoomTypeId, request.BoardTypeId, request.PricePerNight, request.Enabled));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateHotelProviderRoomTypeBoardRequest request)
    {
        var result = await _sender.Send(new UpdateHotelProviderRoomTypeBoardCommand(id, request.HotelProviderRoomTypeId, request.BoardTypeId, request.PricePerNight, request.Enabled));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteHotelProviderRoomTypeBoardCommand(id));
        return NoContent();
    }
}
