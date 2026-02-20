namespace ViajesAltairis.ProvidersApi.ExternalClients.HotelBeds;

public class HotelBedsMapper : IProviderResponseMapper<HbHotelListResponse, HbAvailabilityResponse, HbBookingResponse>
{
    private static readonly Dictionary<string, string> RoomCodeMap = new()
    {
        ["SGL"] = "single",
        ["DBL"] = "double",
        ["TWN"] = "twin",
        ["STE"] = "suite",
        ["JST"] = "junior_suite",
        ["DLX"] = "deluxe"
    };

    private static readonly Dictionary<string, string> BoardCodeMap = new()
    {
        ["RO"] = "room_only",
        ["BB"] = "bed_and_breakfast",
        ["HB"] = "half_board",
        ["FB"] = "full_board",
        ["AI"] = "all_inclusive"
    };

    public IEnumerable<ExternalHotel> MapHotels(HbHotelListResponse raw)
    {
        return raw.Hotels.Select(h => new ExternalHotel(
            Name: h.Name,
            CityName: h.Destination.CityName,
            Stars: h.CategoryCode,
            Address: h.Address.Street,
            Email: h.Contacts.FirstOrDefault(c => c.Type == "EMAIL")?.Value,
            Phone: h.Contacts.FirstOrDefault(c => c.Type == "PHONE")?.Value,
            Rooms: h.Rooms.Select(r => new ExternalRoom(
                RoomTypeName: RoomCodeMap.GetValueOrDefault(r.RoomCode, r.RoomCode),
                Capacity: r.MaxPax,
                Quantity: r.Units,
                PricePerNight: r.NetRate,
                Boards: r.Boards.Select(b => new ExternalBoard(
                    BoardTypeName: BoardCodeMap.GetValueOrDefault(b.BoardCode, b.BoardCode),
                    PricePerNight: b.Supplement
                )).ToList()
            )).ToList()
        ));
    }

    public ExternalAvailabilityResponse MapAvailability(HbAvailabilityResponse raw)
    {
        return new ExternalAvailabilityResponse(raw.Hotels.Select(h => new AvailableHotel(
            HotelName: h.Name,
            CityName: h.CityName,
            Stars: h.CategoryCode,
            Rooms: h.Rooms.Select(r => new AvailableRoom(
                RoomType: RoomCodeMap.GetValueOrDefault(r.RoomCode, r.RoomCode),
                Capacity: r.MaxPax,
                PricePerNight: r.NetRate,
                Available: r.Units,
                Boards: r.Boards.Select(b => new AvailableBoard(
                    BoardType: BoardCodeMap.GetValueOrDefault(b.BoardCode, b.BoardCode),
                    PricePerNight: b.Supplement
                )).ToList()
            )).ToList()
        )).ToList());
    }

    public ExternalBookingResult MapBooking(HbBookingResponse raw)
    {
        return new ExternalBookingResult(
            Success: raw.Status == "CONFIRMED",
            BookingReference: raw.Reference,
            Error: raw.ErrorMessage);
    }
}
