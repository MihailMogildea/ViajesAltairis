using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.ExchangeRates.Commands;

public record DeleteExchangeRateCommand(long Id) : IRequest;

public class DeleteExchangeRateHandler : IRequestHandler<DeleteExchangeRateCommand>
{
    private readonly IRepository<ExchangeRate> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExchangeRateHandler(IRepository<ExchangeRate> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteExchangeRateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"ExchangeRate {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
