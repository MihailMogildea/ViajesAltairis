using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Commands;

public record CreateHotelProviderRoomTypeBoardCommand(long HotelProviderRoomTypeId, long BoardTypeId, decimal PricePerNight, bool Enabled) : IRequest<HotelProviderRoomTypeBoardDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class CreateHotelProviderRoomTypeBoardHandler : IRequestHandler<CreateHotelProviderRoomTypeBoardCommand, HotelProviderRoomTypeBoardDto>
{
    private readonly ISimpleRepository<HotelProviderRoomTypeBoard> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateHotelProviderRoomTypeBoardHandler(ISimpleRepository<HotelProviderRoomTypeBoard> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<HotelProviderRoomTypeBoardDto> Handle(CreateHotelProviderRoomTypeBoardCommand request, CancellationToken cancellationToken)
    {
        var entity = new HotelProviderRoomTypeBoard
        {
            HotelProviderRoomTypeId = request.HotelProviderRoomTypeId,
            BoardTypeId = request.BoardTypeId,
            PricePerNight = request.PricePerNight,
            Enabled = request.Enabled
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new HotelProviderRoomTypeBoardDto(entity.Id, entity.HotelProviderRoomTypeId, entity.BoardTypeId, entity.PricePerNight, entity.Enabled);
    }
}
