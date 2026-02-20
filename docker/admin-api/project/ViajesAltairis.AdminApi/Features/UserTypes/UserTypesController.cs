using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.UserTypes.Queries;

namespace ViajesAltairis.AdminApi.Features.UserTypes;

[ApiController]
[Route("api/[controller]")]
public class UserTypesController : ControllerBase
{
    private readonly ISender _sender;
    public UserTypesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetUserTypesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetUserTypeByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }
}
