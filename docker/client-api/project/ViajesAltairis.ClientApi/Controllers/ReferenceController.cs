using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Client.Reference.Queries.GetCountries;
using ViajesAltairis.Application.Features.Client.Reference.Queries.GetCurrencies;
using ViajesAltairis.Application.Features.Client.Reference.Queries.GetHotelTaxes;
using ViajesAltairis.Application.Features.Client.Reference.Queries.GetLanguages;
using ViajesAltairis.Application.Features.Client.Reference.Queries.GetPaymentMethods;
using ViajesAltairis.Application.Features.Client.Reference.Queries.GetWebTranslations;

namespace ViajesAltairis.ClientApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class ReferenceController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReferenceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("languages")]
    public async Task<IActionResult> GetLanguages()
    {
        var result = await _mediator.Send(new GetLanguagesQuery());
        return Ok(result);
    }

    [HttpGet("currencies")]
    public async Task<IActionResult> GetCurrencies()
    {
        var result = await _mediator.Send(new GetCurrenciesQuery());
        return Ok(result);
    }

    [HttpGet("countries")]
    public async Task<IActionResult> GetCountries()
    {
        var result = await _mediator.Send(new GetCountriesQuery());
        return Ok(result);
    }

    [HttpGet("translations")]
    public async Task<IActionResult> GetTranslations()
    {
        var result = await _mediator.Send(new GetWebTranslationsQuery());
        return Ok(result);
    }

    [HttpGet("taxes/{hotelId:long}")]
    public async Task<IActionResult> GetHotelTaxes(long hotelId)
    {
        var result = await _mediator.Send(new GetHotelTaxesQuery { HotelId = hotelId });
        return Ok(result);
    }

    [HttpGet("payment-methods")]
    public async Task<IActionResult> GetPaymentMethods()
    {
        var result = await _mediator.Send(new GetPaymentMethodsQuery());
        return Ok(result);
    }
}
