using MediatR;
using ViajesAltairis.Application.Features.Admin.AdministrativeDivisionTypes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.AdministrativeDivisionTypes.Commands;

public record CreateAdministrativeDivisionTypeCommand(string Name) : IRequest<AdministrativeDivisionTypeDto>;

public class CreateAdministrativeDivisionTypeHandler : IRequestHandler<CreateAdministrativeDivisionTypeCommand, AdministrativeDivisionTypeDto>
{
    private readonly IRepository<AdministrativeDivisionType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAdministrativeDivisionTypeHandler(IRepository<AdministrativeDivisionType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AdministrativeDivisionTypeDto> Handle(CreateAdministrativeDivisionTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = new AdministrativeDivisionType
        {
            Name = request.Name
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new AdministrativeDivisionTypeDto(entity.Id, entity.Name, entity.CreatedAt);
    }
}
