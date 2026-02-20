namespace ViajesAltairis.Application.Features.Admin.PaymentMethods.Dtos;

public record PaymentMethodDto(long Id, string Name, int MinDaysBeforeCheckin, bool Enabled, DateTime CreatedAt);
