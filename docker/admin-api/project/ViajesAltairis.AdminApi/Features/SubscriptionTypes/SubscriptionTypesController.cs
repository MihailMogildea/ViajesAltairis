using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Commands;
using ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Dtos;
using ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Queries;
using ViajesAltairis.Application.Features.Admin.Shared;

namespace ViajesAltairis.AdminApi.Features.SubscriptionTypes;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionTypesController : ControllerBase
{
    private readonly ISender _sender;
    public SubscriptionTypesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetSubscriptionTypesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetSubscriptionTypeByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSubscriptionTypeRequest request)
    {
        var result = await _sender.Send(new CreateSubscriptionTypeCommand(request.Name, request.PricePerMonth, request.Discount, request.CurrencyId));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateSubscriptionTypeRequest request)
    {
        var result = await _sender.Send(new UpdateSubscriptionTypeCommand(id, request.Name, request.PricePerMonth, request.Discount, request.CurrencyId));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteSubscriptionTypeCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:long}/enabled")]
    public async Task<IActionResult> SetEnabled(long id, SetEnabledRequest request)
    {
        await _sender.Send(new SetSubscriptionTypeEnabledCommand(id, request.Enabled));
        return NoContent();
    }
}
