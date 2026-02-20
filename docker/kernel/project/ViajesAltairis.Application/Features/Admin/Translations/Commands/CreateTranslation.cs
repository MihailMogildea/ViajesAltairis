using MediatR;
using ViajesAltairis.Application.Features.Admin.Translations.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Translations.Commands;

public record CreateTranslationCommand(string EntityType, long EntityId, string Field, long LanguageId, string Value) : IRequest<TranslationDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["trans:", "hotel:", "ref:"];
}

public class CreateTranslationHandler : IRequestHandler<CreateTranslationCommand, TranslationDto>
{
    private readonly IRepository<Translation> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTranslationHandler(IRepository<Translation> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TranslationDto> Handle(CreateTranslationCommand request, CancellationToken cancellationToken)
    {
        var entity = new Translation
        {
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            Field = request.Field,
            LanguageId = request.LanguageId,
            Value = request.Value
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new TranslationDto(entity.Id, entity.EntityType, entity.EntityId, entity.Field, entity.LanguageId, entity.Value, entity.CreatedAt);
    }
}
