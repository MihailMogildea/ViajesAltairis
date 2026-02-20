using MediatR;
using ViajesAltairis.Application.Features.Admin.CancellationPolicies.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.CancellationPolicies.Commands;

public record CreateCancellationPolicyCommand(long HotelId, int FreeCancellationHours, decimal PenaltyPercentage) : IRequest<CancellationPolicyDto>;

public class CreateCancellationPolicyHandler : IRequestHandler<CreateCancellationPolicyCommand, CancellationPolicyDto>
{
    private readonly IRepository<CancellationPolicy> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCancellationPolicyHandler(IRepository<CancellationPolicy> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CancellationPolicyDto> Handle(CreateCancellationPolicyCommand request, CancellationToken cancellationToken)
    {
        var entity = new CancellationPolicy
        {
            HotelId = request.HotelId,
            FreeCancellationHours = request.FreeCancellationHours,
            PenaltyPercentage = request.PenaltyPercentage
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CancellationPolicyDto(entity.Id, entity.HotelId, entity.FreeCancellationHours, entity.PenaltyPercentage, entity.Enabled, entity.CreatedAt);
    }
}
