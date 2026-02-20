using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Commands;

public record SetAdministrativeDivisionEnabledCommand(long Id, bool Enabled) : IRequest;

public class SetAdministrativeDivisionEnabledHandler : IRequestHandler<SetAdministrativeDivisionEnabledCommand>
{
    private readonly IRepository<AdministrativeDivision> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SetAdministrativeDivisionEnabledHandler(IRepository<AdministrativeDivision> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SetAdministrativeDivisionEnabledCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"AdministrativeDivision {request.Id} not found.");
        entity.Enabled = request.Enabled;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
