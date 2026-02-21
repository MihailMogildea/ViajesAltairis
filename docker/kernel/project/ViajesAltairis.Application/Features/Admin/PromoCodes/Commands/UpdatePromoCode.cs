using MediatR;
using ViajesAltairis.Application.Features.Admin.PromoCodes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PromoCodes.Commands;

public record UpdatePromoCodeCommand(long Id, string Code, decimal? DiscountPercentage, decimal? DiscountAmount, long? CurrencyId, DateOnly ValidFrom, DateOnly ValidTo, int? MaxUses) : IRequest<PromoCodeDto>;

public class UpdatePromoCodeHandler : IRequestHandler<UpdatePromoCodeCommand, PromoCodeDto>
{
    private readonly IRepository<PromoCode> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePromoCodeHandler(IRepository<PromoCode> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PromoCodeDto> Handle(UpdatePromoCodeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"PromoCode {request.Id} not found.");
        entity.Code = request.Code;
        entity.DiscountPercentage = request.DiscountPercentage;
        entity.DiscountAmount = request.DiscountAmount;
        entity.CurrencyId = request.CurrencyId;
        entity.ValidFrom = request.ValidFrom;
        entity.ValidTo = request.ValidTo;
        entity.MaxUses = request.MaxUses;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new PromoCodeDto { Id = entity.Id, Code = entity.Code, DiscountPercentage = entity.DiscountPercentage, DiscountAmount = entity.DiscountAmount, CurrencyId = entity.CurrencyId, ValidFrom = entity.ValidFrom, ValidTo = entity.ValidTo, MaxUses = entity.MaxUses, CurrentUses = entity.CurrentUses, Enabled = entity.Enabled, CreatedAt = entity.CreatedAt, UpdatedAt = entity.UpdatedAt };
    }
}
