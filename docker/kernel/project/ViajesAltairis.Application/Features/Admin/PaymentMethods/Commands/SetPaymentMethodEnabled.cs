using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PaymentMethods.Commands;

public record SetPaymentMethodEnabledCommand(long Id, bool Enabled) : IRequest;

public class SetPaymentMethodEnabledHandler : IRequestHandler<SetPaymentMethodEnabledCommand>
{
    private readonly IRepository<PaymentMethod> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SetPaymentMethodEnabledHandler(IRepository<PaymentMethod> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SetPaymentMethodEnabledCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"PaymentMethod {request.Id} not found.");
        entity.Enabled = request.Enabled;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
