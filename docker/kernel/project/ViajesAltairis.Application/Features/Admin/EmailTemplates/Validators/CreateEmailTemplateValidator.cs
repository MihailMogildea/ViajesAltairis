using FluentValidation;
using ViajesAltairis.Application.Features.Admin.EmailTemplates.Commands;

namespace ViajesAltairis.Application.Features.Admin.EmailTemplates.Validators;

public class CreateEmailTemplateValidator : AbstractValidator<CreateEmailTemplateCommand>
{
    public CreateEmailTemplateValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
