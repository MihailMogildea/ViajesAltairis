using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisionTypes.Commands;

public record DeleteAdministrativeDivisionTypeCommand(long Id) : IRequest;

public class DeleteAdministrativeDivisionTypeHandler : IRequestHandler<DeleteAdministrativeDivisionTypeCommand>
{
    private readonly IRepository<AdministrativeDivisionType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAdministrativeDivisionTypeHandler(IRepository<AdministrativeDivisionType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteAdministrativeDivisionTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"AdministrativeDivisionType {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
