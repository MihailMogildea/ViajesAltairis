namespace ViajesAltairis.Application.Features.ExternalClient.Reservations.Dtos;

public record ReservationDetailDto(
    long ReservationId, string ReservationCode, string StatusName,
    string OwnerFirstName, string OwnerLastName, string OwnerEmail, string? OwnerPhone, string? OwnerTaxId,
    decimal Subtotal, decimal TaxAmount, decimal DiscountAmount,
    decimal TotalPrice, string CurrencyCode, string? PromoCode, string? Notes, DateTime CreatedAt,
    List<ReservationLineDto> Lines);

public record ReservationLineDto(
    long ReservationLineId, string HotelName, string RoomTypeName, string BoardTypeName,
    DateOnly CheckInDate, DateOnly CheckOutDate, int NumRooms, int NumGuests,
    decimal PricePerNight, decimal BoardPricePerNight, int NumNights, decimal TotalPrice, string CurrencyCode,
    List<GuestDto> Guests);

public record GuestDto(long GuestId, string FirstName, string LastName, string? Email, string? Phone);

public record ReservationSummaryDto(
    long ReservationId, string ReservationCode, string StatusName,
    string OwnerFirstName, string OwnerLastName, string OwnerEmail,
    decimal TotalPrice, string CurrencyCode, int LineCount, DateTime CreatedAt);
