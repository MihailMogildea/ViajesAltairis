using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Commands;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Dtos;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Queries;
using ViajesAltairis.Application.Features.Admin.Shared;

namespace ViajesAltairis.AdminApi.Features.AdministrativeDivisions;

[ApiController]
[Route("api/[controller]")]
public class AdministrativeDivisionsController : ControllerBase
{
    private readonly ISender _sender;
    public AdministrativeDivisionsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetAdministrativeDivisionsQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetAdministrativeDivisionByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAdministrativeDivisionRequest request)
    {
        var result = await _sender.Send(new CreateAdministrativeDivisionCommand(request.CountryId, request.ParentId, request.Name, request.TypeId, request.Level));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateAdministrativeDivisionRequest request)
    {
        var result = await _sender.Send(new UpdateAdministrativeDivisionCommand(id, request.CountryId, request.ParentId, request.Name, request.TypeId, request.Level));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteAdministrativeDivisionCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:long}/enabled")]
    public async Task<IActionResult> SetEnabled(long id, [FromBody] SetEnabledRequest request)
    {
        await _sender.Send(new SetAdministrativeDivisionEnabledCommand(id, request.Enabled));
        return NoContent();
    }
}
