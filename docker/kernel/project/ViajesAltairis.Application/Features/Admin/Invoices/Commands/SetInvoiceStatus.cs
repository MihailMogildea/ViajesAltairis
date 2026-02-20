using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Invoices.Commands;

public record SetInvoiceStatusCommand(long Id, long StatusId) : IRequest;

public class SetInvoiceStatusHandler : IRequestHandler<SetInvoiceStatusCommand>
{
    private readonly IRepository<Invoice> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SetInvoiceStatusHandler(IRepository<Invoice> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SetInvoiceStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Invoice {request.Id} not found.");
        entity.StatusId = request.StatusId;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
