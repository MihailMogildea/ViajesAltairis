using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.Countries.Commands;
using ViajesAltairis.Application.Features.Admin.Countries.Dtos;
using ViajesAltairis.Application.Features.Admin.Countries.Queries;
using ViajesAltairis.Application.Features.Admin.Shared;

namespace ViajesAltairis.AdminApi.Features.Countries;

[ApiController]
[Route("api/[controller]")]
public class CountriesController : ControllerBase
{
    private readonly ISender _sender;
    public CountriesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetCountriesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetCountryByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCountryRequest request)
    {
        var result = await _sender.Send(new CreateCountryCommand(request.IsoCode, request.Name, request.CurrencyId));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateCountryRequest request)
    {
        var result = await _sender.Send(new UpdateCountryCommand(id, request.IsoCode, request.Name, request.CurrencyId));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteCountryCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:long}/enabled")]
    public async Task<IActionResult> SetEnabled(long id, [FromBody] SetEnabledRequest request)
    {
        await _sender.Send(new SetCountryEnabledCommand(id, request.Enabled));
        return NoContent();
    }
}
