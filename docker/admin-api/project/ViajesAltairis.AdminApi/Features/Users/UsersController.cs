using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.Shared;
using ViajesAltairis.Application.Features.Admin.Users.Commands;
using ViajesAltairis.Application.Features.Admin.Users.Dtos;
using ViajesAltairis.Application.Features.Admin.Users.Queries;

namespace ViajesAltairis.AdminApi.Features.Users;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;
    public UsersController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetUsersQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetUserByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserRequest request)
    {
        var result = await _sender.Send(new CreateUserCommand(request.UserTypeId, request.Email, request.Password, request.FirstName, request.LastName, request.Phone, request.TaxId, request.Address, request.City, request.PostalCode, request.Country, request.LanguageId, request.BusinessPartnerId, request.ProviderId, request.Discount));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateUserRequest request)
    {
        var result = await _sender.Send(new UpdateUserCommand(id, request.UserTypeId, request.Email, request.FirstName, request.LastName, request.Phone, request.TaxId, request.Address, request.City, request.PostalCode, request.Country, request.LanguageId, request.BusinessPartnerId, request.ProviderId, request.Discount));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteUserCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:long}/enabled")]
    public async Task<IActionResult> SetEnabled(long id, SetEnabledRequest request)
    {
        await _sender.Send(new SetUserEnabledCommand(id, request.Enabled));
        return NoContent();
    }
}
