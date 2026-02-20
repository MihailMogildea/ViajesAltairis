using MediatR;
using ViajesAltairis.Application.Features.Admin.ProviderTypes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.ProviderTypes.Commands;

public record UpdateProviderTypeCommand(long Id, string Name) : IRequest<ProviderTypeDto>;

public class UpdateProviderTypeHandler : IRequestHandler<UpdateProviderTypeCommand, ProviderTypeDto>
{
    private readonly IRepository<ProviderType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProviderTypeHandler(IRepository<ProviderType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProviderTypeDto> Handle(UpdateProviderTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"ProviderType {request.Id} not found.");
        entity.Name = request.Name;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new ProviderTypeDto(entity.Id, entity.Name, entity.CreatedAt);
    }
}
