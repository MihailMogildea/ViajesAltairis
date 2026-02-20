namespace ViajesAltairis.ProvidersApi.ExternalClients.TravelGate;

public class TravelGateClient : IExternalProviderClient
{
    public string ProviderName => "TravelGate";

    private readonly TravelGateMapper _mapper = new();

    private static TgHotelListResponse BuildHotelCatalog() => new()
    {
        Data = new TgData
        {
            HotelNodes =
            [
                new TgHotelNode
                {
                    HotelCode = "TG-NCE-001", HotelName = "Hotel Promenade Nice",
                    Location = new TgLocation { City = "Nice", Country = "FR" },
                    Category = 5, AddressLine = "Promenade des Anglais 37",
                    ContactEmail = "nice@altairis.com", ContactPhone = "+33 493 000 001",
                    Options =
                    [
                        new TgRoomOption { RoomCode = "1BED", Occupancy = 1, Allotment = 10, Price = new TgPrice { Net = 105.00m }, MealPlans = [new() { Code = "EP", Supplement = 0 }, new() { Code = "BB", Supplement = 22 }, new() { Code = "MAP", Supplement = 42 }] },
                        new TgRoomOption { RoomCode = "2BED", Occupancy = 2, Allotment = 25, Price = new TgPrice { Net = 175.00m }, MealPlans = [new() { Code = "EP", Supplement = 0 }, new() { Code = "BB", Supplement = 26 }, new() { Code = "MAP", Supplement = 50 }, new() { Code = "AP", Supplement = 72 }] },
                        new TgRoomOption { RoomCode = "SUITE", Occupancy = 3, Allotment = 5, Price = new TgPrice { Net = 340.00m }, MealPlans = [new() { Code = "EP", Supplement = 0 }, new() { Code = "BB", Supplement = 28 }, new() { Code = "AP", Supplement = 78 }, new() { Code = "AI", Supplement = 100 }] }
                    ]
                },
                new TgHotelNode
                {
                    HotelCode = "TG-NCE-002", HotelName = "Hotel Vieux Nice",
                    Location = new TgLocation { City = "Nice", Country = "FR" },
                    Category = 3, AddressLine = "Rue de la Préfecture 12",
                    ContactEmail = "vieuxnice@altairis.com", ContactPhone = "+33 493 000 002",
                    Options =
                    [
                        new TgRoomOption { RoomCode = "1BED", Occupancy = 1, Allotment = 8, Price = new TgPrice { Net = 58.00m }, MealPlans = [new() { Code = "EP", Supplement = 0 }, new() { Code = "BB", Supplement = 11 }] },
                        new TgRoomOption { RoomCode = "2BED", Occupancy = 2, Allotment = 14, Price = new TgPrice { Net = 92.00m }, MealPlans = [new() { Code = "EP", Supplement = 0 }, new() { Code = "BB", Supplement = 15 }, new() { Code = "MAP", Supplement = 28 }] },
                        new TgRoomOption { RoomCode = "TWIN", Occupancy = 2, Allotment = 6, Price = new TgPrice { Net = 88.00m }, MealPlans = [new() { Code = "EP", Supplement = 0 }, new() { Code = "BB", Supplement = 15 }] }
                    ]
                },
                new TgHotelNode
                {
                    HotelCode = "TG-NCE-003", HotelName = "Hotel Masséna Nice",
                    Location = new TgLocation { City = "Nice", Country = "FR" },
                    Category = 4, AddressLine = "Rue Masséna 18",
                    ContactEmail = "massena@travelgate-demo.com", ContactPhone = "+33 493 100 001",
                    Options =
                    [
                        new TgRoomOption { RoomCode = "1BED", Occupancy = 1, Allotment = 10, Price = new TgPrice { Net = 78.00m }, MealPlans = [new() { Code = "EP", Supplement = 0 }, new() { Code = "BB", Supplement = 16 }] },
                        new TgRoomOption { RoomCode = "2BED", Occupancy = 2, Allotment = 20, Price = new TgPrice { Net = 130.00m }, MealPlans = [new() { Code = "EP", Supplement = 0 }, new() { Code = "BB", Supplement = 19 }, new() { Code = "MAP", Supplement = 36 }, new() { Code = "AP", Supplement = 58 }] },
                        new TgRoomOption { RoomCode = "JRSUITE", Occupancy = 2, Allotment = 4, Price = new TgPrice { Net = 210.00m }, MealPlans = [new() { Code = "EP", Supplement = 0 }, new() { Code = "BB", Supplement = 22 }, new() { Code = "MAP", Supplement = 40 }] }
                    ]
                },
                new TgHotelNode
                {
                    HotelCode = "TG-NCE-004", HotelName = "Hotel Riviera Boutique",
                    Location = new TgLocation { City = "Nice", Country = "FR" },
                    Category = 4, AddressLine = "Boulevard Victor Hugo 5",
                    ContactEmail = "riviera@travelgate-demo.com", ContactPhone = "+33 493 100 002",
                    Options =
                    [
                        new TgRoomOption { RoomCode = "2BED", Occupancy = 2, Allotment = 16, Price = new TgPrice { Net = 140.00m }, MealPlans = [new() { Code = "EP", Supplement = 0 }, new() { Code = "BB", Supplement = 18 }, new() { Code = "MAP", Supplement = 34 }] },
                        new TgRoomOption { RoomCode = "DLXE", Occupancy = 2, Allotment = 5, Price = new TgPrice { Net = 265.00m }, MealPlans = [new() { Code = "EP", Supplement = 0 }, new() { Code = "BB", Supplement = 24 }, new() { Code = "AP", Supplement = 65 }] },
                        new TgRoomOption { RoomCode = "SUITE", Occupancy = 3, Allotment = 3, Price = new TgPrice { Net = 370.00m }, MealPlans = [new() { Code = "EP", Supplement = 0 }, new() { Code = "BB", Supplement = 26 }, new() { Code = "AI", Supplement = 95 }] }
                    ]
                }
            ]
        }
    };

    public async Task<IEnumerable<ExternalHotel>> GetHotelsAsync()
    {
        await Task.Delay(180);
        return _mapper.MapHotels(BuildHotelCatalog());
    }

    public async Task<ExternalAvailabilityResponse> SearchAvailabilityAsync(AvailabilityRequest request)
    {
        await Task.Delay(200);
        var catalog = BuildHotelCatalog();
        var filtered = catalog.Data.HotelNodes
            .Where(n => n.Location.City.Equals(request.City, StringComparison.OrdinalIgnoreCase))
            .Where(n => n.Options.Any(o => o.Occupancy >= request.Guests))
            .ToList();

        var response = new TgAvailabilityResponse
        {
            Data = new TgAvailData
            {
                Hotels = filtered.Select(n => new TgAvailHotel
                {
                    HotelName = n.HotelName,
                    City = n.Location.City,
                    Category = n.Category,
                    Options = n.Options.Where(o => o.Occupancy >= request.Guests).ToList()
                }).ToList()
            }
        };
        return _mapper.MapAvailability(response);
    }

    public async Task<ExternalBookingResult> BookAsync(BookingRequest request)
    {
        await Task.Delay(200);
        var result = new TgBookingResponse
        {
            Data = new TgBookingData
            {
                Status = "OK",
                Locator = $"TG-{Guid.NewGuid().ToString()[..8].ToUpper()}"
            }
        };
        return _mapper.MapBooking(result);
    }

    public async Task<ExternalCancellationResult> CancelAsync(string bookingReference)
    {
        await Task.Delay(150);
        return new ExternalCancellationResult(Success: true, Error: null);
    }
}
