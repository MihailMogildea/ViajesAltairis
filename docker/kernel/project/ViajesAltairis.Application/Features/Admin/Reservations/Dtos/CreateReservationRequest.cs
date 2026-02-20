namespace ViajesAltairis.Application.Features.Admin.Reservations.Dtos;

public record CreateReservationRequest(
    string CurrencyCode,
    string? PromoCode,
    long? OwnerUserId,
    string? OwnerFirstName,
    string? OwnerLastName,
    string? OwnerEmail,
    string? OwnerPhone,
    string? OwnerTaxId,
    string? OwnerAddress,
    string? OwnerCity,
    string? OwnerPostalCode,
    string? OwnerCountry);

public record AddLineRequest(long RoomConfigurationId, long BoardTypeId, DateTime CheckIn, DateTime CheckOut, int GuestCount);

public record AddGuestRequest(string FirstName, string LastName, string? Email, string? Phone);

public record SubmitReservationRequest(long PaymentMethodId, string? CardNumber, string? CardExpiry, string? CardCvv, string? CardHolderName);

public record CancelReservationRequest(string? Reason);
