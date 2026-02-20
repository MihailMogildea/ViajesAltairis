using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Commands;

public record SetSubscriptionTypeEnabledCommand(long Id, bool Enabled) : IRequest, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["ref:subscriptions"];
}

public class SetSubscriptionTypeEnabledHandler : IRequestHandler<SetSubscriptionTypeEnabledCommand>
{
    private readonly IRepository<SubscriptionType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SetSubscriptionTypeEnabledHandler(IRepository<SubscriptionType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SetSubscriptionTypeEnabledCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"SubscriptionType {request.Id} not found.");
        entity.Enabled = request.Enabled;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
