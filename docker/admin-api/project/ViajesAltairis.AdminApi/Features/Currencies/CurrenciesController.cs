using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.Currencies.Commands;
using ViajesAltairis.Application.Features.Admin.Currencies.Dtos;
using ViajesAltairis.Application.Features.Admin.Currencies.Queries;

namespace ViajesAltairis.AdminApi.Features.Currencies;

[ApiController]
[Route("api/[controller]")]
public class CurrenciesController : ControllerBase
{
    private readonly ISender _sender;
    public CurrenciesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetCurrenciesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetCurrencyByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCurrencyRequest request)
    {
        var result = await _sender.Send(new CreateCurrencyCommand(request.IsoCode, request.Name, request.Symbol));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateCurrencyRequest request)
    {
        var result = await _sender.Send(new UpdateCurrencyCommand(id, request.IsoCode, request.Name, request.Symbol));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteCurrencyCommand(id));
        return NoContent();
    }
}
