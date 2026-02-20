using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.Amenities.Commands;
using ViajesAltairis.Application.Features.Admin.Amenities.Dtos;
using ViajesAltairis.Application.Features.Admin.Amenities.Queries;

namespace ViajesAltairis.AdminApi.Features.Amenities;

[ApiController]
[Route("api/[controller]")]
public class AmenitiesController : ControllerBase
{
    private readonly ISender _sender;
    public AmenitiesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetAmenitiesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetAmenityByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAmenityRequest request)
    {
        var result = await _sender.Send(new CreateAmenityCommand(request.CategoryId, request.Name));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateAmenityRequest request)
    {
        var result = await _sender.Send(new UpdateAmenityCommand(id, request.CategoryId, request.Name));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteAmenityCommand(id));
        return NoContent();
    }
}
