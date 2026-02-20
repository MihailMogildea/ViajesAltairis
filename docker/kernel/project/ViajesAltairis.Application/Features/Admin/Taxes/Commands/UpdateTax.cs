using MediatR;
using ViajesAltairis.Application.Features.Admin.Taxes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Taxes.Commands;

public record UpdateTaxCommand(long Id, long TaxTypeId, long? CountryId, long? AdministrativeDivisionId, long? CityId, decimal Rate, bool IsPercentage) : IRequest<TaxDto>;

public class UpdateTaxHandler : IRequestHandler<UpdateTaxCommand, TaxDto>
{
    private readonly IRepository<Tax> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaxHandler(IRepository<Tax> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TaxDto> Handle(UpdateTaxCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Tax {request.Id} not found.");
        entity.TaxTypeId = request.TaxTypeId;
        entity.CountryId = request.CountryId;
        entity.AdministrativeDivisionId = request.AdministrativeDivisionId;
        entity.CityId = request.CityId;
        entity.Rate = request.Rate;
        entity.IsPercentage = request.IsPercentage;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new TaxDto(entity.Id, entity.TaxTypeId, entity.CountryId, entity.AdministrativeDivisionId, entity.CityId, entity.Rate, entity.IsPercentage, entity.Enabled, entity.CreatedAt);
    }
}
