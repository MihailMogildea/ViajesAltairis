using MediatR;
using ViajesAltairis.Application.Features.Admin.AmenityCategories.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AmenityCategories.Commands;

public record CreateAmenityCategoryCommand(string Name) : IRequest<AmenityCategoryDto>;

public class CreateAmenityCategoryHandler : IRequestHandler<CreateAmenityCategoryCommand, AmenityCategoryDto>
{
    private readonly IRepository<AmenityCategory> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAmenityCategoryHandler(IRepository<AmenityCategory> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AmenityCategoryDto> Handle(CreateAmenityCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = new AmenityCategory { Name = request.Name };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new AmenityCategoryDto(entity.Id, entity.Name, entity.CreatedAt);
    }
}
