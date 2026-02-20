using FluentValidation;
using ViajesAltairis.Application.Features.Admin.JobSchedules.Commands;

namespace ViajesAltairis.Application.Features.Admin.JobSchedules.Validators;

public class UpdateJobScheduleValidator : AbstractValidator<UpdateJobScheduleCommand>
{
    public UpdateJobScheduleValidator()
    {
        RuleFor(x => x.JobKey).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CronExpression).NotEmpty().MaximumLength(50);
    }
}
