using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeAmenities.Commands;

public record RemoveHotelProviderRoomTypeAmenityCommand(long Id) : IRequest;

public class RemoveHotelProviderRoomTypeAmenityHandler : IRequestHandler<RemoveHotelProviderRoomTypeAmenityCommand>
{
    private readonly IRepository<HotelProviderRoomTypeAmenity> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveHotelProviderRoomTypeAmenityHandler(IRepository<HotelProviderRoomTypeAmenity> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveHotelProviderRoomTypeAmenityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"HotelProviderRoomTypeAmenity {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
