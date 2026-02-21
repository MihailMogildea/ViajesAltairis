using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Commands;

public record CreateHotelProviderRoomTypeCommand(long HotelProviderId, long RoomTypeId, int Capacity, int Quantity, decimal PricePerNight, long CurrencyId, long ExchangeRateId) : IRequest<HotelProviderRoomTypeDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class CreateHotelProviderRoomTypeHandler : IRequestHandler<CreateHotelProviderRoomTypeCommand, HotelProviderRoomTypeDto>
{
    private readonly IRepository<HotelProviderRoomType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateHotelProviderRoomTypeHandler(IRepository<HotelProviderRoomType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<HotelProviderRoomTypeDto> Handle(CreateHotelProviderRoomTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = new HotelProviderRoomType
        {
            HotelProviderId = request.HotelProviderId,
            RoomTypeId = request.RoomTypeId,
            Capacity = (byte)request.Capacity,
            Quantity = request.Quantity,
            PricePerNight = request.PricePerNight,
            CurrencyId = request.CurrencyId,
            ExchangeRateId = request.ExchangeRateId,
            Enabled = true
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new HotelProviderRoomTypeDto { Id = entity.Id, HotelProviderId = entity.HotelProviderId, RoomTypeId = entity.RoomTypeId, Capacity = entity.Capacity, Quantity = entity.Quantity, PricePerNight = entity.PricePerNight, CurrencyId = entity.CurrencyId, ExchangeRateId = entity.ExchangeRateId, Enabled = entity.Enabled, CreatedAt = entity.CreatedAt };
    }
}
