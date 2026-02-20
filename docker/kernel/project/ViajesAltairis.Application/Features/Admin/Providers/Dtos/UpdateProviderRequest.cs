namespace ViajesAltairis.Application.Features.Admin.Providers.Dtos;

public record UpdateProviderRequest(long TypeId, long CurrencyId, string Name, string? ApiUrl, string? ApiUsername, string? ApiPassword, decimal Margin);
