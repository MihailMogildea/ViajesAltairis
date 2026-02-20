using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.UserSubscriptions.Commands;

public record RemoveUserSubscriptionCommand(long Id) : IRequest;

public class RemoveUserSubscriptionHandler : IRequestHandler<RemoveUserSubscriptionCommand>
{
    private readonly IRepository<UserSubscription> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveUserSubscriptionHandler(IRepository<UserSubscription> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveUserSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"UserSubscription {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
