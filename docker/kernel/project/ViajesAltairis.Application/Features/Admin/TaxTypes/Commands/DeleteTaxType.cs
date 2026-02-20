using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.TaxTypes.Commands;

public record DeleteTaxTypeCommand(long Id) : IRequest;

public class DeleteTaxTypeHandler : IRequestHandler<DeleteTaxTypeCommand>
{
    private readonly IRepository<TaxType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaxTypeHandler(IRepository<TaxType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTaxTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"TaxType {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
