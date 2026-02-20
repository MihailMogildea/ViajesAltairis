using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Commands;

public record CreateHotelProviderRoomTypeCommand(long HotelProviderId, long RoomTypeId, byte Capacity, int Quantity, decimal PricePerNight, long CurrencyId, long ExchangeRateId) : IRequest<HotelProviderRoomTypeDto>, IInvalidatesCache
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
            Capacity = request.Capacity,
            Quantity = request.Quantity,
            PricePerNight = request.PricePerNight,
            CurrencyId = request.CurrencyId,
            ExchangeRateId = request.ExchangeRateId,
            Enabled = true
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new HotelProviderRoomTypeDto(entity.Id, entity.HotelProviderId, entity.RoomTypeId, entity.Capacity, entity.Quantity, entity.PricePerNight, entity.CurrencyId, entity.ExchangeRateId, entity.Enabled, entity.CreatedAt);
    }
}
