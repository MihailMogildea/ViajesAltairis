using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.Languages.Commands;
using ViajesAltairis.Application.Features.Admin.Languages.Dtos;
using ViajesAltairis.Application.Features.Admin.Languages.Queries;

namespace ViajesAltairis.AdminApi.Features.Languages;

[ApiController]
[Route("api/[controller]")]
public class LanguagesController : ControllerBase
{
    private readonly ISender _sender;
    public LanguagesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetLanguagesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetLanguageByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLanguageRequest request)
    {
        var result = await _sender.Send(new CreateLanguageCommand(request.IsoCode, request.Name));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateLanguageRequest request)
    {
        var result = await _sender.Send(new UpdateLanguageCommand(id, request.IsoCode, request.Name));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteLanguageCommand(id));
        return NoContent();
    }
}
