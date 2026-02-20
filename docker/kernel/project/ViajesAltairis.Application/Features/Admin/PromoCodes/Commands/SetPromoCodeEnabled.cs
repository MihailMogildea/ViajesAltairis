using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PromoCodes.Commands;

public record SetPromoCodeEnabledCommand(long Id, bool Enabled) : IRequest;

public class SetPromoCodeEnabledHandler : IRequestHandler<SetPromoCodeEnabledCommand>
{
    private readonly IRepository<PromoCode> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SetPromoCodeEnabledHandler(IRepository<PromoCode> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SetPromoCodeEnabledCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"PromoCode {request.Id} not found.");
        entity.Enabled = request.Enabled;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
