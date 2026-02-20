using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Providers.Commands;

public record DeleteProviderCommand(long Id) : IRequest;

public class DeleteProviderHandler : IRequestHandler<DeleteProviderCommand>
{
    private readonly IRepository<Provider> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProviderHandler(IRepository<Provider> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteProviderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Provider {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
