using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Commands;

public record DeleteAdministrativeDivisionCommand(long Id) : IRequest;

public class DeleteAdministrativeDivisionHandler : IRequestHandler<DeleteAdministrativeDivisionCommand>
{
    private readonly IRepository<AdministrativeDivision> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAdministrativeDivisionHandler(IRepository<AdministrativeDivision> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteAdministrativeDivisionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"AdministrativeDivision {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
