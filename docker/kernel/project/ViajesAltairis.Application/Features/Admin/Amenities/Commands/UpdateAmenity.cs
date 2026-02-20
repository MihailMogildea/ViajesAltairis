using MediatR;
using ViajesAltairis.Application.Features.Admin.Amenities.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Amenities.Commands;

public record UpdateAmenityCommand(long Id, long CategoryId, string Name) : IRequest<AmenityDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class UpdateAmenityHandler : IRequestHandler<UpdateAmenityCommand, AmenityDto>
{
    private readonly IRepository<Amenity> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAmenityHandler(IRepository<Amenity> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AmenityDto> Handle(UpdateAmenityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Amenity {request.Id} not found.");
        entity.CategoryId = request.CategoryId;
        entity.Name = request.Name;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new AmenityDto(entity.Id, entity.CategoryId, entity.Name, entity.CreatedAt);
    }
}
