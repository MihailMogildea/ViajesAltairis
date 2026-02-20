using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Taxes.Commands;

public record DeleteTaxCommand(long Id) : IRequest;

public class DeleteTaxHandler : IRequestHandler<DeleteTaxCommand>
{
    private readonly IRepository<Tax> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaxHandler(IRepository<Tax> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTaxCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Tax {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
