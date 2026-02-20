using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.ProviderTypes.Commands;
using ViajesAltairis.Application.Features.Admin.ProviderTypes.Dtos;
using ViajesAltairis.Application.Features.Admin.ProviderTypes.Queries;

namespace ViajesAltairis.AdminApi.Features.ProviderTypes;

[ApiController]
[Route("api/[controller]")]
public class ProviderTypesController : ControllerBase
{
    private readonly ISender _sender;
    public ProviderTypesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetProviderTypesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetProviderTypeByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProviderTypeRequest request)
    {
        var result = await _sender.Send(new CreateProviderTypeCommand(request.Name));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateProviderTypeRequest request)
    {
        var result = await _sender.Send(new UpdateProviderTypeCommand(id, request.Name));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteProviderTypeCommand(id));
        return NoContent();
    }
}
