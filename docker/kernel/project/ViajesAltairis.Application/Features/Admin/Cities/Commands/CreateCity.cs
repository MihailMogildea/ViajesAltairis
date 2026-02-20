using MediatR;
using ViajesAltairis.Application.Features.Admin.Cities.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Cities.Commands;

public record CreateCityCommand(long AdministrativeDivisionId, string Name) : IRequest<CityDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:", "ref:countries"];
}

public class CreateCityHandler : IRequestHandler<CreateCityCommand, CityDto>
{
    private readonly IRepository<City> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCityHandler(IRepository<City> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CityDto> Handle(CreateCityCommand request, CancellationToken cancellationToken)
    {
        var entity = new City
        {
            AdministrativeDivisionId = request.AdministrativeDivisionId,
            Name = request.Name,
            Enabled = true
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CityDto(entity.Id, entity.AdministrativeDivisionId, entity.Name, entity.Enabled, entity.CreatedAt);
    }
}
