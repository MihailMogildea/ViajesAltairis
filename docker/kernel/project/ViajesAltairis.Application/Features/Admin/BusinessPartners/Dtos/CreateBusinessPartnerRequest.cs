namespace ViajesAltairis.Application.Features.Admin.BusinessPartners.Dtos;

public record CreateBusinessPartnerRequest(string CompanyName, string? TaxId, decimal Discount, string Address, string City, string? PostalCode, string Country, string ContactEmail, string? ContactPhone);
