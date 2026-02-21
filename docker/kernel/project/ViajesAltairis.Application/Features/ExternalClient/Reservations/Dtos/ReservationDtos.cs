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

public class ReservationSummaryDto
{
    public long ReservationId { get; set; }
    public string ReservationCode { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public string OwnerFirstName { get; set; } = string.Empty;
    public string OwnerLastName { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public int LineCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
