namespace ViajesAltairis.Application.Interfaces;

public interface IInvoicePdfGenerator
{
    byte[] Generate(InvoicePdfData data, Dictionary<string, string> labels);
}

public record InvoicePdfData(
    string InvoiceNumber,
    DateTime IssuedAt,
    DateTime? PaidAt,
    string Status,
    string CustomerName,
    string? CustomerEmail,
    string? CustomerAddress,
    string? CustomerCity,
    string? CustomerPostalCode,
    string? CustomerCountry,
    string? CustomerTaxId,
    List<InvoicePdfLine> Lines,
    decimal Subtotal,
    decimal DiscountAmount,
    decimal TaxAmount,
    decimal TotalAmount,
    string CurrencyCode,
    decimal ExchangeRateToEur);

public record InvoicePdfLine(
    string HotelName,
    string RoomType,
    string BoardType,
    DateTime CheckIn,
    DateTime CheckOut,
    int GuestCount,
    decimal LineTotal);
