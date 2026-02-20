using MediatR;
using ViajesAltairis.Application.Features.Admin.Cities.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Cities.Commands;

public record UpdateCityCommand(long Id, long AdministrativeDivisionId, string Name) : IRequest<CityDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:", "ref:countries"];
}

public class UpdateCityHandler : IRequestHandler<UpdateCityCommand, CityDto>
{
    private readonly IRepository<City> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCityHandler(IRepository<City> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CityDto> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"City {request.Id} not found.");
        entity.AdministrativeDivisionId = request.AdministrativeDivisionId;
        entity.Name = request.Name;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CityDto(entity.Id, entity.AdministrativeDivisionId, entity.Name, entity.Enabled, entity.CreatedAt);
    }
}
