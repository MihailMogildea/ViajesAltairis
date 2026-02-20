using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.ProvidersApi.Repositories;

namespace ViajesAltairis.ProvidersApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomTypesController : ControllerBase
{
    private readonly IHotelSyncRepository _hotelSyncRepo;

    public RoomTypesController(IHotelSyncRepository hotelSyncRepo)
    {
        _hotelSyncRepo = hotelSyncRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetForHotelProvider([FromQuery] long hotelId, [FromQuery] long providerId)
    {
        var roomTypes = await _hotelSyncRepo.GetRoomTypesForHotelProviderAsync(hotelId, providerId);
        var result = new List<object>();

        foreach (var rt in roomTypes)
        {
            var boards = await _hotelSyncRepo.GetBoardsForRoomTypeAsync(Convert.ToInt64(rt.id));
            result.Add(new
            {
                rt.id,
                rt.room_type,
                rt.capacity,
                rt.quantity,
                rt.price_per_night,
                rt.enabled,
                boards
            });
        }

        return Ok(result);
    }
}
