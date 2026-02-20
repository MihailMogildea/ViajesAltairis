using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeAmenities.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeAmenities.Commands;

public record AssignHotelProviderRoomTypeAmenityCommand(long HotelProviderRoomTypeId, long AmenityId) : IRequest<HotelProviderRoomTypeAmenityDto>;

public class AssignHotelProviderRoomTypeAmenityHandler : IRequestHandler<AssignHotelProviderRoomTypeAmenityCommand, HotelProviderRoomTypeAmenityDto>
{
    private readonly IRepository<HotelProviderRoomTypeAmenity> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignHotelProviderRoomTypeAmenityHandler(IRepository<HotelProviderRoomTypeAmenity> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<HotelProviderRoomTypeAmenityDto> Handle(AssignHotelProviderRoomTypeAmenityCommand request, CancellationToken cancellationToken)
    {
        var entity = new HotelProviderRoomTypeAmenity
        {
            HotelProviderRoomTypeId = request.HotelProviderRoomTypeId,
            AmenityId = request.AmenityId
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new HotelProviderRoomTypeAmenityDto(entity.Id, entity.HotelProviderRoomTypeId, entity.AmenityId, entity.CreatedAt);
    }
}
