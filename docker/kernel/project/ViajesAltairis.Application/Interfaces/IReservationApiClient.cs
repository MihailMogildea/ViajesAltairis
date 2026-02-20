namespace ViajesAltairis.Application.Interfaces;

/// <summary>
/// HTTP client abstraction for delegating reservation operations to reservations-api.
/// Used by client-api and external-client-api instead of direct DB access.
/// </summary>
public interface IReservationApiClient
{
    Task<long> CreateDraftAsync(
        long userId, string currencyCode, string? promoCode,
        long? ownerUserId = null,
        string? ownerFirstName = null, string? ownerLastName = null,
        string? ownerEmail = null, string? ownerPhone = null,
        string? ownerTaxId = null, string? ownerAddress = null,
        string? ownerCity = null, string? ownerPostalCode = null,
        string? ownerCountry = null,
        CancellationToken cancellationToken = default);
    Task<long> AddLineAsync(long reservationId, long roomConfigurationId, long boardTypeId, DateTime checkIn, DateTime checkOut, int guestCount, CancellationToken cancellationToken = default);
    Task RemoveLineAsync(long reservationId, long lineId, CancellationToken cancellationToken = default);
    Task<SubmitResult> SubmitAsync(long reservationId, long paymentMethodId, string? cardNumber, string? cardExpiry, string? cardCvv, string? cardHolderName, CancellationToken cancellationToken = default);
    Task CancelAsync(long reservationId, long cancelledByUserId, string? reason, CancellationToken cancellationToken = default);
    Task<ReservationDetailResult?> GetByIdAsync(long reservationId, CancellationToken cancellationToken = default);
    Task<ReservationListResult> GetByUserAsync(long userId, int page, int pageSize, string? status, CancellationToken cancellationToken = default);
    Task AddGuestAsync(long reservationId, long lineId, string firstName, string lastName, string? email, string? phone, CancellationToken cancellationToken = default);
    Task<InvoiceListResult> GetInvoicesByUserAsync(long userId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<InvoiceDetailResult?> GetInvoiceByIdAsync(long invoiceId, long userId, CancellationToken cancellationToken = default);
    Task<ReservationLineInfoResult?> GetReservationLineInfoAsync(long reservationLineId, CancellationToken cancellationToken = default);
}

public record SubmitResult(long ReservationId, string Status, decimal TotalAmount, string CurrencyCode);

public record ReservationDetailResult(
    long Id,
    long BookedByUserId,
    string Status,
    DateTime CreatedAt,
    decimal TotalAmount,
    decimal TotalDiscount,
    string CurrencyCode,
    decimal ExchangeRate,
    string? PromoCode,
    List<ReservationLineResult> Lines);

public record ReservationLineResult(
    long Id,
    string HotelName,
    string RoomType,
    string BoardType,
    DateTime CheckIn,
    DateTime CheckOut,
    int GuestCount,
    decimal LineTotal,
    List<ReservationGuestResult> Guests);

public record ReservationGuestResult(long Id, string FirstName, string LastName, string? DocumentNumber);

public record ReservationListResult(List<ReservationSummaryResult> Reservations, int TotalCount);

public record ReservationSummaryResult(
    long Id,
    string Status,
    DateTime CreatedAt,
    decimal TotalAmount,
    string CurrencyCode,
    int LineCount);

public record InvoiceListResult(List<InvoiceSummaryResult> Invoices, int TotalCount);

public record InvoiceSummaryResult(
    long Id,
    string InvoiceNumber,
    string Status,
    decimal TotalAmount,
    string Currency,
    DateTime IssuedAt);

public record InvoiceDetailResult(
    long Id,
    string InvoiceNumber,
    string Status,
    decimal SubTotal,
    decimal TaxAmount,
    decimal TotalAmount,
    string Currency,
    decimal ExchangeRateToEur,
    DateTime IssuedAt,
    DateTime? PaidAt,
    long ReservationId);

public record ReservationLineInfoResult(
    long ReservationLineId,
    long ReservationId,
    long HotelId,
    long UserId);
