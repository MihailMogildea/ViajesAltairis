using FluentValidation.TestHelper;
using ViajesAltairis.Application.Features.Admin.JobSchedules.Commands;
using ViajesAltairis.Application.Features.Admin.JobSchedules.Validators;

namespace ViajesAltairis.Scheduled.Api.Tests.Validators;

public class UpdateJobScheduleValidatorTests
{
    private readonly UpdateJobScheduleValidator _validator = new();

    [Fact]
    public void EmptyJobKey_ShouldFail()
    {
        var command = new UpdateJobScheduleCommand("", "0 3 * * *", true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.JobKey);
    }

    [Fact]
    public void EmptyCronExpression_ShouldFail()
    {
        var command = new UpdateJobScheduleCommand("exchange-rate-sync", "", true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CronExpression);
    }

    [Fact]
    public void CronExpressionExceeds50Chars_ShouldFail()
    {
        var command = new UpdateJobScheduleCommand("exchange-rate-sync", new string('x', 51), true);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CronExpression);
    }

    [Fact]
    public void ValidInput_ShouldPass()
    {
        var command = new UpdateJobScheduleCommand("exchange-rate-sync", "0 3 * * *", true);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
