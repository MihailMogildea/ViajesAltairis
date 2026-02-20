namespace ViajesAltairis.Application.Features.Admin.PaymentMethods.Dtos;

public record CreatePaymentMethodRequest(string Name, int MinDaysBeforeCheckin);
