using FluentValidation;
using ViajesAltairis.Application.Features.Admin.RoomTypes.Commands;

namespace ViajesAltairis.Application.Features.Admin.RoomTypes.Validators;

public class UpdateRoomTypeValidator : AbstractValidator<UpdateRoomTypeCommand>
{
    public UpdateRoomTypeValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
