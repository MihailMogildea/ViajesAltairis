using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.SeasonalMargins.Commands;
using ViajesAltairis.Application.Features.Admin.SeasonalMargins.Dtos;
using ViajesAltairis.Application.Features.Admin.SeasonalMargins.Queries;

namespace ViajesAltairis.AdminApi.Features.SeasonalMargins;

[ApiController]
[Route("api/[controller]")]
public class SeasonalMarginsController : ControllerBase
{
    private readonly ISender _sender;
    public SeasonalMarginsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetSeasonalMarginsQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetSeasonalMarginByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSeasonalMarginRequest request)
    {
        var result = await _sender.Send(new CreateSeasonalMarginCommand(request.AdministrativeDivisionId, request.StartMonthDay, request.EndMonthDay, request.Margin));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateSeasonalMarginRequest request)
    {
        var result = await _sender.Send(new UpdateSeasonalMarginCommand(id, request.AdministrativeDivisionId, request.StartMonthDay, request.EndMonthDay, request.Margin));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteSeasonalMarginCommand(id));
        return NoContent();
    }
}
