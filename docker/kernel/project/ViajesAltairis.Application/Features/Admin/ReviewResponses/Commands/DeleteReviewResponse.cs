using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.ReviewResponses.Commands;

public record DeleteReviewResponseCommand(long Id) : IRequest;

public class DeleteReviewResponseHandler : IRequestHandler<DeleteReviewResponseCommand>
{
    private readonly IRepository<ReviewResponse> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteReviewResponseHandler(IRepository<ReviewResponse> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteReviewResponseCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"ReviewResponse {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
