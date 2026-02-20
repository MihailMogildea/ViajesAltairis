namespace ViajesAltairis.ProvidersApi.ExternalClients.HotelBeds;

public class HotelBedsClient : IExternalProviderClient
{
    public string ProviderName => "HotelBeds";

    private readonly HotelBedsMapper _mapper = new();

    private static HbHotelListResponse BuildHotelCatalog() => new()
    {
        Hotels =
        [
            new HbHotel
            {
                Code = "HB-PAL-001", Name = "Hotel Altairis Palma",
                Destination = new HbDestination { CityName = "Palma", CountryCode = "ES" },
                CategoryCode = 5,
                Address = new HbAddress { Street = "Paseo Marítimo 12" },
                Contacts = [new() { Type = "EMAIL", Value = "palma@altairis.com" }, new() { Type = "PHONE", Value = "+34 971 000 001" }],
                Rooms =
                [
                    new HbRoom { RoomCode = "SGL", MaxPax = 1, Units = 15, NetRate = 95.00m, Boards = [new() { BoardCode = "RO", Supplement = 0 }, new() { BoardCode = "BB", Supplement = 18 }, new() { BoardCode = "HB", Supplement = 35 }] },
                    new HbRoom { RoomCode = "DBL", MaxPax = 2, Units = 30, NetRate = 160.00m, Boards = [new() { BoardCode = "RO", Supplement = 0 }, new() { BoardCode = "BB", Supplement = 22 }, new() { BoardCode = "HB", Supplement = 45 }, new() { BoardCode = "FB", Supplement = 65 }] },
                    new HbRoom { RoomCode = "STE", MaxPax = 3, Units = 5, NetRate = 320.00m, Boards = [new() { BoardCode = "RO", Supplement = 0 }, new() { BoardCode = "BB", Supplement = 25 }, new() { BoardCode = "FB", Supplement = 70 }, new() { BoardCode = "AI", Supplement = 95 }] }
                ]
            },
            new HbHotel
            {
                Code = "HB-PAL-002", Name = "Hotel Sol de Palma",
                Destination = new HbDestination { CityName = "Palma", CountryCode = "ES" },
                CategoryCode = 3,
                Address = new HbAddress { Street = "Carrer dels Apuntadors 8" },
                Contacts = [new() { Type = "EMAIL", Value = "solpalma@altairis.com" }, new() { Type = "PHONE", Value = "+34 971 000 002" }],
                Rooms =
                [
                    new HbRoom { RoomCode = "SGL", MaxPax = 1, Units = 10, NetRate = 55.00m, Boards = [new() { BoardCode = "RO", Supplement = 0 }, new() { BoardCode = "BB", Supplement = 12 }] },
                    new HbRoom { RoomCode = "DBL", MaxPax = 2, Units = 20, NetRate = 85.00m, Boards = [new() { BoardCode = "RO", Supplement = 0 }, new() { BoardCode = "BB", Supplement = 15 }, new() { BoardCode = "HB", Supplement = 28 }] },
                    new HbRoom { RoomCode = "TWN", MaxPax = 2, Units = 8, NetRate = 80.00m, Boards = [new() { BoardCode = "RO", Supplement = 0 }, new() { BoardCode = "BB", Supplement = 15 }] }
                ]
            },
            new HbHotel
            {
                Code = "HB-CAL-001", Name = "Hotel Calvià Bay",
                Destination = new HbDestination { CityName = "Calvià", CountryCode = "ES" },
                CategoryCode = 4,
                Address = new HbAddress { Street = "Avinguda de les Palmeres 25" },
                Contacts = [new() { Type = "EMAIL", Value = "calvia@hotelbeds-demo.com" }, new() { Type = "PHONE", Value = "+34 971 100 001" }],
                Rooms =
                [
                    new HbRoom { RoomCode = "SGL", MaxPax = 1, Units = 12, NetRate = 72.00m, Boards = [new() { BoardCode = "RO", Supplement = 0 }, new() { BoardCode = "BB", Supplement = 14 }] },
                    new HbRoom { RoomCode = "DBL", MaxPax = 2, Units = 25, NetRate = 120.00m, Boards = [new() { BoardCode = "RO", Supplement = 0 }, new() { BoardCode = "BB", Supplement = 18 }, new() { BoardCode = "HB", Supplement = 32 }, new() { BoardCode = "FB", Supplement = 50 }] },
                    new HbRoom { RoomCode = "JST", MaxPax = 2, Units = 6, NetRate = 195.00m, Boards = [new() { BoardCode = "RO", Supplement = 0 }, new() { BoardCode = "BB", Supplement = 20 }, new() { BoardCode = "HB", Supplement = 38 }] }
                ]
            },
            new HbHotel
            {
                Code = "HB-POL-001", Name = "Hotel Cap de Formentor",
                Destination = new HbDestination { CityName = "Pollença", CountryCode = "ES" },
                CategoryCode = 5,
                Address = new HbAddress { Street = "Carretera del Far 1" },
                Contacts = [new() { Type = "EMAIL", Value = "formentor@hotelbeds-demo.com" }, new() { Type = "PHONE", Value = "+34 971 100 002" }],
                Rooms =
                [
                    new HbRoom { RoomCode = "DBL", MaxPax = 2, Units = 20, NetRate = 210.00m, Boards = [new() { BoardCode = "RO", Supplement = 0 }, new() { BoardCode = "BB", Supplement = 25 }, new() { BoardCode = "HB", Supplement = 48 }, new() { BoardCode = "FB", Supplement = 72 }] },
                    new HbRoom { RoomCode = "STE", MaxPax = 3, Units = 8, NetRate = 380.00m, Boards = [new() { BoardCode = "RO", Supplement = 0 }, new() { BoardCode = "BB", Supplement = 28 }, new() { BoardCode = "FB", Supplement = 75 }, new() { BoardCode = "AI", Supplement = 105 }] },
                    new HbRoom { RoomCode = "DLX", MaxPax = 2, Units = 4, NetRate = 450.00m, Boards = [new() { BoardCode = "RO", Supplement = 0 }, new() { BoardCode = "AI", Supplement = 110 }] }
                ]
            }
        ]
    };

