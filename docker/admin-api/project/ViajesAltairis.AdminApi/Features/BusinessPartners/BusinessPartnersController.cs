using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.BusinessPartners.Commands;
using ViajesAltairis.Application.Features.Admin.BusinessPartners.Dtos;
using ViajesAltairis.Application.Features.Admin.BusinessPartners.Queries;
using ViajesAltairis.Application.Features.Admin.Shared;

namespace ViajesAltairis.AdminApi.Features.BusinessPartners;

[ApiController]
[Route("api/[controller]")]
public class BusinessPartnersController : ControllerBase
{
    private readonly ISender _sender;
    public BusinessPartnersController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetBusinessPartnersQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetBusinessPartnerByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBusinessPartnerRequest request)
    {
        var result = await _sender.Send(new CreateBusinessPartnerCommand(request.CompanyName, request.TaxId, request.Discount, request.Address, request.City, request.PostalCode, request.Country, request.ContactEmail, request.ContactPhone));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateBusinessPartnerRequest request)
    {
        var result = await _sender.Send(new UpdateBusinessPartnerCommand(id, request.CompanyName, request.TaxId, request.Discount, request.Address, request.City, request.PostalCode, request.Country, request.ContactEmail, request.ContactPhone));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteBusinessPartnerCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:long}/enabled")]
    public async Task<IActionResult> SetEnabled(long id, SetEnabledRequest request)
    {
        await _sender.Send(new SetBusinessPartnerEnabledCommand(id, request.Enabled));
        return NoContent();
    }
}
