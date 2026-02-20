using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Languages.Commands;

public record DeleteLanguageCommand(long Id) : IRequest;

public class DeleteLanguageHandler : IRequestHandler<DeleteLanguageCommand>
{
    private readonly IRepository<Language> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLanguageHandler(IRepository<Language> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteLanguageCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Language {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
