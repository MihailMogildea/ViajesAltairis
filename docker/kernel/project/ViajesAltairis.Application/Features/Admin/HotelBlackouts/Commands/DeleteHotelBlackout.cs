using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelBlackouts.Commands;

public record DeleteHotelBlackoutCommand(long Id) : IRequest;

public class DeleteHotelBlackoutHandler : IRequestHandler<DeleteHotelBlackoutCommand>
{
    private readonly IRepository<HotelBlackout> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHotelBlackoutHandler(IRepository<HotelBlackout> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteHotelBlackoutCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"HotelBlackout {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
