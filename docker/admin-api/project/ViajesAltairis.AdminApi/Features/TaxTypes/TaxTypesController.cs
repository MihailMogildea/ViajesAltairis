using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.TaxTypes.Commands;
using ViajesAltairis.Application.Features.Admin.TaxTypes.Dtos;
using ViajesAltairis.Application.Features.Admin.TaxTypes.Queries;

namespace ViajesAltairis.AdminApi.Features.TaxTypes;

[ApiController]
[Route("api/[controller]")]
public class TaxTypesController : ControllerBase
{
    private readonly ISender _sender;
    public TaxTypesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetTaxTypesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetTaxTypeByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaxTypeRequest request)
    {
        var result = await _sender.Send(new CreateTaxTypeCommand(request.Name));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateTaxTypeRequest request)
    {
        var result = await _sender.Send(new UpdateTaxTypeCommand(id, request.Name));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteTaxTypeCommand(id));
        return NoContent();
    }
}
