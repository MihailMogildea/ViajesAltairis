using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.HotelProviders.Commands;
using ViajesAltairis.Application.Features.Admin.HotelProviders.Dtos;
using ViajesAltairis.Application.Features.Admin.HotelProviders.Queries;
using ViajesAltairis.Application.Features.Admin.Shared;

namespace ViajesAltairis.AdminApi.Features.HotelProviders;

[ApiController]
[Route("api/[controller]")]
public class HotelProvidersController : ControllerBase
{
    private readonly ISender _sender;
    public HotelProvidersController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetHotelProvidersQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetHotelProviderByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateHotelProviderRequest request)
    {
        var result = await _sender.Send(new CreateHotelProviderCommand(request.HotelId, request.ProviderId));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateHotelProviderRequest request)
    {
        var result = await _sender.Send(new UpdateHotelProviderCommand(id, request.HotelId, request.ProviderId));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteHotelProviderCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:long}/enabled")]
    public async Task<IActionResult> SetEnabled(long id, SetEnabledRequest request)
    {
        await _sender.Send(new SetHotelProviderEnabledCommand(id, request.Enabled));
        return NoContent();
    }
}
