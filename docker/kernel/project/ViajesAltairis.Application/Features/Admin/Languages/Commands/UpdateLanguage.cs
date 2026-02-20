using MediatR;
using ViajesAltairis.Application.Features.Admin.Languages.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Languages.Commands;

public record UpdateLanguageCommand(long Id, string IsoCode, string Name) : IRequest<LanguageDto>;

public class UpdateLanguageHandler : IRequestHandler<UpdateLanguageCommand, LanguageDto>
{
    private readonly IRepository<Language> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLanguageHandler(IRepository<Language> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<LanguageDto> Handle(UpdateLanguageCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Language {request.Id} not found.");
        entity.IsoCode = request.IsoCode;
        entity.Name = request.Name;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new LanguageDto(entity.Id, entity.IsoCode, entity.Name, entity.CreatedAt);
    }
}
