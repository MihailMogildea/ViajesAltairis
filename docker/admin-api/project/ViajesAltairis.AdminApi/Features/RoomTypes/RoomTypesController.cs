using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.RoomTypes.Commands;
using ViajesAltairis.Application.Features.Admin.RoomTypes.Dtos;
using ViajesAltairis.Application.Features.Admin.RoomTypes.Queries;

namespace ViajesAltairis.AdminApi.Features.RoomTypes;

[ApiController]
[Route("api/[controller]")]
public class RoomTypesController : ControllerBase
{
    private readonly ISender _sender;
    public RoomTypesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetRoomTypesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetRoomTypeByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRoomTypeRequest request)
    {
        var result = await _sender.Send(new CreateRoomTypeCommand(request.Name));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateRoomTypeRequest request)
    {
        var result = await _sender.Send(new UpdateRoomTypeCommand(id, request.Name));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteRoomTypeCommand(id));
        return NoContent();
    }
}
