namespace ViajesAltairis.Application.Features.Admin.Reservations.Dtos;

public class ReservationAdminDto
{
    // Core fields (from view + reservation table)
    public long Id { get; init; }
    public string ReservationCode { get; init; } = null!;
    public long StatusId { get; init; }
    public string StatusName { get; init; } = null!;
    public long BookedByUserId { get; init; }
    public string BookedByFirstName { get; init; } = null!;
    public string BookedByLastName { get; init; } = null!;
    public long? OwnerUserId { get; init; }
    public string OwnerFirstName { get; init; } = null!;
    public string OwnerLastName { get; init; } = null!;
    public string? OwnerEmail { get; init; }
    public string? OwnerPhone { get; init; }
    public string? OwnerTaxId { get; init; }
    public string? OwnerAddress { get; init; }
    public string? OwnerCity { get; init; }
    public string? OwnerPostalCode { get; init; }
    public string? OwnerCountry { get; init; }
    public decimal Subtotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal MarginAmount { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TotalPrice { get; init; }
    public long CurrencyId { get; init; }
    public string CurrencyCode { get; init; } = null!;
    public long ExchangeRateId { get; init; }
    public long? PromoCodeId { get; init; }
    public string? PromoCode { get; init; }
    public string? Notes { get; init; }
    public int LineCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    // Display currency fields (populated when admin's currency differs from reservation's)
    public decimal? DisplaySubtotal { get; set; }
    public decimal? DisplayTaxAmount { get; set; }
    public decimal? DisplayMarginAmount { get; set; }
    public decimal? DisplayDiscountAmount { get; set; }
    public decimal? DisplayTotalPrice { get; set; }
    public string? DisplayCurrencyCode { get; set; }
}
