using MediatR;
using ViajesAltairis.Application.Features.Admin.Amenities.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Amenities.Commands;

public record CreateAmenityCommand(long CategoryId, string Name) : IRequest<AmenityDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class CreateAmenityHandler : IRequestHandler<CreateAmenityCommand, AmenityDto>
{
    private readonly IRepository<Amenity> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAmenityHandler(IRepository<Amenity> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AmenityDto> Handle(CreateAmenityCommand request, CancellationToken cancellationToken)
    {
        var entity = new Amenity
        {
            CategoryId = request.CategoryId,
            Name = request.Name
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new AmenityDto(entity.Id, entity.CategoryId, entity.Name, entity.CreatedAt);
    }
}
