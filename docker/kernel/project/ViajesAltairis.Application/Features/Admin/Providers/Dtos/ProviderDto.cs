namespace ViajesAltairis.Application.Features.Admin.Providers.Dtos;

public record ProviderDto(long Id, long TypeId, long CurrencyId, string Name, string? ApiUrl, string? ApiUsername, decimal Margin, bool Enabled, string? SyncStatus, DateTime? LastSyncedAt, DateTime CreatedAt);
