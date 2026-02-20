using MediatR;
using ViajesAltairis.Application.Features.Admin.RoomImages.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.RoomImages.Commands;

public record UpdateRoomImageCommand(long Id, long HotelProviderRoomTypeId, string Url, string? AltText, int SortOrder) : IRequest<RoomImageDto>;

public class UpdateRoomImageHandler : IRequestHandler<UpdateRoomImageCommand, RoomImageDto>
{
    private readonly IRepository<RoomImage> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoomImageHandler(IRepository<RoomImage> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RoomImageDto> Handle(UpdateRoomImageCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"RoomImage {request.Id} not found.");
        entity.HotelProviderRoomTypeId = request.HotelProviderRoomTypeId;
        entity.Url = request.Url;
        entity.AltText = request.AltText;
        entity.SortOrder = request.SortOrder;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new RoomImageDto(entity.Id, entity.HotelProviderRoomTypeId, entity.Url, entity.AltText, entity.SortOrder, entity.CreatedAt);
    }
}
