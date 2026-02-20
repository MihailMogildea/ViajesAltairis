using MediatR;
using ViajesAltairis.Application.Features.Admin.ExchangeRates.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.ExchangeRates.Commands;

public record CreateExchangeRateCommand(long CurrencyId, decimal RateToEur, DateTime ValidFrom, DateTime ValidTo) : IRequest<ExchangeRateDto>;

public class CreateExchangeRateHandler : IRequestHandler<CreateExchangeRateCommand, ExchangeRateDto>
{
    private readonly IRepository<ExchangeRate> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateExchangeRateHandler(IRepository<ExchangeRate> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ExchangeRateDto> Handle(CreateExchangeRateCommand request, CancellationToken cancellationToken)
    {
        var entity = new ExchangeRate
        {
            CurrencyId = request.CurrencyId,
            RateToEur = request.RateToEur,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new ExchangeRateDto(entity.Id, entity.CurrencyId, entity.RateToEur, entity.ValidFrom, entity.ValidTo, entity.CreatedAt);
    }
}
