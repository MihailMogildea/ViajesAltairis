using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.Translations.Commands;
using ViajesAltairis.Application.Features.Admin.Translations.Dtos;
using ViajesAltairis.Application.Features.Admin.Translations.Queries;

namespace ViajesAltairis.AdminApi.Features.Translations;

[ApiController]
[Route("api/[controller]")]
public class TranslationsController : ControllerBase
{
    private readonly ISender _sender;
    public TranslationsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetTranslationsQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetTranslationByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTranslationRequest request)
    {
        var result = await _sender.Send(new CreateTranslationCommand(request.EntityType, request.EntityId, request.Field, request.LanguageId, request.Value));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateTranslationRequest request)
    {
        var result = await _sender.Send(new UpdateTranslationCommand(id, request.EntityType, request.EntityId, request.Field, request.LanguageId, request.Value));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteTranslationCommand(id));
        return NoContent();
    }
}
