using MediatR;
using ViajesAltairis.Application.Features.Admin.SeasonalMargins.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.SeasonalMargins.Commands;

public record UpdateSeasonalMarginCommand(long Id, long AdministrativeDivisionId, string StartMonthDay, string EndMonthDay, decimal Margin) : IRequest<SeasonalMarginDto>;

public class UpdateSeasonalMarginHandler : IRequestHandler<UpdateSeasonalMarginCommand, SeasonalMarginDto>
{
    private readonly IRepository<SeasonalMargin> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSeasonalMarginHandler(IRepository<SeasonalMargin> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SeasonalMarginDto> Handle(UpdateSeasonalMarginCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"SeasonalMargin {request.Id} not found.");
        entity.AdministrativeDivisionId = request.AdministrativeDivisionId;
        entity.StartMonthDay = request.StartMonthDay;
        entity.EndMonthDay = request.EndMonthDay;
        entity.Margin = request.Margin;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new SeasonalMarginDto(entity.Id, entity.AdministrativeDivisionId, entity.StartMonthDay, entity.EndMonthDay, entity.Margin, entity.CreatedAt, entity.UpdatedAt);
    }
}
