using FluentValidation.TestHelper;
using ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;
using ViajesAltairis.Application.Features.ExternalClient.Reservations.Validators;

namespace ViajesAltairis.ExternalClient.Api.Tests.Validators;

public class SubmitPartnerReservationValidatorTests
{
    private readonly SubmitPartnerReservationValidator _validator = new();

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var cmd = new SubmitPartnerReservationCommand(1, 1, null, null, null, null);
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ReservationId_Zero_Fails()
    {
        var cmd = new SubmitPartnerReservationCommand(0, 1, null, null, null, null);
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ReservationId);
    }

    [Fact]
    public void PaymentMethodId_Zero_Fails()
    {
        var cmd = new SubmitPartnerReservationCommand(1, 0, null, null, null, null);
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.PaymentMethodId);
    }
}
