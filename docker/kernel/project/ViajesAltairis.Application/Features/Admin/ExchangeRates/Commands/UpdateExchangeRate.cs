using MediatR;
using ViajesAltairis.Application.Features.Admin.ExchangeRates.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.ExchangeRates.Commands;

public record UpdateExchangeRateCommand(long Id, long CurrencyId, decimal RateToEur, DateTime ValidFrom, DateTime ValidTo) : IRequest<ExchangeRateDto>;

public class UpdateExchangeRateHandler : IRequestHandler<UpdateExchangeRateCommand, ExchangeRateDto>
{
    private readonly IRepository<ExchangeRate> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateExchangeRateHandler(IRepository<ExchangeRate> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ExchangeRateDto> Handle(UpdateExchangeRateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"ExchangeRate {request.Id} not found.");
        entity.CurrencyId = request.CurrencyId;
        entity.RateToEur = request.RateToEur;
        entity.ValidFrom = request.ValidFrom;
        entity.ValidTo = request.ValidTo;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new ExchangeRateDto(entity.Id, entity.CurrencyId, entity.RateToEur, entity.ValidFrom, entity.ValidTo, entity.CreatedAt);
    }
}
