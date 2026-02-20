using MediatR;
using ViajesAltairis.Application.Features.Admin.SeasonalMargins.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.SeasonalMargins.Commands;

public record CreateSeasonalMarginCommand(long AdministrativeDivisionId, string StartMonthDay, string EndMonthDay, decimal Margin) : IRequest<SeasonalMarginDto>;

public class CreateSeasonalMarginHandler : IRequestHandler<CreateSeasonalMarginCommand, SeasonalMarginDto>
{
    private readonly IRepository<SeasonalMargin> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSeasonalMarginHandler(IRepository<SeasonalMargin> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SeasonalMarginDto> Handle(CreateSeasonalMarginCommand request, CancellationToken cancellationToken)
    {
        var entity = new SeasonalMargin
        {
            AdministrativeDivisionId = request.AdministrativeDivisionId,
            StartMonthDay = request.StartMonthDay,
            EndMonthDay = request.EndMonthDay,
            Margin = request.Margin
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new SeasonalMarginDto(entity.Id, entity.AdministrativeDivisionId, entity.StartMonthDay, entity.EndMonthDay, entity.Margin, entity.CreatedAt, entity.UpdatedAt);
    }
}
