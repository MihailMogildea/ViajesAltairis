using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.RoomImages.Commands;

public record DeleteRoomImageCommand(long Id) : IRequest;

public class DeleteRoomImageHandler : IRequestHandler<DeleteRoomImageCommand>
{
    private readonly IRepository<RoomImage> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoomImageHandler(IRepository<RoomImage> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteRoomImageCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"RoomImage {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
