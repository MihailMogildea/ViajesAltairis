using MediatR;
using ViajesAltairis.Domain.Enums;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Reservations.Commands;

public record RemoveReservationLineCommand(long ReservationId, long LineId) : IRequest;

public class RemoveReservationLineHandler : IRequestHandler<RemoveReservationLineCommand>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveReservationLineHandler(
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveReservationLineCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _reservationRepository.GetWithLinesAsync(request.ReservationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation {request.ReservationId} not found");

        if (reservation.StatusId != (long)ReservationStatusEnum.Draft)
            throw new InvalidOperationException("Can only remove lines from draft reservations");

        var line = reservation.ReservationLines.FirstOrDefault(l => l.Id == request.LineId)
            ?? throw new KeyNotFoundException($"Reservation line {request.LineId} not found");

        reservation.ReservationLines.Remove(line);

        // Recalculate header totals
        reservation.Subtotal = reservation.ReservationLines.Sum(l => l.Subtotal);
        reservation.TaxAmount = reservation.ReservationLines.Sum(l => l.TaxAmount);
        reservation.MarginAmount = reservation.ReservationLines.Sum(l => l.MarginAmount);
        reservation.DiscountAmount = reservation.ReservationLines.Sum(l => l.DiscountAmount);
        reservation.TotalPrice = reservation.ReservationLines.Sum(l => l.TotalPrice);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
