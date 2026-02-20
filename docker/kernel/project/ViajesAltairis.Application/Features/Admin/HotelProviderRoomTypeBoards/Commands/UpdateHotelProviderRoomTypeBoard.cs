using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Commands;

public record UpdateHotelProviderRoomTypeBoardCommand(long Id, long HotelProviderRoomTypeId, long BoardTypeId, decimal PricePerNight, bool Enabled) : IRequest<HotelProviderRoomTypeBoardDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class UpdateHotelProviderRoomTypeBoardHandler : IRequestHandler<UpdateHotelProviderRoomTypeBoardCommand, HotelProviderRoomTypeBoardDto>
{
    private readonly ISimpleRepository<HotelProviderRoomTypeBoard> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateHotelProviderRoomTypeBoardHandler(ISimpleRepository<HotelProviderRoomTypeBoard> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<HotelProviderRoomTypeBoardDto> Handle(UpdateHotelProviderRoomTypeBoardCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"HotelProviderRoomTypeBoard {request.Id} not found.");
        entity.HotelProviderRoomTypeId = request.HotelProviderRoomTypeId;
        entity.BoardTypeId = request.BoardTypeId;
        entity.PricePerNight = request.PricePerNight;
        entity.Enabled = request.Enabled;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new HotelProviderRoomTypeBoardDto(entity.Id, entity.HotelProviderRoomTypeId, entity.BoardTypeId, entity.PricePerNight, entity.Enabled);
    }
}
