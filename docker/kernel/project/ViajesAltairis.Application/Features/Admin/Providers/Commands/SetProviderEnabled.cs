using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Providers.Commands;

public record SetProviderEnabledCommand(long Id, bool Enabled) : IRequest;

public class SetProviderEnabledHandler : IRequestHandler<SetProviderEnabledCommand>
{
    private readonly IRepository<Provider> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SetProviderEnabledHandler(IRepository<Provider> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SetProviderEnabledCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Provider {request.Id} not found.");
        entity.Enabled = request.Enabled;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
