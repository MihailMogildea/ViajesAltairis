using MediatR;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Commands;

public record CreateAdministrativeDivisionCommand(long CountryId, long? ParentId, string Name, long TypeId, byte Level) : IRequest<AdministrativeDivisionDto>;

public class CreateAdministrativeDivisionHandler : IRequestHandler<CreateAdministrativeDivisionCommand, AdministrativeDivisionDto>
{
    private readonly IRepository<AdministrativeDivision> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAdministrativeDivisionHandler(IRepository<AdministrativeDivision> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AdministrativeDivisionDto> Handle(CreateAdministrativeDivisionCommand request, CancellationToken cancellationToken)
    {
        var entity = new AdministrativeDivision
        {
            CountryId = request.CountryId,
            ParentId = request.ParentId,
            Name = request.Name,
            TypeId = request.TypeId,
            Level = request.Level,
            Enabled = true
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new AdministrativeDivisionDto(entity.Id, entity.CountryId, entity.ParentId, entity.Name, entity.TypeId, entity.Level, entity.Enabled, entity.CreatedAt);
    }
}
