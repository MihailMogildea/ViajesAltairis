using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviders.Commands;

public record SetHotelProviderEnabledCommand(long Id, bool Enabled) : IRequest, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class SetHotelProviderEnabledHandler : IRequestHandler<SetHotelProviderEnabledCommand>
{
    private readonly IRepository<HotelProvider> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SetHotelProviderEnabledHandler(IRepository<HotelProvider> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SetHotelProviderEnabledCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"HotelProvider {request.Id} not found.");
        entity.Enabled = request.Enabled;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
