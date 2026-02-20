using MediatR;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisions.Commands;

public record UpdateAdministrativeDivisionCommand(long Id, long CountryId, long? ParentId, string Name, long TypeId, byte Level) : IRequest<AdministrativeDivisionDto>;

public class UpdateAdministrativeDivisionHandler : IRequestHandler<UpdateAdministrativeDivisionCommand, AdministrativeDivisionDto>
{
    private readonly IRepository<AdministrativeDivision> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAdministrativeDivisionHandler(IRepository<AdministrativeDivision> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AdministrativeDivisionDto> Handle(UpdateAdministrativeDivisionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"AdministrativeDivision {request.Id} not found.");
        entity.CountryId = request.CountryId;
        entity.ParentId = request.ParentId;
        entity.Name = request.Name;
        entity.TypeId = request.TypeId;
        entity.Level = request.Level;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new AdministrativeDivisionDto(entity.Id, entity.CountryId, entity.ParentId, entity.Name, entity.TypeId, entity.Level, entity.Enabled, entity.CreatedAt);
    }
}
