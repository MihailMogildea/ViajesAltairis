namespace ViajesAltairis.Application.Features.Admin.Reservations.Dtos;

public record ReservationGuestAdminDto(
    long GuestId, long ReservationLineId, string FirstName, string LastName,
    string? Email, string? Phone, string HotelName, string RoomTypeName);
