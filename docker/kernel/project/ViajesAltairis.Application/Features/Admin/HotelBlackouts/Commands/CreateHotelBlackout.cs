using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelBlackouts.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelBlackouts.Commands;

public record CreateHotelBlackoutCommand(long HotelId, DateOnly StartDate, DateOnly EndDate, string? Reason) : IRequest<HotelBlackoutDto>;

public class CreateHotelBlackoutHandler : IRequestHandler<CreateHotelBlackoutCommand, HotelBlackoutDto>
{
    private readonly IRepository<HotelBlackout> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateHotelBlackoutHandler(IRepository<HotelBlackout> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<HotelBlackoutDto> Handle(CreateHotelBlackoutCommand request, CancellationToken cancellationToken)
    {
        var entity = new HotelBlackout
        {
            HotelId = request.HotelId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Reason = request.Reason
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new HotelBlackoutDto(entity.Id, entity.HotelId, entity.StartDate, entity.EndDate, entity.Reason, entity.CreatedAt);
    }
}
