using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Reviews.Commands;

public record SetReviewVisibleCommand(long Id, bool Visible) : IRequest;

public class SetReviewVisibleHandler : IRequestHandler<SetReviewVisibleCommand>
{
    private readonly IRepository<Review> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SetReviewVisibleHandler(IRepository<Review> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SetReviewVisibleCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Review {request.Id} not found.");
        entity.Visible = request.Visible;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
