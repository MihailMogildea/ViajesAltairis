using FluentValidation;
using ViajesAltairis.Application.Features.Admin.RoomTypes.Commands;

namespace ViajesAltairis.Application.Features.Admin.RoomTypes.Validators;

public class CreateRoomTypeValidator : AbstractValidator<CreateRoomTypeCommand>
{
    public CreateRoomTypeValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
