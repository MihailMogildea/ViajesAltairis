using MediatR;
using ViajesAltairis.Application.Features.Admin.AmenityCategories.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AmenityCategories.Commands;

public record UpdateAmenityCategoryCommand(long Id, string Name) : IRequest<AmenityCategoryDto>;

public class UpdateAmenityCategoryHandler : IRequestHandler<UpdateAmenityCategoryCommand, AmenityCategoryDto>
{
    private readonly IRepository<AmenityCategory> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAmenityCategoryHandler(IRepository<AmenityCategory> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AmenityCategoryDto> Handle(UpdateAmenityCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"AmenityCategory {request.Id} not found.");
        entity.Name = request.Name;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new AmenityCategoryDto(entity.Id, entity.Name, entity.CreatedAt);
    }
}
