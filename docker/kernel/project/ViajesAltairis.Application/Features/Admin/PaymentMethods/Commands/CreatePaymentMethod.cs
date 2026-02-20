using MediatR;
using ViajesAltairis.Application.Features.Admin.PaymentMethods.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PaymentMethods.Commands;

public record CreatePaymentMethodCommand(string Name, int MinDaysBeforeCheckin) : IRequest<PaymentMethodDto>;

public class CreatePaymentMethodHandler : IRequestHandler<CreatePaymentMethodCommand, PaymentMethodDto>
{
    private readonly IRepository<PaymentMethod> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentMethodHandler(IRepository<PaymentMethod> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentMethodDto> Handle(CreatePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var entity = new PaymentMethod
        {
            Name = request.Name,
            MinDaysBeforeCheckin = request.MinDaysBeforeCheckin
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new PaymentMethodDto(entity.Id, entity.Name, entity.MinDaysBeforeCheckin, entity.Enabled, entity.CreatedAt);
    }
}
