using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.PromoCodes.Commands;
using ViajesAltairis.Application.Features.Admin.PromoCodes.Dtos;
using ViajesAltairis.Application.Features.Admin.PromoCodes.Queries;
using ViajesAltairis.Application.Features.Admin.Shared;

namespace ViajesAltairis.AdminApi.Features.PromoCodes;

[ApiController]
[Route("api/[controller]")]
public class PromoCodesController : ControllerBase
{
    private readonly ISender _sender;
    public PromoCodesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetPromoCodesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetPromoCodeByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePromoCodeRequest request)
    {
        var result = await _sender.Send(new CreatePromoCodeCommand(request.Code, request.DiscountPercentage, request.DiscountAmount, request.CurrencyId, request.ValidFrom, request.ValidTo, request.MaxUses));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdatePromoCodeRequest request)
    {
        var result = await _sender.Send(new UpdatePromoCodeCommand(id, request.Code, request.DiscountPercentage, request.DiscountAmount, request.CurrencyId, request.ValidFrom, request.ValidTo, request.MaxUses));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeletePromoCodeCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:long}/enabled")]
    public async Task<IActionResult> SetEnabled(long id, SetEnabledRequest request)
    {
        await _sender.Send(new SetPromoCodeEnabledCommand(id, request.Enabled));
        return NoContent();
    }
}
