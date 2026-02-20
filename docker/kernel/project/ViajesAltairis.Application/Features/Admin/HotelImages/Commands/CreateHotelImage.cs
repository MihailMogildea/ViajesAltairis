using MediatR;
using ViajesAltairis.Application.Features.Admin.HotelImages.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.HotelImages.Commands;

public record CreateHotelImageCommand(long HotelId, string Url, string? AltText, int SortOrder) : IRequest<HotelImageDto>;

public class CreateHotelImageHandler : IRequestHandler<CreateHotelImageCommand, HotelImageDto>
{
    private readonly IRepository<HotelImage> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateHotelImageHandler(IRepository<HotelImage> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<HotelImageDto> Handle(CreateHotelImageCommand request, CancellationToken cancellationToken)
    {
        var entity = new HotelImage
        {
            HotelId = request.HotelId,
            Url = request.Url,
            AltText = request.AltText,
            SortOrder = request.SortOrder
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new HotelImageDto(entity.Id, entity.HotelId, entity.Url, entity.AltText, entity.SortOrder, entity.CreatedAt);
    }
}
