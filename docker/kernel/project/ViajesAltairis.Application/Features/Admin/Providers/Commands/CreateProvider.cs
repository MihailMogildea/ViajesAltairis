using MediatR;
using ViajesAltairis.Application.Features.Admin.Providers.Dtos;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Providers.Commands;

public record CreateProviderCommand(long TypeId, long CurrencyId, string Name, string? ApiUrl, string? ApiUsername, string? ApiPassword, decimal Margin) : IRequest<ProviderDto>;

public class CreateProviderHandler : IRequestHandler<CreateProviderCommand, ProviderDto>
{
    private readonly IRepository<Provider> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEncryptionService _encryption;

    public CreateProviderHandler(IRepository<Provider> repository, IUnitOfWork unitOfWork, IEncryptionService encryption)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _encryption = encryption;
    }

    public async Task<ProviderDto> Handle(CreateProviderCommand request, CancellationToken cancellationToken)
    {
        var entity = new Provider
        {
            TypeId = request.TypeId,
            CurrencyId = request.CurrencyId,
            Name = request.Name,
            ApiUrl = request.ApiUrl,
            ApiUsername = request.ApiUsername,
            ApiPasswordEncrypted = request.ApiPassword != null ? _encryption.Encrypt(request.ApiPassword) : null,
            Margin = request.Margin,
            Enabled = true
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new ProviderDto(entity.Id, entity.TypeId, entity.CurrencyId, entity.Name, entity.ApiUrl, entity.ApiUsername, entity.Margin, entity.Enabled, entity.SyncStatus, entity.LastSyncedAt, entity.CreatedAt);
    }
}
