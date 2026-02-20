using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.Cities.Commands;
using ViajesAltairis.Application.Features.Admin.Cities.Dtos;
using ViajesAltairis.Application.Features.Admin.Cities.Queries;
using ViajesAltairis.Application.Features.Admin.Shared;

namespace ViajesAltairis.AdminApi.Features.Cities;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly ISender _sender;
    public CitiesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetCitiesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetCityByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCityRequest request)
    {
        var result = await _sender.Send(new CreateCityCommand(request.AdministrativeDivisionId, request.Name));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateCityRequest request)
    {
        var result = await _sender.Send(new UpdateCityCommand(id, request.AdministrativeDivisionId, request.Name));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteCityCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:long}/enabled")]
    public async Task<IActionResult> SetEnabled(long id, [FromBody] SetEnabledRequest request)
    {
        await _sender.Send(new SetCityEnabledCommand(id, request.Enabled));
        return NoContent();
    }
}
