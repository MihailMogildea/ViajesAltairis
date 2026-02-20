using MediatR;
using ViajesAltairis.Application.Features.Admin.Translations.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Translations.Commands;

public record UpdateTranslationCommand(long Id, string EntityType, long EntityId, string Field, long LanguageId, string Value) : IRequest<TranslationDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["trans:", "hotel:", "ref:"];
}

public class UpdateTranslationHandler : IRequestHandler<UpdateTranslationCommand, TranslationDto>
{
    private readonly IRepository<Translation> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTranslationHandler(IRepository<Translation> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TranslationDto> Handle(UpdateTranslationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Translation {request.Id} not found.");
        entity.EntityType = request.EntityType;
        entity.EntityId = request.EntityId;
        entity.Field = request.Field;
        entity.LanguageId = request.LanguageId;
        entity.Value = request.Value;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new TranslationDto(entity.Id, entity.EntityType, entity.EntityId, entity.Field, entity.LanguageId, entity.Value, entity.CreatedAt);
    }
}
