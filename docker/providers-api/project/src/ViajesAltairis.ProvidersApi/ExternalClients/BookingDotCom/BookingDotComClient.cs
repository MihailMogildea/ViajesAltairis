namespace ViajesAltairis.ProvidersApi.ExternalClients.BookingDotCom;

public class BookingDotComClient : IExternalProviderClient
{
    public string ProviderName => "BookingDotCom";

    private readonly BookingDotComMapper _mapper = new();

    private static BdcPropertyListResponse BuildPropertyCatalog() => new()
    {
        Properties =
        [
            new BdcProperty
            {
                PropertyId = "BDC-BCN-001", PropertyName = "Hotel Altairis Barcelona",
                City = "Barcelona", Country = "ES", StarRating = 5,
                Address = "Passeig de Gràcia 45", Email = "barcelona@altairis.com", Phone = "+34 933 000 001",
                RoomUnits =
                [
                    new BdcRoomUnit { RoomType = "standard_single", MaxOccupancy = 1, TotalUnits = 12, BasePrice = 110.00m, MealPlans = [new() { Plan = "none", AdditionalCost = 0 }, new() { Plan = "breakfast", AdditionalCost = 20 }, new() { Plan = "half_board", AdditionalCost = 40 }] },
                    new BdcRoomUnit { RoomType = "standard_double", MaxOccupancy = 2, TotalUnits = 28, BasePrice = 185.00m, MealPlans = [new() { Plan = "none", AdditionalCost = 0 }, new() { Plan = "breakfast", AdditionalCost = 24 }, new() { Plan = "half_board", AdditionalCost = 48 }, new() { Plan = "full_board", AdditionalCost = 70 }] },
                    new BdcRoomUnit { RoomType = "suite", MaxOccupancy = 3, TotalUnits = 6, BasePrice = 350.00m, MealPlans = [new() { Plan = "none", AdditionalCost = 0 }, new() { Plan = "breakfast", AdditionalCost = 28 }, new() { Plan = "full_board", AdditionalCost = 75 }, new() { Plan = "all_inclusive", AdditionalCost = 100 }] }
                ]
            },
            new BdcProperty
            {
                PropertyId = "BDC-BCN-002", PropertyName = "Hotel Gótico",
                City = "Barcelona", Country = "ES", StarRating = 3,
                Address = "Carrer d'Avinyó 16", Email = "gotico@altairis.com", Phone = "+34 933 000 002",
                RoomUnits =
                [
                    new BdcRoomUnit { RoomType = "standard_single", MaxOccupancy = 1, TotalUnits = 8, BasePrice = 60.00m, MealPlans = [new() { Plan = "none", AdditionalCost = 0 }, new() { Plan = "breakfast", AdditionalCost = 10 }] },
                    new BdcRoomUnit { RoomType = "standard_double", MaxOccupancy = 2, TotalUnits = 15, BasePrice = 95.00m, MealPlans = [new() { Plan = "none", AdditionalCost = 0 }, new() { Plan = "breakfast", AdditionalCost = 14 }, new() { Plan = "half_board", AdditionalCost = 25 }] },
                    new BdcRoomUnit { RoomType = "twin", MaxOccupancy = 2, TotalUnits = 6, BasePrice = 90.00m, MealPlans = [new() { Plan = "none", AdditionalCost = 0 }, new() { Plan = "breakfast", AdditionalCost = 14 }] }
                ]
            },
            new BdcProperty
            {
                PropertyId = "BDC-BCN-003", PropertyName = "Hotel Sagrada Família",
                City = "Barcelona", Country = "ES", StarRating = 4,
                Address = "Carrer de Mallorca 401", Email = "sagrada@bookingdemo.com", Phone = "+34 933 100 001",
                RoomUnits =
                [
                    new BdcRoomUnit { RoomType = "standard_single", MaxOccupancy = 1, TotalUnits = 10, BasePrice = 82.00m, MealPlans = [new() { Plan = "none", AdditionalCost = 0 }, new() { Plan = "breakfast", AdditionalCost = 15 }] },
                    new BdcRoomUnit { RoomType = "standard_double", MaxOccupancy = 2, TotalUnits = 22, BasePrice = 135.00m, MealPlans = [new() { Plan = "none", AdditionalCost = 0 }, new() { Plan = "breakfast", AdditionalCost = 18 }, new() { Plan = "half_board", AdditionalCost = 35 }, new() { Plan = "full_board", AdditionalCost = 55 }] },
                    new BdcRoomUnit { RoomType = "junior_suite", MaxOccupancy = 2, TotalUnits = 4, BasePrice = 220.00m, MealPlans = [new() { Plan = "none", AdditionalCost = 0 }, new() { Plan = "breakfast", AdditionalCost = 20 }, new() { Plan = "half_board", AdditionalCost = 40 }] }
                ]
            },
            new BdcProperty
            {
                PropertyId = "BDC-BCN-004", PropertyName = "Hotel Eixample Boutique",
                City = "Barcelona", Country = "ES", StarRating = 4,
                Address = "Carrer d'Aragó 255", Email = "eixample@bookingdemo.com", Phone = "+34 933 100 002",
                RoomUnits =
                [
                    new BdcRoomUnit { RoomType = "standard_double", MaxOccupancy = 2, TotalUnits = 18, BasePrice = 145.00m, MealPlans = [new() { Plan = "none", AdditionalCost = 0 }, new() { Plan = "breakfast", AdditionalCost = 16 }, new() { Plan = "half_board", AdditionalCost = 32 }] },
                    new BdcRoomUnit { RoomType = "deluxe", MaxOccupancy = 2, TotalUnits = 5, BasePrice = 280.00m, MealPlans = [new() { Plan = "none", AdditionalCost = 0 }, new() { Plan = "breakfast", AdditionalCost = 22 }, new() { Plan = "full_board", AdditionalCost = 60 }] },
                    new BdcRoomUnit { RoomType = "suite", MaxOccupancy = 3, TotalUnits = 3, BasePrice = 390.00m, MealPlans = [new() { Plan = "none", AdditionalCost = 0 }, new() { Plan = "breakfast", AdditionalCost = 25 }, new() { Plan = "all_inclusive", AdditionalCost = 90 }] }
                ]
            }
        ]
    };

