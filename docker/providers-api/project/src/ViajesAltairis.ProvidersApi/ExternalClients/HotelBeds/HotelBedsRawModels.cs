namespace ViajesAltairis.ProvidersApi.ExternalClients.HotelBeds;

// Simulates HotelBeds nested JSON with code-based fields
public class HbHotelListResponse
{
    public List<HbHotel> Hotels { get; set; } = [];
}

public class HbHotel
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public HbDestination Destination { get; set; } = new();
    public int CategoryCode { get; set; } // stars
    public HbAddress Address { get; set; } = new();
    public List<HbContact> Contacts { get; set; } = [];
    public List<HbRoom> Rooms { get; set; } = [];
}

public class HbDestination
{
    public string CityName { get; set; } = "";
    public string CountryCode { get; set; } = "";
}

public class HbAddress
{
    public string Street { get; set; } = "";
}

public class HbContact
{
    public string Type { get; set; } = ""; // EMAIL, PHONE
    public string Value { get; set; } = "";
}

public class HbRoom
{
    public string RoomCode { get; set; } = ""; // SGL, DBL, TWN, STE, JST, DLX
    public int MaxPax { get; set; }
    public int Units { get; set; }
    public decimal NetRate { get; set; }
    public List<HbBoard> Boards { get; set; } = [];
}

public class HbBoard
{
    public string BoardCode { get; set; } = ""; // RO, BB, HB, FB, AI
    public decimal Supplement { get; set; }
}

public class HbAvailabilityResponse
{
    public List<HbAvailHotel> Hotels { get; set; } = [];
}

public class HbAvailHotel
{
    public string Name { get; set; } = "";
    public string CityName { get; set; } = "";
    public int CategoryCode { get; set; }
    public List<HbRoom> Rooms { get; set; } = [];
}

public class HbBookingResponse
{
    public string Status { get; set; } = "";
    public string Reference { get; set; } = "";
    public string? ErrorMessage { get; set; }
}
