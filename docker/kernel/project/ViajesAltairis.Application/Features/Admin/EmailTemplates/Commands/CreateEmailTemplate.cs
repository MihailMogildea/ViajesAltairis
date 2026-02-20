using MediatR;
using ViajesAltairis.Application.Features.Admin.EmailTemplates.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.EmailTemplates.Commands;

public record CreateEmailTemplateCommand(string Name) : IRequest<EmailTemplateDto>;

public class CreateEmailTemplateHandler : IRequestHandler<CreateEmailTemplateCommand, EmailTemplateDto>
{
    private readonly ISimpleRepository<EmailTemplate> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEmailTemplateHandler(ISimpleRepository<EmailTemplate> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<EmailTemplateDto> Handle(CreateEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        var entity = new EmailTemplate
        {
            Name = request.Name
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new EmailTemplateDto(entity.Id, entity.Name);
    }
}
