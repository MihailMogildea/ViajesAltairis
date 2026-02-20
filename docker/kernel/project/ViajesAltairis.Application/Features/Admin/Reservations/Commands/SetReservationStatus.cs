using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Reservations.Commands;

public record SetReservationStatusCommand(long Id, long StatusId) : IRequest;

public class SetReservationStatusHandler : IRequestHandler<SetReservationStatusCommand>
{
    private readonly IRepository<Reservation> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SetReservationStatusHandler(IRepository<Reservation> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SetReservationStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation {request.Id} not found.");
        entity.StatusId = request.StatusId;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
