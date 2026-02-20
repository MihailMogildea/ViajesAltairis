using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelProviders.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviders.Commands;

public record UpdateHotelProviderCommand(long Id, long HotelId, long ProviderId) : IRequest<HotelProviderDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class UpdateHotelProviderHandler : IRequestHandler<UpdateHotelProviderCommand, HotelProviderDto>
{
    private readonly IRepository<HotelProvider> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateHotelProviderHandler(IRepository<HotelProvider> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<HotelProviderDto> Handle(UpdateHotelProviderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"HotelProvider {request.Id} not found.");
        entity.HotelId = request.HotelId;
        entity.ProviderId = request.ProviderId;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new HotelProviderDto(entity.Id, entity.HotelId, entity.ProviderId, entity.Enabled, entity.CreatedAt);
    }
}
