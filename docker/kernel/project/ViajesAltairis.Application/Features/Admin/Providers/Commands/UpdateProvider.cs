using MediatR;
using ViajesAltairis.Application.Features.Admin.Providers.Dtos;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Providers.Commands;

public record UpdateProviderCommand(long Id, long TypeId, long CurrencyId, string Name, string? ApiUrl, string? ApiUsername, string? ApiPassword, decimal Margin) : IRequest<ProviderDto>;

public class UpdateProviderHandler : IRequestHandler<UpdateProviderCommand, ProviderDto>
{
    private readonly IRepository<Provider> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEncryptionService _encryption;

    public UpdateProviderHandler(IRepository<Provider> repository, IUnitOfWork unitOfWork, IEncryptionService encryption)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _encryption = encryption;
    }

    public async Task<ProviderDto> Handle(UpdateProviderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Provider {request.Id} not found.");
        entity.TypeId = request.TypeId;
        entity.CurrencyId = request.CurrencyId;
        entity.Name = request.Name;
        entity.ApiUrl = request.ApiUrl;
        entity.ApiUsername = request.ApiUsername;
        entity.ApiPasswordEncrypted = request.ApiPassword != null ? _encryption.Encrypt(request.ApiPassword) : null;
        entity.Margin = request.Margin;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new ProviderDto(entity.Id, entity.TypeId, entity.CurrencyId, entity.Name, entity.ApiUrl, entity.ApiUsername, entity.Margin, entity.Enabled, entity.SyncStatus, entity.LastSyncedAt, entity.CreatedAt);
    }
}
