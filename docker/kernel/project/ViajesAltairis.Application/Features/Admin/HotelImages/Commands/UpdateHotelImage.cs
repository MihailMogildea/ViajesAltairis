using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelImages.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelImages.Commands;

public record UpdateHotelImageCommand(long Id, long HotelId, string Url, string? AltText, int SortOrder) : IRequest<HotelImageDto>;

public class UpdateHotelImageHandler : IRequestHandler<UpdateHotelImageCommand, HotelImageDto>
{
    private readonly IRepository<HotelImage> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateHotelImageHandler(IRepository<HotelImage> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<HotelImageDto> Handle(UpdateHotelImageCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"HotelImage {request.Id} not found.");
        entity.HotelId = request.HotelId;
        entity.Url = request.Url;
        entity.AltText = request.AltText;
        entity.SortOrder = request.SortOrder;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new HotelImageDto(entity.Id, entity.HotelId, entity.Url, entity.AltText, entity.SortOrder, entity.CreatedAt);
    }
}
