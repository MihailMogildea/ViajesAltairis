using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Infrastructure.Services;

public class ReservationApiClient : IReservationApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ReservationApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("ReservationsApi");

    public async Task<long> CreateDraftAsync(
        long userId, string currencyCode, string? promoCode,
        long? ownerUserId = null,
        string? ownerFirstName = null, string? ownerLastName = null,
        string? ownerEmail = null, string? ownerPhone = null,
        string? ownerTaxId = null, string? ownerAddress = null,
        string? ownerCity = null, string? ownerPostalCode = null,
        string? ownerCountry = null,
        CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var payload = new
        {
            UserId = userId,
            CurrencyCode = currencyCode,
            PromoCode = promoCode,
            OwnerUserId = ownerUserId,
            OwnerFirstName = ownerFirstName,
            OwnerLastName = ownerLastName,
            OwnerEmail = ownerEmail,
            OwnerPhone = ownerPhone,
            OwnerTaxId = ownerTaxId,
            OwnerAddress = ownerAddress,
            OwnerCity = ownerCity,
            OwnerPostalCode = ownerPostalCode,
            OwnerCountry = ownerCountry
        };
        var response = await client.PostAsJsonAsync("/api/reservations/draft", payload, cancellationToken);
        await EnsureSuccessOrThrowAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<long>(cancellationToken: cancellationToken);
    }

    public async Task<long> AddLineAsync(long reservationId, long roomConfigurationId, long boardTypeId, DateTime checkIn, DateTime checkOut, int guestCount, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var payload = new { RoomConfigurationId = roomConfigurationId, BoardTypeId = boardTypeId, CheckIn = checkIn, CheckOut = checkOut, GuestCount = guestCount };
        var response = await client.PostAsJsonAsync($"/api/reservations/{reservationId}/lines", payload, cancellationToken);
        await EnsureSuccessOrThrowAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<long>(cancellationToken: cancellationToken);
    }

    public async Task RemoveLineAsync(long reservationId, long lineId, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var response = await client.DeleteAsync($"/api/reservations/{reservationId}/lines/{lineId}", cancellationToken);
        await EnsureSuccessOrThrowAsync(response, cancellationToken);
    }

    public async Task<SubmitResult> SubmitAsync(long reservationId, long paymentMethodId, string? cardNumber, string? cardExpiry, string? cardCvv, string? cardHolderName, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var payload = new { PaymentMethodId = paymentMethodId, CardNumber = cardNumber, CardExpiry = cardExpiry, CardCvv = cardCvv, CardHolderName = cardHolderName };
        var response = await client.PostAsJsonAsync($"/api/reservations/{reservationId}/submit", payload, cancellationToken);
        await EnsureSuccessOrThrowAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<SubmitResult>(cancellationToken: cancellationToken))!;
    }

    public async Task CancelAsync(long reservationId, long cancelledByUserId, string? reason, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var payload = new { CancelledByUserId = cancelledByUserId, Reason = reason };
        var response = await client.PostAsJsonAsync($"/api/reservations/{reservationId}/cancel", payload, cancellationToken);
        await EnsureSuccessOrThrowAsync(response, cancellationToken);
    }

    public async Task<ReservationDetailResult?> GetByIdAsync(long reservationId, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var response = await client.GetAsync($"/api/reservations/{reservationId}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        await EnsureSuccessOrThrowAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<ReservationDetailResult>(cancellationToken: cancellationToken);
    }

    public async Task<ReservationListResult> GetByUserAsync(long userId, int page, int pageSize, string? status, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var url = $"/api/reservations?userId={userId}&page={page}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(status))
            url += $"&status={status}";
        var response = await client.GetAsync(url, cancellationToken);
        await EnsureSuccessOrThrowAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<ReservationListResult>(cancellationToken: cancellationToken))!;
    }

    public async Task AddGuestAsync(long reservationId, long lineId, string firstName, string lastName, string? email, string? phone, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var payload = new { FirstName = firstName, LastName = lastName, Email = email, Phone = phone };
        var response = await client.PostAsJsonAsync($"/api/reservations/{reservationId}/lines/{lineId}/guests", payload, cancellationToken);
        await EnsureSuccessOrThrowAsync(response, cancellationToken);
    }

    public async Task<InvoiceListResult> GetInvoicesByUserAsync(long userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var response = await client.GetAsync($"/api/invoices?userId={userId}&page={page}&pageSize={pageSize}", cancellationToken);
        await EnsureSuccessOrThrowAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<InvoiceListResult>(cancellationToken: cancellationToken))!;
    }

    public async Task<InvoiceDetailResult?> GetInvoiceByIdAsync(long invoiceId, long userId, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var response = await client.GetAsync($"/api/invoices/{invoiceId}?userId={userId}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        await EnsureSuccessOrThrowAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<InvoiceDetailResult>(cancellationToken: cancellationToken);
    }

    public async Task<ReservationLineInfoResult?> GetReservationLineInfoAsync(long reservationLineId, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var response = await client.GetAsync($"/api/reservations/lines/{reservationLineId}/info", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        await EnsureSuccessOrThrowAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<ReservationLineInfoResult>(cancellationToken: cancellationToken);
    }

    public async Task<InvoiceDetailResult> CreateInvoiceAsync(long reservationId, long userId, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var payload = new { ReservationId = reservationId, UserId = userId };
        var response = await client.PostAsJsonAsync("/api/invoices", payload, cancellationToken);
        await EnsureSuccessOrThrowAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<InvoiceDetailResult>(cancellationToken: cancellationToken))!;
    }

    public async Task<byte[]?> GetInvoicePdfAsync(long invoiceId, long userId, long? languageId = null, CancellationToken cancellationToken = default)
    {
        var client = CreateClient();
        var url = $"/api/invoices/{invoiceId}/pdf?userId={userId}";
        if (languageId.HasValue)
            url += $"&languageId={languageId.Value}";
        var response = await client.GetAsync(url, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        await EnsureSuccessOrThrowAsync(response, cancellationToken);
        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    /// <summary>
    /// Reads the error body from reservations-api and throws the appropriate exception type
    /// so that upstream middleware (ExceptionHandlerMiddleware) can map it to the correct HTTP status.
    /// </summary>
    private static async Task EnsureSuccessOrThrowAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var message = TryExtractErrorMessage(body) ?? $"Reservations API returned {(int)response.StatusCode}";

        throw response.StatusCode switch
        {
            HttpStatusCode.NotFound => new KeyNotFoundException(message),
            HttpStatusCode.BadRequest => new InvalidOperationException(message),
            HttpStatusCode.Conflict => new InvalidOperationException(message),
            _ => new HttpRequestException(message, null, response.StatusCode)
        };
    }

    private static string? TryExtractErrorMessage(string body)
    {
        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("error", out var error))
                return error.GetString();
            if (doc.RootElement.TryGetProperty("errors", out var errors))
                return errors.ToString();
        }
        catch (JsonException) { }
        return string.IsNullOrWhiteSpace(body) ? null : body;
    }
}
