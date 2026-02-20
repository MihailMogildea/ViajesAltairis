using MediatR;
using ViajesAltairis.Application.Features.Admin.Hotels.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Hotels.Commands;

public record UpdateHotelCommand(long Id, long CityId, string Name, byte Stars, string Address, string? Email, string? Phone, string CheckInTime, string CheckOutTime, decimal? Latitude, decimal? Longitude, decimal Margin) : IRequest<HotelDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class UpdateHotelHandler : IRequestHandler<UpdateHotelCommand, HotelDto>
{
    private readonly IRepository<Hotel> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateHotelHandler(IRepository<Hotel> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<HotelDto> Handle(UpdateHotelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Hotel {request.Id} not found.");
        entity.CityId = request.CityId;
        entity.Name = request.Name;
        entity.Stars = request.Stars;
        entity.Address = request.Address;
        entity.Email = request.Email;
        entity.Phone = request.Phone;
        entity.CheckInTime = TimeOnly.ParseExact(request.CheckInTime, "HH:mm");
        entity.CheckOutTime = TimeOnly.ParseExact(request.CheckOutTime, "HH:mm");
        entity.Latitude = request.Latitude;
        entity.Longitude = request.Longitude;
        entity.Margin = request.Margin;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new HotelDto(entity.Id, entity.CityId, entity.Name, entity.Stars, entity.Address, entity.Email, entity.Phone, entity.CheckInTime, entity.CheckOutTime, entity.Latitude, entity.Longitude, entity.Margin, entity.Enabled, entity.CreatedAt);
    }
}
