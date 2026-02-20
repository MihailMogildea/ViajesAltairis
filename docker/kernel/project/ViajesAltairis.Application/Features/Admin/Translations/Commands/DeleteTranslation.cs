using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Translations.Commands;

public record DeleteTranslationCommand(long Id) : IRequest, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["trans:", "hotel:", "ref:"];
}

public class DeleteTranslationHandler : IRequestHandler<DeleteTranslationCommand>
{
    private readonly IRepository<Translation> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTranslationHandler(IRepository<Translation> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTranslationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Translation {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