    public async Task<IEnumerable<ExternalHotel>> GetHotelsAsync()
    {
        await Task.Delay(150); // simulate network
        return _mapper.MapHotels(BuildHotelCatalog());
    }

    public async Task<ExternalAvailabilityResponse> SearchAvailabilityAsync(AvailabilityRequest request)
    {
        await Task.Delay(200);
        var catalog = BuildHotelCatalog();
        var filtered = catalog.Hotels
            .Where(h => h.Destination.CityName.Equals(request.City, StringComparison.OrdinalIgnoreCase))
            .Where(h => h.Rooms.Any(r => r.MaxPax >= request.Guests))
            .ToList();

        var response = new HbAvailabilityResponse
        {
            Hotels = filtered.Select(h => new HbAvailHotel
            {
                Name = h.Name,
                CityName = h.Destination.CityName,
                CategoryCode = h.CategoryCode,
                Rooms = h.Rooms.Where(r => r.MaxPax >= request.Guests).ToList()
            }).ToList()
        };
        return _mapper.MapAvailability(response);
    }

    public async Task<ExternalBookingResult> BookAsync(BookingRequest request)
    {
        await Task.Delay(200);
        var result = new HbBookingResponse
        {
            Status = "CONFIRMED",
            Reference = $"HB-{Guid.NewGuid().ToString()[..8].ToUpper()}"
        };
        return _mapper.MapBooking(result);
    }

    public async Task<ExternalCancellationResult> CancelAsync(string bookingReference)
    {
        await Task.Delay(100);
        return new ExternalCancellationResult(Success: true, Error: null);
    }
}
