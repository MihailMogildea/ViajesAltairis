using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.PaymentMethods.Commands;
using ViajesAltairis.Application.Features.Admin.PaymentMethods.Dtos;
using ViajesAltairis.Application.Features.Admin.PaymentMethods.Queries;
using ViajesAltairis.Application.Features.Admin.Shared;

namespace ViajesAltairis.AdminApi.Features.PaymentMethods;

[ApiController]
[Route("api/[controller]")]
public class PaymentMethodsController : ControllerBase
{
    private readonly ISender _sender;
    public PaymentMethodsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetPaymentMethodsQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetPaymentMethodByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePaymentMethodRequest request)
    {
        var result = await _sender.Send(new CreatePaymentMethodCommand(request.Name, request.MinDaysBeforeCheckin));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdatePaymentMethodRequest request)
    {
        var result = await _sender.Send(new UpdatePaymentMethodCommand(id, request.Name, request.MinDaysBeforeCheckin));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeletePaymentMethodCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:long}/enabled")]
    public async Task<IActionResult> SetEnabled(long id, SetEnabledRequest request)
    {
        await _sender.Send(new SetPaymentMethodEnabledCommand(id, request.Enabled));
        return NoContent();
    }
}
