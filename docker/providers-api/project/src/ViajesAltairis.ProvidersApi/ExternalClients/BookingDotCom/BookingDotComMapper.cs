namespace ViajesAltairis.ProvidersApi.ExternalClients.BookingDotCom;

public class BookingDotComMapper : IProviderResponseMapper<BdcPropertyListResponse, BdcAvailabilityResponse, BdcBookingResponse>
{
    private static readonly Dictionary<string, string> RoomTypeMap = new()
    {
        ["standard_single"] = "single",
        ["standard_double"] = "double",
        ["twin"] = "twin",
        ["suite"] = "suite",
        ["junior_suite"] = "junior_suite",
        ["deluxe"] = "deluxe"
    };

    private static readonly Dictionary<string, string> MealPlanMap = new()
    {
        ["none"] = "room_only",
        ["breakfast"] = "bed_and_breakfast",
        ["half_board"] = "half_board",
        ["full_board"] = "full_board",
        ["all_inclusive"] = "all_inclusive"
    };

    public IEnumerable<ExternalHotel> MapHotels(BdcPropertyListResponse raw)
    {
        return raw.Properties.Select(p => new ExternalHotel(
            Name: p.PropertyName,
            CityName: p.City,
            Stars: p.StarRating,
            Address: p.Address,
            Email: p.Email,
            Phone: p.Phone,
            Rooms: p.RoomUnits.Select(r => new ExternalRoom(
                RoomTypeName: RoomTypeMap.GetValueOrDefault(r.RoomType, r.RoomType),
                Capacity: r.MaxOccupancy,
                Quantity: r.TotalUnits,
                PricePerNight: r.BasePrice,
                Boards: r.MealPlans.Select(m => new ExternalBoard(
                    BoardTypeName: MealPlanMap.GetValueOrDefault(m.Plan, m.Plan),
                    PricePerNight: m.AdditionalCost
                )).ToList()
            )).ToList()
        ));
    }

    public ExternalAvailabilityResponse MapAvailability(BdcAvailabilityResponse raw)
    {
        return new ExternalAvailabilityResponse(raw.Properties.Select(p => new AvailableHotel(
            HotelName: p.PropertyName,
            CityName: p.City,
            Stars: p.StarRating,
            Rooms: p.AvailableRooms.Select(r => new AvailableRoom(
                RoomType: RoomTypeMap.GetValueOrDefault(r.RoomType, r.RoomType),
                Capacity: r.MaxOccupancy,
                PricePerNight: r.BasePrice,
                Available: r.TotalUnits,
                Boards: r.MealPlans.Select(m => new AvailableBoard(
                    BoardType: MealPlanMap.GetValueOrDefault(m.Plan, m.Plan),
                    PricePerNight: m.AdditionalCost
                )).ToList()
            )).ToList()
        )).ToList());
    }

    public ExternalBookingResult MapBooking(BdcBookingResponse raw)
    {
        return new ExternalBookingResult(
            Success: raw.BookingStatus == "confirmed",
            BookingReference: raw.ConfirmationNumber,
            Error: raw.Message);
    }
}
