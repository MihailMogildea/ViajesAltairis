using MediatR;
using ViajesAltairis.Application.Features.Admin.Currencies.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Currencies.Commands;

public record CreateCurrencyCommand(string IsoCode, string Name, string Symbol) : IRequest<CurrencyDto>;

public class CreateCurrencyHandler : IRequestHandler<CreateCurrencyCommand, CurrencyDto>
{
    private readonly IRepository<Currency> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCurrencyHandler(IRepository<Currency> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CurrencyDto> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var entity = new Currency
        {
            IsoCode = request.IsoCode,
            Name = request.Name,
            Symbol = request.Symbol
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CurrencyDto(entity.Id, entity.IsoCode, entity.Name, entity.Symbol, entity.CreatedAt);
    }
}
