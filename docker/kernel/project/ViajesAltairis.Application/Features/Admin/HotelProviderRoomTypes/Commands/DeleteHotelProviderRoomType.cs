using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelProviderRoomTypes.Commands;

public record DeleteHotelProviderRoomTypeCommand(long Id) : IRequest, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["hotel:"];
}

public class DeleteHotelProviderRoomTypeHandler : IRequestHandler<DeleteHotelProviderRoomTypeCommand>
{
    private readonly IRepository<HotelProviderRoomType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHotelProviderRoomTypeHandler(IRepository<HotelProviderRoomType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteHotelProviderRoomTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"HotelProviderRoomType {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
