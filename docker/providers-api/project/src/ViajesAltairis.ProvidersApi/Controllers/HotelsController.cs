using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.ProvidersApi.Repositories;

namespace ViajesAltairis.ProvidersApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IHotelSyncRepository _hotelSyncRepo;

    public HotelsController(IHotelSyncRepository hotelSyncRepo)
    {
        _hotelSyncRepo = hotelSyncRepo;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? city, [FromQuery] int? stars)
    {
        var hotels = await _hotelSyncRepo.SearchHotelsAsync(city, stars);
        return Ok(hotels);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var hotel = await _hotelSyncRepo.GetHotelDetailAsync(id);
        if (hotel == null) return NotFound();
        return Ok(hotel);
    }
}
