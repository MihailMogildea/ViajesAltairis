using MediatR;
using ViajesAltairis.Application.Features.Admin.WebTranslations.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.WebTranslations.Commands;

public record CreateWebTranslationCommand(string TranslationKey, long LanguageId, string Value) : IRequest<WebTranslationDto>;

public class CreateWebTranslationHandler : IRequestHandler<CreateWebTranslationCommand, WebTranslationDto>
{
    private readonly IRepository<WebTranslation> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWebTranslationHandler(IRepository<WebTranslation> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<WebTranslationDto> Handle(CreateWebTranslationCommand request, CancellationToken cancellationToken)
    {
        var entity = new WebTranslation
        {
            TranslationKey = request.TranslationKey,
            LanguageId = request.LanguageId,
            Value = request.Value
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new WebTranslationDto(entity.Id, entity.TranslationKey, entity.LanguageId, entity.Value, entity.CreatedAt);
    }
}
