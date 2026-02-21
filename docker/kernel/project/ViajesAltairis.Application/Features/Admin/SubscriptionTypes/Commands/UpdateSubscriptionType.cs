using MediatR;
using ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Commands;

public record UpdateSubscriptionTypeCommand(long Id, string Name, decimal PricePerMonth, decimal Discount, long CurrencyId) : IRequest<SubscriptionTypeDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["ref:subscriptions"];
}

public class UpdateSubscriptionTypeHandler : IRequestHandler<UpdateSubscriptionTypeCommand, SubscriptionTypeDto>
{
    private readonly IRepository<SubscriptionType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSubscriptionTypeHandler(IRepository<SubscriptionType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SubscriptionTypeDto> Handle(UpdateSubscriptionTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"SubscriptionType {request.Id} not found.");
        entity.Name = request.Name;
        entity.PricePerMonth = request.PricePerMonth;
        entity.Discount = request.Discount;
        entity.CurrencyId = request.CurrencyId;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new SubscriptionTypeDto
        {
            Id = entity.Id, Name = entity.Name, PricePerMonth = entity.PricePerMonth,
            Discount = entity.Discount, CurrencyId = entity.CurrencyId, Enabled = entity.Enabled,
            CreatedAt = entity.CreatedAt, UpdatedAt = entity.UpdatedAt
        };
    }
}
