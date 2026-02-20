using MediatR;
using ViajesAltairis.Application.Features.Admin.WebTranslations.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.WebTranslations.Commands;

public record UpdateWebTranslationCommand(long Id, string TranslationKey, long LanguageId, string Value) : IRequest<WebTranslationDto>;

public class UpdateWebTranslationHandler : IRequestHandler<UpdateWebTranslationCommand, WebTranslationDto>
{
    private readonly IRepository<WebTranslation> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWebTranslationHandler(IRepository<WebTranslation> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<WebTranslationDto> Handle(UpdateWebTranslationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"WebTranslation {request.Id} not found.");
        entity.TranslationKey = request.TranslationKey;
        entity.LanguageId = request.LanguageId;
        entity.Value = request.Value;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new WebTranslationDto(entity.Id, entity.TranslationKey, entity.LanguageId, entity.Value, entity.CreatedAt);
    }
}
