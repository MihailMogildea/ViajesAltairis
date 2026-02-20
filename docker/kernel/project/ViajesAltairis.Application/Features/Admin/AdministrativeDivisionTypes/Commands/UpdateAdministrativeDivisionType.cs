using MediatR;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisionTypes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisionTypes.Commands;

public record UpdateAdministrativeDivisionTypeCommand(long Id, string Name) : IRequest<AdministrativeDivisionTypeDto>;

public class UpdateAdministrativeDivisionTypeHandler : IRequestHandler<UpdateAdministrativeDivisionTypeCommand, AdministrativeDivisionTypeDto>
{
    private readonly IRepository<AdministrativeDivisionType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAdministrativeDivisionTypeHandler(IRepository<AdministrativeDivisionType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AdministrativeDivisionTypeDto> Handle(UpdateAdministrativeDivisionTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"AdministrativeDivisionType {request.Id} not found.");
        entity.Name = request.Name;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new AdministrativeDivisionTypeDto(entity.Id, entity.Name, entity.CreatedAt);
    }
}
