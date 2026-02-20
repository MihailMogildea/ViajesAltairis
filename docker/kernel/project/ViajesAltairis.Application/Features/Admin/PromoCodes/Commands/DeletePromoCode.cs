using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PromoCodes.Commands;

public record DeletePromoCodeCommand(long Id) : IRequest;

public class DeletePromoCodeHandler : IRequestHandler<DeletePromoCodeCommand>
{
    private readonly IRepository<PromoCode> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePromoCodeHandler(IRepository<PromoCode> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeletePromoCodeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"PromoCode {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
