using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisionTypes.Commands;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisionTypes.Dtos;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisionTypes.Queries;

namespace ViajesAltairis.AdminApi.Features.AdministrativeDivisionTypes;

[ApiController]
[Route("api/[controller]")]
public class AdministrativeDivisionTypesController : ControllerBase
{
    private readonly ISender _sender;
    public AdministrativeDivisionTypesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetAdministrativeDivisionTypesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetAdministrativeDivisionTypeByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAdministrativeDivisionTypeRequest request)
    {
        var result = await _sender.Send(new CreateAdministrativeDivisionTypeCommand(request.Name));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateAdministrativeDivisionTypeRequest request)
    {
        var result = await _sender.Send(new UpdateAdministrativeDivisionTypeCommand(id, request.Name));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteAdministrativeDivisionTypeCommand(id));
        return NoContent();
    }
}
