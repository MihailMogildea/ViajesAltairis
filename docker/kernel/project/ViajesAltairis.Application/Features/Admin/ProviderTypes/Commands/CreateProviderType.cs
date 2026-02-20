using MediatR;
using ViajesAltairis.Application.Features.Admin.ProviderTypes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.ProviderTypes.Commands;

public record CreateProviderTypeCommand(string Name) : IRequest<ProviderTypeDto>;

public class CreateProviderTypeHandler : IRequestHandler<CreateProviderTypeCommand, ProviderTypeDto>
{
    private readonly IRepository<ProviderType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProviderTypeHandler(IRepository<ProviderType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProviderTypeDto> Handle(CreateProviderTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = new ProviderType
        {
            Name = request.Name
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new ProviderTypeDto(entity.Id, entity.Name, entity.CreatedAt);
    }
}
