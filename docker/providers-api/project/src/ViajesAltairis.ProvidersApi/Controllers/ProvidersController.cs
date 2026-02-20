using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.ProvidersApi.ExternalClients;
using ViajesAltairis.ProvidersApi.Repositories;
using ViajesAltairis.ProvidersApi.Services;

namespace ViajesAltairis.ProvidersApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProvidersController : ControllerBase
{
    private readonly IProviderRepository _providerRepo;
    private readonly IHotelSyncRepository _hotelSyncRepo;
    private readonly SyncService _syncService;

    public ProvidersController(IProviderRepository providerRepo, IHotelSyncRepository hotelSyncRepo, SyncService syncService)
    {
        _providerRepo = providerRepo;
        _hotelSyncRepo = hotelSyncRepo;
        _syncService = syncService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var providers = await _providerRepo.GetAllEnabledAsync();
        return Ok(providers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var provider = await _providerRepo.GetByIdAsync(id);
        if (provider == null) return NotFound();
        return Ok(provider);
    }

    [HttpPost("{id}/sync")]
    public async Task<IActionResult> Sync(long id)
    {
        var (provider, client, error) = await ResolveExternalClientAsync(id);
        if (error != null) return error;

        if (!await _syncService.TryStartSyncAsync(id))
            return Conflict(new { error = "Sync already in progress" });

        // Fire-and-forget
        _ = Task.Run(() => _syncService.ExecuteSyncAsync(id, client!));

        return Accepted(new { message = "Sync started", providerId = id });
    }

    [HttpGet("{id}/availability")]
    public async Task<IActionResult> SearchAvailability(long id, [FromQuery] string city, [FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut, [FromQuery] int guests)
    {
        var (provider, client, error) = await ResolveExternalClientAsync(id);
        if (error != null) return error;

        var request = new AvailabilityRequest(city, checkIn, checkOut, guests);
        var result = await client!.SearchAvailabilityAsync(request);
        return Ok(result);
    }

    [HttpGet("{id}/availability/hotel/{hotelId}")]
    public async Task<IActionResult> GetHotelAvailability(long id, long hotelId, [FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut, [FromQuery] int guests)
    {
        var (provider, client, error) = await ResolveExternalClientAsync(id);
        if (error != null) return error;

        var hotel = await _hotelSyncRepo.GetHotelDetailAsync(hotelId);
        if (hotel is null)
            return NotFound(new { error = "Hotel not found" });

        string cityName = hotel.city_name;
        string hotelName = hotel.name;

        var request = new AvailabilityRequest(cityName, checkIn, checkOut, guests);
        var result = await client!.SearchAvailabilityAsync(request);

        var matched = result.Hotels
            .FirstOrDefault(h => h.HotelName.Equals(hotelName, StringComparison.OrdinalIgnoreCase));

        if (matched is null)
            return Ok(new { rooms = Array.Empty<AvailableRoom>() });

        return Ok(new { rooms = matched.Rooms });
    }

    [HttpPost("{id}/book")]
    public async Task<IActionResult> Book(long id, [FromBody] BookingRequest request)
    {
        var (provider, client, error) = await ResolveExternalClientAsync(id);
        if (error != null) return error;

        var result = await client!.BookAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}/bookings/{reference}")]
    public async Task<IActionResult> CancelBooking(long id, string reference)
    {
        var (provider, client, error) = await ResolveExternalClientAsync(id);
        if (error != null) return error;

        var result = await client!.CancelAsync(reference);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private async Task<(dynamic? provider, IExternalProviderClient? client, IActionResult? error)> ResolveExternalClientAsync(long id)
    {
        var provider = await _providerRepo.GetByIdAsync(id);
        if (provider == null)
            return (null, null, NotFound());

        string providerType = provider.type.ToString();
        if (providerType.Equals("internal", StringComparison.OrdinalIgnoreCase))
            return (null, null, BadRequest(new { error = "Internal providers do not support external operations" }));

        string providerName = provider.name.ToString();
        var client = _syncService.GetClientForProvider(id, providerName);
        if (client == null)
            return (null, null, BadRequest(new { error = $"No external client registered for provider '{providerName}'" }));

        return (provider, client, null);
    }
}
