using MediatR;
using ViajesAltairis.Application.Features.Admin.RoomImages.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.RoomImages.Commands;

public record CreateRoomImageCommand(long HotelProviderRoomTypeId, string Url, string? AltText, int SortOrder) : IRequest<RoomImageDto>;

public class CreateRoomImageHandler : IRequestHandler<CreateRoomImageCommand, RoomImageDto>
{
    private readonly IRepository<RoomImage> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoomImageHandler(IRepository<RoomImage> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RoomImageDto> Handle(CreateRoomImageCommand request, CancellationToken cancellationToken)
    {
        var entity = new RoomImage
        {
            HotelProviderRoomTypeId = request.HotelProviderRoomTypeId,
            Url = request.Url,
            AltText = request.AltText,
            SortOrder = request.SortOrder
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new RoomImageDto(entity.Id, entity.HotelProviderRoomTypeId, entity.Url, entity.AltText, entity.SortOrder, entity.CreatedAt);
    }
}
