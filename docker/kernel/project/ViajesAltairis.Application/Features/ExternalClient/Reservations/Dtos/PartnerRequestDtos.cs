namespace ViajesAltairis.Application.Features.ExternalClient.Reservations.Dtos;

public record CreatePartnerDraftRequest(
    string OwnerFirstName, string OwnerLastName, string OwnerEmail,
    string? OwnerPhone, string? OwnerTaxId,
    string CurrencyCode, string? PromoCode);

public record AddPartnerLineRequest(
    long RoomConfigurationId, long BoardTypeId,
    DateOnly CheckIn, DateOnly CheckOut, int GuestCount);

public record AddPartnerGuestRequest(
    string FirstName, string LastName, string? Email, string? Phone);

public record SubmitPartnerReservationRequest(
    long PaymentMethodId, string? CardNumber, string? CardExpiry,
    string? CardCvv, string? CardHolderName);
