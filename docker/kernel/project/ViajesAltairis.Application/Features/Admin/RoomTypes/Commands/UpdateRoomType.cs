using MediatR;
using ViajesAltairis.Application.Features.Admin.RoomTypes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.RoomTypes.Commands;

public record UpdateRoomTypeCommand(long Id, string Name) : IRequest<RoomTypeDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class UpdateRoomTypeHandler : IRequestHandler<UpdateRoomTypeCommand, RoomTypeDto>
{
    private readonly IRepository<RoomType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoomTypeHandler(IRepository<RoomType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RoomTypeDto> Handle(UpdateRoomTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"RoomType {request.Id} not found.");
        entity.Name = request.Name;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new RoomTypeDto(entity.Id, entity.Name, entity.CreatedAt);
    }
}
