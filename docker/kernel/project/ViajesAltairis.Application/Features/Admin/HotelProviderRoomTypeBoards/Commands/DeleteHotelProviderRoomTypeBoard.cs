using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypeBoards.Commands;

public record DeleteHotelProviderRoomTypeBoardCommand(long Id) : IRequest, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class DeleteHotelProviderRoomTypeBoardHandler : IRequestHandler<DeleteHotelProviderRoomTypeBoardCommand>
{
    private readonly ISimpleRepository<HotelProviderRoomTypeBoard> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHotelProviderRoomTypeBoardHandler(ISimpleRepository<HotelProviderRoomTypeBoard> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteHotelProviderRoomTypeBoardCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"HotelProviderRoomTypeBoard {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