    public async Task<IEnumerable<ExternalHotel>> GetHotelsAsync()
    {
        await Task.Delay(120);
        return _mapper.MapHotels(BuildPropertyCatalog());
    }

    public async Task<ExternalAvailabilityResponse> SearchAvailabilityAsync(AvailabilityRequest request)
    {
        await Task.Delay(180);
        var catalog = BuildPropertyCatalog();
        var filtered = catalog.Properties
            .Where(p => p.City.Equals(request.City, StringComparison.OrdinalIgnoreCase))
            .Where(p => p.RoomUnits.Any(r => r.MaxOccupancy >= request.Guests))
            .ToList();

        var response = new BdcAvailabilityResponse
        {
            Properties = filtered.Select(p => new BdcAvailProperty
            {
                PropertyName = p.PropertyName,
                City = p.City,
                StarRating = p.StarRating,
                AvailableRooms = p.RoomUnits.Where(r => r.MaxOccupancy >= request.Guests).ToList()
            }).ToList()
        };
        return _mapper.MapAvailability(response);
    }

    public async Task<ExternalBookingResult> BookAsync(BookingRequest request)
    {
        await Task.Delay(180);
        var result = new BdcBookingResponse
        {
            BookingStatus = "confirmed",
            ConfirmationNumber = $"BDC-{Guid.NewGuid().ToString()[..8].ToUpper()}"
        };
        return _mapper.MapBooking(result);
    }

    public async Task<ExternalCancellationResult> CancelAsync(string bookingReference)
    {
        await Task.Delay(100);
        return new ExternalCancellationResult(Success: true, Error: null);
    }
}
