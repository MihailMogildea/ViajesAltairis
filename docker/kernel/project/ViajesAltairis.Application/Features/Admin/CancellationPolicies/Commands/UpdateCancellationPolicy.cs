using MediatR;
using ViajesAltairis.Application.Features.Admin.CancellationPolicies.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.CancellationPolicies.Commands;

public record UpdateCancellationPolicyCommand(long Id, long HotelId, int FreeCancellationHours, decimal PenaltyPercentage) : IRequest<CancellationPolicyDto>;

public class UpdateCancellationPolicyHandler : IRequestHandler<UpdateCancellationPolicyCommand, CancellationPolicyDto>
{
    private readonly IRepository<CancellationPolicy> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCancellationPolicyHandler(IRepository<CancellationPolicy> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CancellationPolicyDto> Handle(UpdateCancellationPolicyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"CancellationPolicy {request.Id} not found.");
        entity.HotelId = request.HotelId;
        entity.FreeCancellationHours = request.FreeCancellationHours;
        entity.PenaltyPercentage = request.PenaltyPercentage;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CancellationPolicyDto(entity.Id, entity.HotelId, entity.FreeCancellationHours, entity.PenaltyPercentage, entity.Enabled, entity.CreatedAt);
    }
}
