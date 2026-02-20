using FluentValidation.TestHelper;
using ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;
using ViajesAltairis.Application.Features.ExternalClient.Reservations.Validators;

namespace ViajesAltairis.ExternalClient.Api.Tests.Validators;

public class CreatePartnerDraftValidatorTests
{
    private readonly CreatePartnerDraftValidator _validator = new();

    private static CreatePartnerDraftCommand ValidCommand() => new(
        OwnerFirstName: "John",
        OwnerLastName: "Doe",
        OwnerEmail: "john@example.com",
        OwnerPhone: "+34600000000",
        OwnerTaxId: "B12345678",
        CurrencyCode: "EUR",
        PromoCode: null);

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyOwnerFirstName_Fails()
    {
        var cmd = ValidCommand() with { OwnerFirstName = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.OwnerFirstName);
    }

    [Fact]
    public void EmptyOwnerLastName_Fails()
    {
        var cmd = ValidCommand() with { OwnerLastName = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.OwnerLastName);
    }

    [Fact]
    public void InvalidEmail_Fails()
    {
        var cmd = ValidCommand() with { OwnerEmail = "not-an-email" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.OwnerEmail);
    }

    [Fact]
    public void EmptyCurrencyCode_Fails()
    {
        var cmd = ValidCommand() with { CurrencyCode = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.CurrencyCode);
    }
}
