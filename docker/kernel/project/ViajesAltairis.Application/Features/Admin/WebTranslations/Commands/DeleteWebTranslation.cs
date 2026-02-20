using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.WebTranslations.Commands;

public record DeleteWebTranslationCommand(long Id) : IRequest;

public class DeleteWebTranslationHandler : IRequestHandler<DeleteWebTranslationCommand>
{
    private readonly IRepository<WebTranslation> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteWebTranslationHandler(IRepository<WebTranslation> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteWebTranslationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"WebTranslation {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
