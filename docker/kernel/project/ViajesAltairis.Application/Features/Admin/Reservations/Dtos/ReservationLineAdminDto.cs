namespace ViajesAltairis.Application.Features.Admin.Reservations.Dtos;

public record ReservationLineAdminDto(
    long ReservationLineId, long ReservationId, long HotelId, string HotelName,
    long RoomTypeId, string RoomTypeName, long BoardTypeId, string BoardTypeName,
    long ProviderId, string ProviderName, DateTime CheckInDate, DateTime CheckOutDate,
    int NumRooms, int NumGuests, decimal PricePerNight, decimal BoardPricePerNight,
    int NumNights, decimal Subtotal, decimal TaxAmount, decimal MarginAmount,
    decimal DiscountAmount, decimal TotalPrice, string CurrencyCode);
