using MediatR;
using ViajesAltairis.Application.Features.Admin.PaymentMethods.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PaymentMethods.Commands;

public record UpdatePaymentMethodCommand(long Id, string Name, int MinDaysBeforeCheckin) : IRequest<PaymentMethodDto>;

public class UpdatePaymentMethodHandler : IRequestHandler<UpdatePaymentMethodCommand, PaymentMethodDto>
{
    private readonly IRepository<PaymentMethod> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePaymentMethodHandler(IRepository<PaymentMethod> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentMethodDto> Handle(UpdatePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"PaymentMethod {request.Id} not found.");
        entity.Name = request.Name;
        entity.MinDaysBeforeCheckin = request.MinDaysBeforeCheckin;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new PaymentMethodDto(entity.Id, entity.Name, entity.MinDaysBeforeCheckin, entity.Enabled, entity.CreatedAt);
    }
}
