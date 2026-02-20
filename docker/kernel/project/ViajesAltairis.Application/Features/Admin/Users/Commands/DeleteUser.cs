using MediatR;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Users.Commands;

public record DeleteUserCommand(long Id) : IRequest;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IRepository<User> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserHandler(IRepository<User> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"User {request.Id} not found.");
        await _repository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
