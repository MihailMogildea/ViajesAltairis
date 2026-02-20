namespace ViajesAltairis.ProvidersApi.ExternalClients;

// --- Normalized DTOs ---

public record ExternalHotel(
    string Name,
    string CityName,
    int Stars,
    string Address,
    string? Email,
    string? Phone,
    List<ExternalRoom> Rooms);

public record ExternalRoom(
    string RoomTypeName,   // normalized: single, double, twin, suite, junior_suite, deluxe
    int Capacity,
    int Quantity,
    decimal PricePerNight,
    List<ExternalBoard> Boards);

public record ExternalBoard(
    string BoardTypeName,  // normalized: room_only, bed_and_breakfast, half_board, full_board, all_inclusive
    decimal PricePerNight);

// --- Request DTOs ---

public record AvailabilityRequest(
    string City,
    DateTime CheckIn,
    DateTime CheckOut,
    int Guests);

public record BookingRequest(
    string HotelName,
    string RoomType,
    string BoardType,
    DateTime CheckIn,
    DateTime CheckOut,
    int Guests,
    string GuestName,
    string GuestEmail);

// --- Response DTOs ---

public record ExternalAvailabilityResponse(List<AvailableHotel> Hotels);

public record AvailableHotel(
    string HotelName,
    string CityName,
    int Stars,
    List<AvailableRoom> Rooms);

public record AvailableRoom(
    string RoomType,
    int Capacity,
    decimal PricePerNight,
    int Available,
    List<AvailableBoard> Boards);

public record AvailableBoard(
    string BoardType,
    decimal PricePerNight);

public record ExternalBookingResult(
    bool Success,
    string? BookingReference,
    string? Error);

public record ExternalCancellationResult(
    bool Success,
    string? Error);
