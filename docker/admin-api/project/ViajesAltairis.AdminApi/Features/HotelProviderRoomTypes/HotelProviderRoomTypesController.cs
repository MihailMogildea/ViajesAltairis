using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Commands;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Dtos;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Queries;
using ViajesAltairis.Application.Features.Admin.Shared;

namespace ViajesAltairis.AdminApi.Features.HotelProviderRoomTypes;

[ApiController]
[Route("api/[controller]")]
public class HotelProviderRoomTypesController : ControllerBase
{
    private readonly ISender _sender;
    public HotelProviderRoomTypesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetHotelProviderRoomTypesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetHotelProviderRoomTypeByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateHotelProviderRoomTypeRequest request)
    {
        var result = await _sender.Send(new CreateHotelProviderRoomTypeCommand(request.HotelProviderId, request.RoomTypeId, request.Capacity, request.Quantity, request.PricePerNight, request.CurrencyId, request.ExchangeRateId));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateHotelProviderRoomTypeRequest request)
    {
        var result = await _sender.Send(new UpdateHotelProviderRoomTypeCommand(id, request.HotelProviderId, request.RoomTypeId, request.Capacity, request.Quantity, request.PricePerNight, request.CurrencyId, request.ExchangeRateId));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteHotelProviderRoomTypeCommand(id));
        return NoContent();
    }

    [HttpPatch("{id:long}/enabled")]
    public async Task<IActionResult> SetEnabled(long id, SetEnabledRequest request)
    {
        await _sender.Send(new SetHotelProviderRoomTypeEnabledCommand(id, request.Enabled));
        return NoContent();
    }
}
