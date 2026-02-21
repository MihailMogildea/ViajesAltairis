using MediatR;
using ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.SubscriptionTypes.Commands;

public record CreateSubscriptionTypeCommand(string Name, decimal PricePerMonth, decimal Discount, long CurrencyId) : IRequest<SubscriptionTypeDto>, IInvalidatesCache
{
    public static IReadOnlyList<string> CachePrefixes => ["ref:subscriptions"];
}

public class CreateSubscriptionTypeHandler : IRequestHandler<CreateSubscriptionTypeCommand, SubscriptionTypeDto>
{
    private readonly IRepository<SubscriptionType> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSubscriptionTypeHandler(IRepository<SubscriptionType> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SubscriptionTypeDto> Handle(CreateSubscriptionTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = new SubscriptionType
        {
            Name = request.Name,
            PricePerMonth = request.PricePerMonth,
            Discount = request.Discount,
            CurrencyId = request.CurrencyId
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new SubscriptionTypeDto
        {
            Id = entity.Id, Name = entity.Name, PricePerMonth = entity.PricePerMonth,
            Discount = entity.Discount, CurrencyId = entity.CurrencyId, Enabled = entity.Enabled,
            CreatedAt = entity.CreatedAt, UpdatedAt = entity.UpdatedAt
        };
    }
}
