using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.CancellationPolicies.Commands;
using ViajesAltairis.Application.Features.Admin.CancellationPolicies.Dtos;
using ViajesAltairis.Application.Features.Admin.CancellationPolicies.Queries;
using ViajesAltairis.Application.Features.Admin.Shared;

namespace ViajesAltairis.AdminApi.Features.CancellationPolicies;

[ApiController]
[Route("api/[controller]")]
public class CancellationPoliciesController : ControllerBase
{
    private readonly ISender _sender;
    public CancellationPoliciesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetCancellationPoliciesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetCancellationPolicyByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCancellationPolicyRequest request)
    {
        var result = await _sender.Send(new CreateCancellationPolicyCommand(request.HotelId, request.FreeCancellationHours, request.PenaltyPercentage));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateCancellationPolicyRequest request)
    {
        var result = await _sender.Send(new UpdateCancellationPolicyCommand(id, request.HotelId, request.FreeCancellationHours, request.PenaltyPercentage));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteCancellationPolicyCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:long}/enabled")]
    public async Task<IActionResult> SetEnabled(long id, SetEnabledRequest request)
    {
        await _sender.Send(new SetCancellationPolicyEnabledCommand(id, request.Enabled));
        return NoContent();
    }
}
