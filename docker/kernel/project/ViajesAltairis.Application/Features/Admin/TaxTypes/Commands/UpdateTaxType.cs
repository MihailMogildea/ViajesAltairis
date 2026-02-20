using MediatR;
using ViajesAltairis.Application.Features.Admin.TaxTypes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.TaxTypes.Commands;

public record UpdateTaxTypeCommand(long Id, string Name) : IRequest<TaxTypeDto>;

public class UpdateTaxTypeHandler : IRequestHandler<UpdateTaxTypeCommand, TaxTypeDto>
{
    private readonly IRepository<TaxType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaxTypeHandler(IRepository<TaxType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TaxTypeDto> Handle(UpdateTaxTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"TaxType {request.Id} not found.");
        entity.Name = request.Name;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new TaxTypeDto(entity.Id, entity.Name, entity.CreatedAt);
    }
}
