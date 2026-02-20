using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelBlackouts.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelBlackouts.Commands;

public record UpdateHotelBlackoutCommand(long Id, long HotelId, DateOnly StartDate, DateOnly EndDate, string? Reason) : IRequest<HotelBlackoutDto>;

public class UpdateHotelBlackoutHandler : IRequestHandler<UpdateHotelBlackoutCommand, HotelBlackoutDto>
{
    private readonly IRepository<HotelBlackout> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateHotelBlackoutHandler(IRepository<HotelBlackout> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<HotelBlackoutDto> Handle(UpdateHotelBlackoutCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"HotelBlackout {request.Id} not found.");
        entity.HotelId = request.HotelId;
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.Reason = request.Reason;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new HotelBlackoutDto(entity.Id, entity.HotelId, entity.StartDate, entity.EndDate, entity.Reason, entity.CreatedAt);
    }
}
