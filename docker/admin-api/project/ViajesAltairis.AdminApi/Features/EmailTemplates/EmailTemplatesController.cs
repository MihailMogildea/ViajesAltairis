using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.EmailTemplates.Commands;
using ViajesAltairis.Application.Features.Admin.EmailTemplates.Dtos;
using ViajesAltairis.Application.Features.Admin.EmailTemplates.Queries;

namespace ViajesAltairis.AdminApi.Features.EmailTemplates;

[ApiController]
[Route("api/[controller]")]
public class EmailTemplatesController : ControllerBase
{
    private readonly ISender _sender;
    public EmailTemplatesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetEmailTemplatesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetEmailTemplateByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEmailTemplateRequest request)
    {
        var result = await _sender.Send(new CreateEmailTemplateCommand(request.Name));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateEmailTemplateRequest request)
    {
        var result = await _sender.Send(new UpdateEmailTemplateCommand(id, request.Name));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteEmailTemplateCommand(id));
        return NoContent();
    }
}
