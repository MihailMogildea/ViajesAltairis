using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Commands;

public record SetHotelProviderRoomTypeEnabledCommand(long Id, bool Enabled) : IRequest, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class SetHotelProviderRoomTypeEnabledHandler : IRequestHandler<SetHotelProviderRoomTypeEnabledCommand>
{
    private readonly IRepository<HotelProviderRoomType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SetHotelProviderRoomTypeEnabledHandler(IRepository<HotelProviderRoomType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SetHotelProviderRoomTypeEnabledCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"HotelProviderRoomType {request.Id} not found.");
        entity.Enabled = request.Enabled;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
