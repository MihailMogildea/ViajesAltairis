using MediatR;
using ViajesAltairis.Application.Features.Admin.EmailTemplates.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.EmailTemplates.Commands;

public record UpdateEmailTemplateCommand(long Id, string Name) : IRequest<EmailTemplateDto>;

public class UpdateEmailTemplateHandler : IRequestHandler<UpdateEmailTemplateCommand, EmailTemplateDto>
{
    private readonly ISimpleRepository<EmailTemplate> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmailTemplateHandler(ISimpleRepository<EmailTemplate> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<EmailTemplateDto> Handle(UpdateEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"EmailTemplate {request.Id} not found.");
        entity.Name = request.Name;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new EmailTemplateDto(entity.Id, entity.Name);
    }
}
