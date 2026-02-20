using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.Providers.Commands;
using ViajesAltairis.Application.Features.Admin.Providers.Dtos;
using ViajesAltairis.Application.Features.Admin.Providers.Queries;
using ViajesAltairis.Application.Features.Admin.Shared;

namespace ViajesAltairis.AdminApi.Features.Providers;

[ApiController]
[Route("api/[controller]")]
public class ProvidersController : ControllerBase
{
    private readonly ISender _sender;
    public ProvidersController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetProvidersQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetProviderByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProviderRequest request)
    {
        var result = await _sender.Send(new CreateProviderCommand(request.TypeId, request.CurrencyId, request.Name, request.ApiUrl, request.ApiUsername, request.ApiPassword, request.Margin));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateProviderRequest request)
    {
        var result = await _sender.Send(new UpdateProviderCommand(id, request.TypeId, request.CurrencyId, request.Name, request.ApiUrl, request.ApiUsername, request.ApiPassword, request.Margin));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteProviderCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:long}/enabled")]
    public async Task<IActionResult> SetEnabled(long id, SetEnabledRequest request)
    {
        await _sender.Send(new SetProviderEnabledCommand(id, request.Enabled));
        return NoContent();
    }
}
