using FluentValidation;

namespace ViajesAltairis.Application.Features.Client.Profile.Commands.UpdateProfile;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.FirstName).MaximumLength(100).When(x => x.FirstName != null);
        RuleFor(x => x.LastName).MaximumLength(100).When(x => x.LastName != null);
    }
}
