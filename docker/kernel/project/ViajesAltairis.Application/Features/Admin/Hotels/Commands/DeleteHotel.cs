using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Hotels.Commands;

public record DeleteHotelCommand(long Id) : IRequest, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class DeleteHotelHandler : IRequestHandler<DeleteHotelCommand>
{
    private readonly IRepository<Hotel> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHotelHandler(IRepository<Hotel> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteHotelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Hotel {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
