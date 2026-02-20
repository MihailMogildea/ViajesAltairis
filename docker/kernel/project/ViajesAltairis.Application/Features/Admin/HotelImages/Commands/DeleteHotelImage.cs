using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelImages.Commands;

public record DeleteHotelImageCommand(long Id) : IRequest;

public class DeleteHotelImageHandler : IRequestHandler<DeleteHotelImageCommand>
{
    private readonly IRepository<HotelImage> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHotelImageHandler(IRepository<HotelImage> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteHotelImageCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"HotelImage {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
