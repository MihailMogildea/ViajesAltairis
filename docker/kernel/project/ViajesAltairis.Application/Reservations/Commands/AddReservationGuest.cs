using FluentValidation;
using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Reservations.Commands;

public record AddReservationGuestCommand(
    long ReservationId,
    long LineId,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone) : IRequest;

public class AddReservationGuestHandler : IRequestHandler<AddReservationGuestCommand>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddReservationGuestHandler(
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddReservationGuestCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetWithLinesAsync(request.ReservationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation {request.ReservationId} not found");

        var line = reservation.ReservationLines.FirstOrDefault(l => l.Id == request.LineId)
            ?? throw new KeyNotFoundException($"Reservation line {request.LineId} not found");

        var guest = new ReservationGuest
        {
            ReservationLineId = request.LineId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
        };

        line.ReservationGuests.Add(guest);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public class AddReservationGuestValidator : AbstractValidator<AddReservationGuestCommand>
{
    public AddReservationGuestValidator()
    {
        RuleFor(x => x.ReservationId).GreaterThan(0);
        RuleFor(x => x.LineId).GreaterThan(0);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
    }
}
