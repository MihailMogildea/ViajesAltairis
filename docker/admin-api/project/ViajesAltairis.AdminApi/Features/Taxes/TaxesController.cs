using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.Shared;
using ViajesAltairis.Application.Features.Admin.Taxes.Commands;
using ViajesAltairis.Application.Features.Admin.Taxes.Dtos;
using ViajesAltairis.Application.Features.Admin.Taxes.Queries;

namespace ViajesAltairis.AdminApi.Features.Taxes;

[ApiController]
[Route("api/[controller]")]
public class TaxesController : ControllerBase
{
    private readonly ISender _sender;
    public TaxesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetTaxesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetTaxByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaxRequest request)
    {
        var result = await _sender.Send(new CreateTaxCommand(request.TaxTypeId, request.CountryId, request.AdministrativeDivisionId, request.CityId, request.Rate, request.IsPercentage));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateTaxRequest request)
    {
        var result = await _sender.Send(new UpdateTaxCommand(id, request.TaxTypeId, request.CountryId, request.AdministrativeDivisionId, request.CityId, request.Rate, request.IsPercentage));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteTaxCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:long}/enabled")]
    public async Task<IActionResult> SetEnabled(long id, SetEnabledRequest request)
    {
        await _sender.Send(new SetTaxEnabledCommand(id, request.Enabled));
        return NoContent();
    }
}
