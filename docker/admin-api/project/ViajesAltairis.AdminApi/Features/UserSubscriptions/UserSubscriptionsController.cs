using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.UserSubscriptions.Commands;
using ViajesAltairis.Application.Features.Admin.UserSubscriptions.Dtos;
using ViajesAltairis.Application.Features.Admin.UserSubscriptions.Queries;

namespace ViajesAltairis.AdminApi.Features.UserSubscriptions;

[ApiController]
[Route("api/[controller]")]
public class UserSubscriptionsController : ControllerBase
{
    private readonly ISender _sender;
    public UserSubscriptionsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] long? userId) => Ok(await _sender.Send(new GetUserSubscriptionsQuery(userId)));

    [HttpPost]
    public async Task<IActionResult> Assign(AssignUserSubscriptionRequest request)
    {
        var result = await _sender.Send(new AssignUserSubscriptionCommand(request.UserId, request.SubscriptionTypeId, request.StartDate, request.EndDate));
        return Created($"api/UserSubscriptions/{result.Id}", result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Remove(long id)
    {
        await _sender.Send(new RemoveUserSubscriptionCommand(id));
        return NoContent();
    }
}
