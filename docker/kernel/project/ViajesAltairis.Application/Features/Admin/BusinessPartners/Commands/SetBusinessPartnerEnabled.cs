using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.BusinessPartners.Commands;

public record SetBusinessPartnerEnabledCommand(long Id, bool Enabled) : IRequest;

public class SetBusinessPartnerEnabledHandler : IRequestHandler<SetBusinessPartnerEnabledCommand>
{
    private readonly IRepository<BusinessPartner> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SetBusinessPartnerEnabledHandler(IRepository<BusinessPartner> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SetBusinessPartnerEnabledCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"BusinessPartner {request.Id} not found.");
        entity.Enabled = request.Enabled;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
