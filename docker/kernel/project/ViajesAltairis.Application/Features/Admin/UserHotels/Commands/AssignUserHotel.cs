using MediatR;
using ViajesAltairis.Application.Features.Admin.UserHotels.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.UserHotels.Commands;

public record AssignUserHotelCommand(long UserId, long HotelId) : IRequest<UserHotelDto>;

public class AssignUserHotelHandler : IRequestHandler<AssignUserHotelCommand, UserHotelDto>
{
    private readonly IRepository<UserHotel> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignUserHotelHandler(IRepository<UserHotel> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserHotelDto> Handle(AssignUserHotelCommand request, CancellationToken cancellationToken)
    {
        var entity = new UserHotel
        {
            UserId = request.UserId,
            HotelId = request.HotelId
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new UserHotelDto(entity.Id, entity.UserId, entity.HotelId, entity.CreatedAt);
    }
}
