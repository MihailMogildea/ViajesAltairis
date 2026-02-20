using MediatR;
using ViajesAltairis.Application.Features.Admin.RoomTypes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.RoomTypes.Commands;

public record CreateRoomTypeCommand(string Name) : IRequest<RoomTypeDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class CreateRoomTypeHandler : IRequestHandler<CreateRoomTypeCommand, RoomTypeDto>
{
    private readonly IRepository<RoomType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoomTypeHandler(IRepository<RoomType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RoomTypeDto> Handle(CreateRoomTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = new RoomType { Name = request.Name };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new RoomTypeDto(entity.Id, entity.Name, entity.CreatedAt);
    }
}
