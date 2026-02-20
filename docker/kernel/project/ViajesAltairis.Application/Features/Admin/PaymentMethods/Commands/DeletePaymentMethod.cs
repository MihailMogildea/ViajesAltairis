using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PaymentMethods.Commands;

public record DeletePaymentMethodCommand(long Id) : IRequest;

public class DeletePaymentMethodHandler : IRequestHandler<DeletePaymentMethodCommand>
{
    private readonly IRepository<PaymentMethod> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePaymentMethodHandler(IRepository<PaymentMethod> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeletePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"PaymentMethod {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
