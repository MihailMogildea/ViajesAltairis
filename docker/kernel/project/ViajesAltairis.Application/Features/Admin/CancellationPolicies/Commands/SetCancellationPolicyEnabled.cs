using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.CancellationPolicies.Commands;

public record SetCancellationPolicyEnabledCommand(long Id, bool Enabled) : IRequest;

public class SetCancellationPolicyEnabledHandler : IRequestHandler<SetCancellationPolicyEnabledCommand>
{
    private readonly IRepository<CancellationPolicy> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SetCancellationPolicyEnabledHandler(IRepository<CancellationPolicy> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SetCancellationPolicyEnabledCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"CancellationPolicy {request.Id} not found.");
        entity.Enabled = request.Enabled;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
