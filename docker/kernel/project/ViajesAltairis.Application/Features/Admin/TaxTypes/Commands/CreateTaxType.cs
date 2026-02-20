using MediatR;
using ViajesAltairis.Application.Features.Admin.TaxTypes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.TaxTypes.Commands;

public record CreateTaxTypeCommand(string Name) : IRequest<TaxTypeDto>;

public class CreateTaxTypeHandler : IRequestHandler<CreateTaxTypeCommand, TaxTypeDto>
{
    private readonly IRepository<TaxType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaxTypeHandler(IRepository<TaxType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TaxTypeDto> Handle(CreateTaxTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = new TaxType
        {
            Name = request.Name
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new TaxTypeDto(entity.Id, entity.Name, entity.CreatedAt);
    }
}
