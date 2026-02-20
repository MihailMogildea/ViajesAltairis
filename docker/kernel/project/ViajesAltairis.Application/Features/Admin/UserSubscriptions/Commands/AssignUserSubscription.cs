using MediatR;
using ViajesAltairis.Application.Features.Admin.UserSubscriptions.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.UserSubscriptions.Commands;

public record AssignUserSubscriptionCommand(long UserId, long SubscriptionTypeId, DateOnly StartDate, DateOnly? EndDate) : IRequest<UserSubscriptionDto>;

public class AssignUserSubscriptionHandler : IRequestHandler<AssignUserSubscriptionCommand, UserSubscriptionDto>
{
    private readonly IRepository<UserSubscription> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignUserSubscriptionHandler(IRepository<UserSubscription> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserSubscriptionDto> Handle(AssignUserSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var entity = new UserSubscription
        {
            UserId = request.UserId,
            SubscriptionTypeId = request.SubscriptionTypeId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Active = true
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new UserSubscriptionDto(entity.Id, entity.UserId, entity.SubscriptionTypeId, entity.StartDate, entity.EndDate, entity.Active, entity.CreatedAt, entity.UpdatedAt);
    }
}
