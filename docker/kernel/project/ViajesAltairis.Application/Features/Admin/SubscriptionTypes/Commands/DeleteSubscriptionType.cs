using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Commands;

public record DeleteSubscriptionTypeCommand(long Id) : IRequest, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["ref:subscriptions"];
}

public class DeleteSubscriptionTypeHandler : IRequestHandler<DeleteSubscriptionTypeCommand>
{
    private readonly IRepository<SubscriptionType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSubscriptionTypeHandler(IRepository<SubscriptionType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteSubscriptionTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"SubscriptionType {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
