using FluentValidation.TestHelper;
using ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;
using ViajesAltairis.Application.Features.ExternalClient.Reservations.Validators;

namespace ViajesAltairis.ExternalClient.Api.Tests.Validators;

public class AddPartnerLineValidatorTests
{
    private readonly AddPartnerLineValidator _validator = new();

    private static AddPartnerLineCommand ValidCommand() => new(
        ReservationId: 1,
        RoomConfigurationId: 10,
        BoardTypeId: 1,
        CheckIn: new DateOnly(2026, 6, 1),
        CheckOut: new DateOnly(2026, 6, 5),
        GuestCount: 2);

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void RoomConfigurationId_Zero_Fails()
    {
        var cmd = ValidCommand() with { RoomConfigurationId = 0 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.RoomConfigurationId);
    }

    [Fact]
    public void BoardTypeId_Zero_Fails()
    {
        var cmd = ValidCommand() with { BoardTypeId = 0 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.BoardTypeId);
    }

    [Fact]
    public void CheckOut_BeforeCheckIn_Fails()
    {
        var cmd = ValidCommand() with { CheckOut = new DateOnly(2026, 5, 30) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.CheckOut);
    }

    [Fact]
    public void GuestCount_Zero_Fails()
    {
        var cmd = ValidCommand() with { GuestCount = 0 };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.GuestCount);
    }
}
