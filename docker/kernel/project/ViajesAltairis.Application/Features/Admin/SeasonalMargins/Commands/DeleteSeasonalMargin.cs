using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.SeasonalMargins.Commands;

public record DeleteSeasonalMarginCommand(long Id) : IRequest;

public class DeleteSeasonalMarginHandler : IRequestHandler<DeleteSeasonalMarginCommand>
{
    private readonly IRepository<SeasonalMargin> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSeasonalMarginHandler(IRepository<SeasonalMargin> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteSeasonalMarginCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"SeasonalMargin {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
