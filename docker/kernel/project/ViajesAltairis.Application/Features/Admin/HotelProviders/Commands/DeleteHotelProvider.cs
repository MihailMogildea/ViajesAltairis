using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviders.Commands;

public record DeleteHotelProviderCommand(long Id) : IRequest, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class DeleteHotelProviderHandler : IRequestHandler<DeleteHotelProviderCommand>
{
    private readonly IRepository<HotelProvider> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHotelProviderHandler(IRepository<HotelProvider> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteHotelProviderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"HotelProvider {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
