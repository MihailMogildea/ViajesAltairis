using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.BoardTypes.Commands;
using ViajesAltairis.Application.Features.Admin.BoardTypes.Dtos;
using ViajesAltairis.Application.Features.Admin.BoardTypes.Queries;

namespace ViajesAltairis.AdminApi.Features.BoardTypes;

[ApiController]
[Route("api/[controller]")]
public class BoardTypesController : ControllerBase
{
    private readonly ISender _sender;
    public BoardTypesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetBoardTypesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetBoardTypeByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBoardTypeRequest request)
    {
        var result = await _sender.Send(new CreateBoardTypeCommand(request.Name));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateBoardTypeRequest request)
    {
        var result = await _sender.Send(new UpdateBoardTypeCommand(id, request.Name));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteBoardTypeCommand(id));
        return NoContent();
    }
}
