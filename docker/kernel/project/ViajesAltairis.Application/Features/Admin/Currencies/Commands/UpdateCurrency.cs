using MediatR;
using ViajesAltairis.Application.Features.Admin.Currencies.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Currencies.Commands;

public record UpdateCurrencyCommand(long Id, string IsoCode, string Name, string Symbol) : IRequest<CurrencyDto>;

public class UpdateCurrencyHandler : IRequestHandler<UpdateCurrencyCommand, CurrencyDto>
{
    private readonly IRepository<Currency> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCurrencyHandler(IRepository<Currency> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CurrencyDto> Handle(UpdateCurrencyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Currency {request.Id} not found.");
        entity.IsoCode = request.IsoCode;
        entity.Name = request.Name;
        entity.Symbol = request.Symbol;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CurrencyDto(entity.Id, entity.IsoCode, entity.Name, entity.Symbol, entity.CreatedAt);
    }
}
