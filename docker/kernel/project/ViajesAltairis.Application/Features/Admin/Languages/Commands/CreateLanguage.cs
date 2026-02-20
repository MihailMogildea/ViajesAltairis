using MediatR;
using ViajesAltairis.Application.Features.Admin.Languages.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Languages.Commands;

public record CreateLanguageCommand(string IsoCode, string Name) : IRequest<LanguageDto>;

public class CreateLanguageHandler : IRequestHandler<CreateLanguageCommand, LanguageDto>
{
    private readonly IRepository<Language> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLanguageHandler(IRepository<Language> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<LanguageDto> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
    {
        var entity = new Language
        {
            IsoCode = request.IsoCode,
            Name = request.Name
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new LanguageDto(entity.Id, entity.IsoCode, entity.Name, entity.CreatedAt);
    }
}
