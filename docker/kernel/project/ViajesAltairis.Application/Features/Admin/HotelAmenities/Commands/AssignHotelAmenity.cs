using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelAmenities.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelAmenities.Commands;

public record AssignHotelAmenityCommand(long HotelId, long AmenityId) : IRequest<HotelAmenityDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class AssignHotelAmenityHandler : IRequestHandler<AssignHotelAmenityCommand, HotelAmenityDto>
{
    private readonly IRepository<HotelAmenity> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignHotelAmenityHandler(IRepository<HotelAmenity> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<HotelAmenityDto> Handle(AssignHotelAmenityCommand request, CancellationToken cancellationToken)
    {
        var entity = new HotelAmenity
        {
            HotelId = request.HotelId,
            AmenityId = request.AmenityId
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new HotelAmenityDto(entity.Id, entity.HotelId, entity.AmenityId, entity.CreatedAt);
    }
}
