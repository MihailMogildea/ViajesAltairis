using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.EmailTemplates.Commands;

public record DeleteEmailTemplateCommand(long Id) : IRequest;

public class DeleteEmailTemplateHandler : IRequestHandler<DeleteEmailTemplateCommand>
{
    private readonly ISimpleRepository<EmailTemplate> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmailTemplateHandler(ISimpleRepository<EmailTemplate> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"EmailTemplate {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
