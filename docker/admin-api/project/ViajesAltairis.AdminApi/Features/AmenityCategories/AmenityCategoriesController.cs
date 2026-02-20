using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.AmenityCategories.Commands;
using ViajesAltairis.Application.Features.Admin.AmenityCategories.Dtos;
using ViajesAltairis.Application.Features.Admin.AmenityCategories.Queries;

namespace ViajesAltairis.AdminApi.Features.AmenityCategories;

[ApiController]
[Route("api/[controller]")]
public class AmenityCategoriesController : ControllerBase
{
    private readonly ISender _sender;
    public AmenityCategoriesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetAmenityCategoriesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetAmenityCategoryByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAmenityCategoryRequest request)
    {
        var result = await _sender.Send(new CreateAmenityCategoryCommand(request.Name));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateAmenityCategoryRequest request)
    {
        var result = await _sender.Send(new UpdateAmenityCategoryCommand(id, request.Name));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteAmenityCategoryCommand(id));
        return NoContent();
    }
}
