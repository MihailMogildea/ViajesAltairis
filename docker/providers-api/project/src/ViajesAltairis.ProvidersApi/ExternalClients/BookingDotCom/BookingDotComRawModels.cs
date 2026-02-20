namespace ViajesAltairis.ProvidersApi.ExternalClients.BookingDotCom;

// Simulates Booking.com flat property-centric JSON
public class BdcPropertyListResponse
{
    public List<BdcProperty> Properties { get; set; } = [];
}

public class BdcProperty
{
    public string PropertyId { get; set; } = "";
    public string PropertyName { get; set; } = "";
    public string City { get; set; } = "";
    public string Country { get; set; } = "";
    public int StarRating { get; set; }
    public string Address { get; set; } = "";
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public List<BdcRoomUnit> RoomUnits { get; set; } = [];
}

public class BdcRoomUnit
{
    public string RoomType { get; set; } = ""; // standard_single, standard_double, twin, suite, junior_suite, deluxe
    public int MaxOccupancy { get; set; }
    public int TotalUnits { get; set; }
    public decimal BasePrice { get; set; }
    public List<BdcMealPlan> MealPlans { get; set; } = [];
}

public class BdcMealPlan
{
    public string Plan { get; set; } = ""; // none, breakfast, half_board, full_board, all_inclusive
    public decimal AdditionalCost { get; set; }
}

public class BdcAvailabilityResponse
{
    public List<BdcAvailProperty> Properties { get; set; } = [];
}

public class BdcAvailProperty
{
    public string PropertyName { get; set; } = "";
    public string City { get; set; } = "";
    public int StarRating { get; set; }
    public List<BdcRoomUnit> AvailableRooms { get; set; } = [];
}

public class BdcBookingResponse
{
    public string BookingStatus { get; set; } = "";
    public string ConfirmationNumber { get; set; } = "";
    public string? Message { get; set; }
}
