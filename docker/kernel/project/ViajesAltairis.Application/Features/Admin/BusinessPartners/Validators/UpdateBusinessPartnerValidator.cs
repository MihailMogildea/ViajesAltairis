using FluentValidation;
using ViajesAltairis.Application.Features.Admin.BusinessPartners.Commands;

namespace ViajesAltairis.Application.Features.Admin.BusinessPartners.Validators;

public class UpdateBusinessPartnerValidator : AbstractValidator<UpdateBusinessPartnerCommand>
{
    public UpdateBusinessPartnerValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(300);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ContactEmail).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Discount).GreaterThanOrEqualTo(0);
    }
}
