using MediatR;
using ViajesAltairis.Application.Features.Admin.PromoCodes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.PromoCodes.Commands;

public record CreatePromoCodeCommand(string Code, decimal? DiscountPercentage, decimal? DiscountAmount, long? CurrencyId, DateOnly ValidFrom, DateOnly ValidTo, int? MaxUses) : IRequest<PromoCodeDto>;

public class CreatePromoCodeHandler : IRequestHandler<CreatePromoCodeCommand, PromoCodeDto>
{
    private readonly IRepository<PromoCode> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePromoCodeHandler(IRepository<PromoCode> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PromoCodeDto> Handle(CreatePromoCodeCommand request, CancellationToken cancellationToken)
    {
        var entity = new PromoCode
        {
            Code = request.Code,
            DiscountPercentage = request.DiscountPercentage,
            DiscountAmount = request.DiscountAmount,
            CurrencyId = request.CurrencyId,
            ValidFrom = request.ValidFrom,
            ValidTo = request.ValidTo,
            MaxUses = request.MaxUses,
            CurrentUses = 0,
            Enabled = true
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new PromoCodeDto { Id = entity.Id, Code = entity.Code, DiscountPercentage = entity.DiscountPercentage, DiscountAmount = entity.DiscountAmount, CurrencyId = entity.CurrencyId, ValidFrom = entity.ValidFrom, ValidTo = entity.ValidTo, MaxUses = entity.MaxUses, CurrentUses = entity.CurrentUses, Enabled = entity.Enabled, CreatedAt = entity.CreatedAt, UpdatedAt = entity.UpdatedAt };
    }
}
