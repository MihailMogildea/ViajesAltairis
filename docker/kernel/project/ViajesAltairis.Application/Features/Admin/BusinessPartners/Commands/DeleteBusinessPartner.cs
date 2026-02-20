using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.BusinessPartners.Commands;

public record DeleteBusinessPartnerCommand(long Id) : IRequest;

public class DeleteBusinessPartnerHandler : IRequestHandler<DeleteBusinessPartnerCommand>
{
    private readonly IRepository<BusinessPartner> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBusinessPartnerHandler(IRepository<BusinessPartner> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteBusinessPartnerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"BusinessPartner {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
