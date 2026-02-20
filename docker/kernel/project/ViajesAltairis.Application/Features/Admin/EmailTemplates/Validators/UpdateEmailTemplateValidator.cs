using FluentValidation;
using ViajesAltairis.Application.Features.Admin.EmailTemplates.Commands;

namespace ViajesAltairis.Application.Features.Admin.EmailTemplates.Validators;

public class UpdateEmailTemplateValidator : AbstractValidator<UpdateEmailTemplateCommand>
{
    public UpdateEmailTemplateValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
