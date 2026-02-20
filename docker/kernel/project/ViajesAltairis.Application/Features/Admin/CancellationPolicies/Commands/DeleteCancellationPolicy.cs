using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.CancellationPolicies.Commands;

public record DeleteCancellationPolicyCommand(long Id) : IRequest;

public class DeleteCancellationPolicyHandler : IRequestHandler<DeleteCancellationPolicyCommand>
{
    private readonly IRepository<CancellationPolicy> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCancellationPolicyHandler(IRepository<CancellationPolicy> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCancellationPolicyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"CancellationPolicy {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
