using FluentValidation;
using ViajesAltairis.Application.Features.ExternalClient.Reservations.Commands;

namespace ViajesAltairis.Application.Features.ExternalClient.Reservations.Validators;

public class CreatePartnerDraftValidator : AbstractValidator<CreatePartnerDraftCommand>
{
    public CreatePartnerDraftValidator()
    {
        RuleFor(x => x.OwnerFirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.OwnerLastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.OwnerEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.CurrencyCode).NotEmpty().MaximumLength(3);
    }
}
