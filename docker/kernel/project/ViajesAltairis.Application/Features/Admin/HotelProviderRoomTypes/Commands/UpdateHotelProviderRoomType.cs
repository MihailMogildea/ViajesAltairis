using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Commands;

public record UpdateHotelProviderRoomTypeCommand(long Id, long HotelProviderId, long RoomTypeId, int Capacity, int Quantity, decimal PricePerNight, long CurrencyId, long ExchangeRateId) : IRequest<HotelProviderRoomTypeDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class UpdateHotelProviderRoomTypeHandler : IRequestHandler<UpdateHotelProviderRoomTypeCommand, HotelProviderRoomTypeDto>
{
    private readonly IRepository<HotelProviderRoomType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateHotelProviderRoomTypeHandler(IRepository<HotelProviderRoomType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<HotelProviderRoomTypeDto> Handle(UpdateHotelProviderRoomTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"HotelProviderRoomType {request.Id} not found.");
        entity.HotelProviderId = request.HotelProviderId;
        entity.RoomTypeId = request.RoomTypeId;
        entity.Capacity = (byte)request.Capacity;
        entity.Quantity = request.Quantity;
        entity.PricePerNight = request.PricePerNight;
        entity.CurrencyId = request.CurrencyId;
        entity.ExchangeRateId = request.ExchangeRateId;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new HotelProviderRoomTypeDto { Id = entity.Id, HotelProviderId = entity.HotelProviderId, RoomTypeId = entity.RoomTypeId, Capacity = entity.Capacity, Quantity = entity.Quantity, PricePerNight = entity.PricePerNight, CurrencyId = entity.CurrencyId, ExchangeRateId = entity.ExchangeRateId, Enabled = entity.Enabled, CreatedAt = entity.CreatedAt };
    }
}
