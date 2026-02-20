using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Users.Commands;

public record SetUserEnabledCommand(long Id, bool Enabled) : IRequest;

public class SetUserEnabledHandler : IRequestHandler<SetUserEnabledCommand>
{
    private readonly IRepository<User> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SetUserEnabledHandler(IRepository<User> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SetUserEnabledCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"User {request.Id} not found.");
        entity.Enabled = request.Enabled;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
