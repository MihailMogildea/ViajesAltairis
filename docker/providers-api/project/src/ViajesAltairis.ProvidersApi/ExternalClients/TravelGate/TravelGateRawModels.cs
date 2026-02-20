namespace ViajesAltairis.ProvidersApi.ExternalClients.TravelGate;

// Simulates TravelGate GraphQL/SOAP-style wrapper
public class TgHotelListResponse
{
    public TgData Data { get; set; } = new();
}

public class TgData
{
    public List<TgHotelNode> HotelNodes { get; set; } = [];
}

public class TgHotelNode
{
    public string HotelCode { get; set; } = "";
    public string HotelName { get; set; } = "";
    public TgLocation Location { get; set; } = new();
    public int Category { get; set; }
    public string AddressLine { get; set; } = "";
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public List<TgRoomOption> Options { get; set; } = [];
}

public class TgLocation
{
    public string City { get; set; } = "";
    public string Country { get; set; } = "";
}

public class TgRoomOption
{
    public string RoomCode { get; set; } = ""; // 1BED, 2BED, TWIN, SUITE, JRSUITE, DLXE
    public int Occupancy { get; set; }
    public int Allotment { get; set; }
    public TgPrice Price { get; set; } = new();
    public List<TgMealPlan> MealPlans { get; set; } = [];
}

public class TgPrice
{
    public decimal Net { get; set; }
    public string Currency { get; set; } = "EUR";
}

public class TgMealPlan
{
    public string Code { get; set; } = ""; // EP, BB, MAP, AP, AI (European Plan = room only, MAP = Modified American Plan = half board, AP = American Plan = full board)
    public decimal Supplement { get; set; }
}

public class TgAvailabilityResponse
{
    public TgAvailData Data { get; set; } = new();
}

public class TgAvailData
{
    public List<TgAvailHotel> Hotels { get; set; } = [];
}

public class TgAvailHotel
{
    public string HotelName { get; set; } = "";
    public string City { get; set; } = "";
    public int Category { get; set; }
    public List<TgRoomOption> Options { get; set; } = [];
}

public class TgBookingResponse
{
    public TgBookingData Data { get; set; } = new();
}

public class TgBookingData
{
    public string Status { get; set; } = "";
    public string Locator { get; set; } = "";
    public string? ErrorDescription { get; set; }
}
