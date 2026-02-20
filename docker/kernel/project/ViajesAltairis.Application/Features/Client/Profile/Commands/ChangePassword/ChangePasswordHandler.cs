using MediatR;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Profile.Commands.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IPasswordService _passwordService;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordHandler(IUserRepository userRepository, ICurrentUserService currentUser, IPasswordService passwordService, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
        _passwordService = passwordService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_currentUser.UserId!.Value, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        if (!_passwordService.Verify(request.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Current password is incorrect.");

        user.PasswordHash = _passwordService.Hash(request.NewPassword);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
