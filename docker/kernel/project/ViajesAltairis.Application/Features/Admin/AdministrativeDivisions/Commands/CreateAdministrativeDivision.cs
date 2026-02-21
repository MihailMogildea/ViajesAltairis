using MediatR;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Commands;

public record CreateAdministrativeDivisionCommand(long CountryId, long? ParentId, string Name, long TypeId, int Level) : IRequest<AdministrativeDivisionDto>;

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
            Level = (byte)request.Level,
            Enabled = true
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new AdministrativeDivisionDto { Id = entity.Id, CountryId = entity.CountryId, ParentId = entity.ParentId, Name = entity.Name, TypeId = entity.TypeId, Level = entity.Level, Enabled = entity.Enabled, CreatedAt = entity.CreatedAt };
    }
}
