namespace ViajesAltairis.ProvidersApi.ExternalClients.TravelGate;

public class TravelGateMapper : IProviderResponseMapper<TgHotelListResponse, TgAvailabilityResponse, TgBookingResponse>
{
    private static readonly Dictionary<string, string> RoomCodeMap = new()
    {
        ["1BED"] = "single",
        ["2BED"] = "double",
        ["TWIN"] = "twin",
        ["SUITE"] = "suite",
        ["JRSUITE"] = "junior_suite",
        ["DLXE"] = "deluxe"
    };

    private static readonly Dictionary<string, string> MealCodeMap = new()
    {
        ["EP"] = "room_only",
        ["BB"] = "bed_and_breakfast",
        ["MAP"] = "half_board",
        ["AP"] = "full_board",
        ["AI"] = "all_inclusive"
    };

    public IEnumerable<ExternalHotel> MapHotels(TgHotelListResponse raw)
    {
        return raw.Data.HotelNodes.Select(n => new ExternalHotel(
            Name: n.HotelName,
            CityName: n.Location.City,
            Stars: n.Category,
            Address: n.AddressLine,
            Email: n.ContactEmail,
            Phone: n.ContactPhone,
            Rooms: n.Options.Select(o => new ExternalRoom(
                RoomTypeName: RoomCodeMap.GetValueOrDefault(o.RoomCode, o.RoomCode),
                Capacity: o.Occupancy,
                Quantity: o.Allotment,
                PricePerNight: o.Price.Net,
                Boards: o.MealPlans.Select(m => new ExternalBoard(
                    BoardTypeName: MealCodeMap.GetValueOrDefault(m.Code, m.Code),
                    PricePerNight: m.Supplement
                )).ToList()
            )).ToList()
        ));
    }

    public ExternalAvailabilityResponse MapAvailability(TgAvailabilityResponse raw)
    {
        return new ExternalAvailabilityResponse(raw.Data.Hotels.Select(h => new AvailableHotel(
            HotelName: h.HotelName,
            CityName: h.City,
            Stars: h.Category,
            Rooms: h.Options.Select(o => new AvailableRoom(
                RoomType: RoomCodeMap.GetValueOrDefault(o.RoomCode, o.RoomCode),
                Capacity: o.Occupancy,
                PricePerNight: o.Price.Net,
                Available: o.Allotment,
                Boards: o.MealPlans.Select(m => new AvailableBoard(
                    BoardType: MealCodeMap.GetValueOrDefault(m.Code, m.Code),
                    PricePerNight: m.Supplement
                )).ToList()
            )).ToList()
        )).ToList());
    }

    public ExternalBookingResult MapBooking(TgBookingResponse raw)
    {
        return new ExternalBookingResult(
            Success: raw.Data.Status == "OK",
            BookingReference: raw.Data.Locator,
            Error: raw.Data.ErrorDescription);
    }
}
