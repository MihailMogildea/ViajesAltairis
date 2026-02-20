using MediatR;
using ViajesAltairis.Application.Features.Admin.Countries.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Countries.Commands;

public record UpdateCountryCommand(long Id, string IsoCode, string Name, long CurrencyId) : IRequest<CountryDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:", "ref:countries"];
}

public class UpdateCountryHandler : IRequestHandler<UpdateCountryCommand, CountryDto>
{
    private readonly IRepository<Country> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCountryHandler(IRepository<Country> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CountryDto> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Country {request.Id} not found.");
        entity.IsoCode = request.IsoCode;
        entity.Name = request.Name;
        entity.CurrencyId = request.CurrencyId;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CountryDto(entity.Id, entity.IsoCode, entity.Name, entity.CurrencyId, entity.Enabled, entity.CreatedAt);
    }
}
