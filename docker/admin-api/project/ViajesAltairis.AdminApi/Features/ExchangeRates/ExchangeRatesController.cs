using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.ExchangeRates.Commands;
using ViajesAltairis.Application.Features.Admin.ExchangeRates.Dtos;
using ViajesAltairis.Application.Features.Admin.ExchangeRates.Queries;

namespace ViajesAltairis.AdminApi.Features.ExchangeRates;

[ApiController]
[Route("api/[controller]")]
public class ExchangeRatesController : ControllerBase
{
    private readonly ISender _sender;
    public ExchangeRatesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetExchangeRatesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetExchangeRateByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateExchangeRateRequest request)
    {
        var result = await _sender.Send(new CreateExchangeRateCommand(request.CurrencyId, request.RateToEur, request.ValidFrom, request.ValidTo));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateExchangeRateRequest request)
    {
        var result = await _sender.Send(new UpdateExchangeRateCommand(id, request.CurrencyId, request.RateToEur, request.ValidFrom, request.ValidTo));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteExchangeRateCommand(id));
        return NoContent();
    }
}
