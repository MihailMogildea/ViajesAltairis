using MediatR;
using ViajesAltairis.Application.Features.Admin.Hotels.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Hotels.Commands;

public record CreateHotelCommand(long CityId, string Name, byte Stars, string Address, string? Email, string? Phone, string CheckInTime, string CheckOutTime, decimal? Latitude, decimal? Longitude, decimal Margin) : IRequest<HotelDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class CreateHotelHandler : IRequestHandler<CreateHotelCommand, HotelDto>
{
    private readonly IRepository<Hotel> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateHotelHandler(IRepository<Hotel> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<HotelDto> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
    {
        var entity = new Hotel
        {
            CityId = request.CityId,
            Name = request.Name,
            Stars = request.Stars,
            Address = request.Address,
            Email = request.Email,
            Phone = request.Phone,
            CheckInTime = TimeOnly.ParseExact(request.CheckInTime, "HH:mm"),
            CheckOutTime = TimeOnly.ParseExact(request.CheckOutTime, "HH:mm"),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Margin = request.Margin,
            Enabled = true
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new HotelDto(entity.Id, entity.CityId, entity.Name, entity.Stars, entity.Address, entity.Email, entity.Phone, entity.CheckInTime, entity.CheckOutTime, entity.Latitude, entity.Longitude, entity.Margin, entity.Enabled, entity.CreatedAt);
    }
}
