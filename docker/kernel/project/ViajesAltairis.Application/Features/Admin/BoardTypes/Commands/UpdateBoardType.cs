using MediatR;
using ViajesAltairis.Application.Features.Admin.BoardTypes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.BoardTypes.Commands;

public record UpdateBoardTypeCommand(long Id, string Name) : IRequest<BoardTypeDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class UpdateBoardTypeHandler : IRequestHandler<UpdateBoardTypeCommand, BoardTypeDto>
{
    private readonly ISimpleRepository<BoardType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBoardTypeHandler(ISimpleRepository<BoardType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BoardTypeDto> Handle(UpdateBoardTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"BoardType {request.Id} not found.");
        entity.Name = request.Name;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new BoardTypeDto(entity.Id, entity.Name);
    }
}
