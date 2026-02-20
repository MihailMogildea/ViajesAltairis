using MediatR;
using ViajesAltairis.Application.Features.Admin.Countries.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Countries.Commands;

public record CreateCountryCommand(string IsoCode, string Name, long CurrencyId) : IRequest<CountryDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:", "ref:countries"];
}

public class CreateCountryHandler : IRequestHandler<CreateCountryCommand, CountryDto>
{
    private readonly IRepository<Country> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCountryHandler(IRepository<Country> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CountryDto> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var entity = new Country
        {
            IsoCode = request.IsoCode,
            Name = request.Name,
            CurrencyId = request.CurrencyId,
            Enabled = true
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CountryDto(entity.Id, entity.IsoCode, entity.Name, entity.CurrencyId, entity.Enabled, entity.CreatedAt);
    }
}
