using MediatR;
using ViajesAltairis.Application.Features.Admin.BoardTypes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.BoardTypes.Commands;

public record CreateBoardTypeCommand(string Name) : IRequest<BoardTypeDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class CreateBoardTypeHandler : IRequestHandler<CreateBoardTypeCommand, BoardTypeDto>
{
    private readonly ISimpleRepository<BoardType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBoardTypeHandler(ISimpleRepository<BoardType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BoardTypeDto> Handle(CreateBoardTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = new BoardType
        {
            Name = request.Name
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new BoardTypeDto(entity.Id, entity.Name);
    }
}
