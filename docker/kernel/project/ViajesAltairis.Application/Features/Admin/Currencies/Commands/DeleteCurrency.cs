using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Currencies.Commands;

public record DeleteCurrencyCommand(long Id) : IRequest;

public class DeleteCurrencyHandler : IRequestHandler<DeleteCurrencyCommand>
{
    private readonly IRepository<Currency> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCurrencyHandler(IRepository<Currency> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Currency {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
