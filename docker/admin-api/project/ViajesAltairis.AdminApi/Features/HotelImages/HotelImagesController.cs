using MediatR;
using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.Application.Features.Admin.HotelImages.Commands;
using ViajesAltairis.Application.Features.Admin.HotelImages.Dtos;
using ViajesAltairis.Application.Features.Admin.HotelImages.Queries;

namespace ViajesAltairis.AdminApi.Features.HotelImages;

[ApiController]
[Route("api/[controller]")]
public class HotelImagesController : ControllerBase
{
    private readonly ISender _sender;
    public HotelImagesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _sender.Send(new GetHotelImagesQuery()));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _sender.Send(new GetHotelImageByIdQuery(id));
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateHotelImageRequest request)
    {
        var result = await _sender.Send(new CreateHotelImageCommand(request.HotelId, request.Url, request.AltText, request.SortOrder));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, UpdateHotelImageRequest request)
    {
        var result = await _sender.Send(new UpdateHotelImageCommand(id, request.HotelId, request.Url, request.AltText, request.SortOrder));
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _sender.Send(new DeleteHotelImageCommand(id));
        return NoContent();
    }
}
