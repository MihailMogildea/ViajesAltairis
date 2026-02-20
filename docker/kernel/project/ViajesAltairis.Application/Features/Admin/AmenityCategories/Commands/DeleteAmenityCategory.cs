using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AmenityCategories.Commands;

public record DeleteAmenityCategoryCommand(long Id) : IRequest;

public class DeleteAmenityCategoryHandler : IRequestHandler<DeleteAmenityCategoryCommand>
{
    private readonly IRepository<AmenityCategory> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAmenityCategoryHandler(IRepository<AmenityCategory> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteAmenityCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"AmenityCategory {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
