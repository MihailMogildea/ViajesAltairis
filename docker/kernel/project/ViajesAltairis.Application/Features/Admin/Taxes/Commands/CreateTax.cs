using MediatR;
using ViajesAltairis.Application.Features.Admin.Taxes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Taxes.Commands;

public record CreateTaxCommand(long TaxTypeId, long? CountryId, long? AdministrativeDivisionId, long? CityId, decimal Rate, bool IsPercentage) : IRequest<TaxDto>;

public class CreateTaxHandler : IRequestHandler<CreateTaxCommand, TaxDto>
{
    private readonly IRepository<Tax> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaxHandler(IRepository<Tax> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TaxDto> Handle(CreateTaxCommand request, CancellationToken cancellationToken)
    {
        var entity = new Tax
        {
            TaxTypeId = request.TaxTypeId,
            CountryId = request.CountryId,
            AdministrativeDivisionId = request.AdministrativeDivisionId,
            CityId = request.CityId,
            Rate = request.Rate,
            IsPercentage = request.IsPercentage,
            Enabled = true
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new TaxDto { Id = entity.Id, TaxTypeId = entity.TaxTypeId, CountryId = entity.CountryId, AdministrativeDivisionId = entity.AdministrativeDivisionId, CityId = entity.CityId, Rate = entity.Rate, IsPercentage = entity.IsPercentage, Enabled = entity.Enabled, CreatedAt = entity.CreatedAt };
    }
}
