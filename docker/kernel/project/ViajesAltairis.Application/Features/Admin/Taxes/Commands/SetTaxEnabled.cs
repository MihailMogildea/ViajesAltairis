using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Taxes.Commands;

public record SetTaxEnabledCommand(long Id, bool Enabled) : IRequest;

public class SetTaxEnabledHandler : IRequestHandler<SetTaxEnabledCommand>
{
    private readonly IRepository<Tax> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SetTaxEnabledHandler(IRepository<Tax> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SetTaxEnabledCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Tax {request.Id} not found.");
        entity.Enabled = request.Enabled;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
