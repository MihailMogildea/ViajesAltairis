using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.WebTranslations.Commands;
using ViajesAltairis.Application.Features.Admin.WebTranslations.Dtos;
using ViajesAltairis.Application.Features.Admin.WebTranslations.Queries;

namespace ViajesAltairis.AdminApi.Features.WebTranslations;

[ApiController]
[Route("api/[controller]")]
public class WebTranslationsController : ControllerBase
{
    private readonly ISender _sender;
    public WebTranslationsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetWebTranslationsQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetWebTranslationByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("public")]
    public async Task<IActionResult> GetPublic()
        => Ok(await _sender.Send(new GetAdminWebTranslationsPublicQuery()));

    [HttpPost]
    public async Task<IActionResult> Create(CreateWebTranslationRequest request)
    {
        var result = await _sender.Send(new CreateWebTranslationCommand(request.TranslationKey, request.LanguageId, request.Value));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateWebTranslationRequest request)
    {
        var result = await _sender.Send(new UpdateWebTranslationCommand(id, request.TranslationKey, request.LanguageId, request.Value));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteWebTranslationCommand(id));
        return NoContent();
    }
}
