using FluentValidation.TestHelper;
using ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;
using ViajesAltairis.Application.Features.ExternalClient.Reservations.Validators;

namespace ViajesAltairis.ExternalClient.Api.Tests.Validators;

public class CancelPartnerReservationValidatorTests
{
    private readonly CancelPartnerReservationValidator _validator = new();

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var cmd = new CancelPartnerReservationCommand(1, null);
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ReservationId_Zero_Fails()
    {
        var cmd = new CancelPartnerReservationCommand(0, null);
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ReservationId);
    }
}
