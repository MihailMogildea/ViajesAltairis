using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.UserHotels.Commands;

public record RemoveUserHotelCommand(long Id) : IRequest;

public class RemoveUserHotelHandler : IRequestHandler<RemoveUserHotelCommand>
{
    private readonly IRepository<UserHotel> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveUserHotelHandler(IRepository<UserHotel> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RemoveUserHotelCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"UserHotel {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
