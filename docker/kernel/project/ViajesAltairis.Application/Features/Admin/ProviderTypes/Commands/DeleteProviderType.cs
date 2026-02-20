using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.ProviderTypes.Commands;

public record DeleteProviderTypeCommand(long Id) : IRequest;

public class DeleteProviderTypeHandler : IRequestHandler<DeleteProviderTypeCommand>
{
    private readonly IRepository<ProviderType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProviderTypeHandler(IRepository<ProviderType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteProviderTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"ProviderType {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
