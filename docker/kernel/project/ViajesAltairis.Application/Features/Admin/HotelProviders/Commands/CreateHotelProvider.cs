using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelProviders.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviders.Commands;

public record CreateHotelProviderCommand(long HotelId, long ProviderId) : IRequest<HotelProviderDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class CreateHotelProviderHandler : IRequestHandler<CreateHotelProviderCommand, HotelProviderDto>
{
    private readonly IRepository<HotelProvider> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateHotelProviderHandler(IRepository<HotelProvider> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<HotelProviderDto> Handle(CreateHotelProviderCommand request, CancellationToken cancellationToken)
    {
        var entity = new HotelProvider
        {
            HotelId = request.HotelId,
            ProviderId = request.ProviderId,
            Enabled = true
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new HotelProviderDto(entity.Id, entity.HotelId, entity.ProviderId, entity.Enabled, entity.CreatedAt);
    }
}
