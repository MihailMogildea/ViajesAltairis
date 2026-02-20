using MediatR;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Auth.Commands.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly ICacheService _cacheService;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordHandler(ICacheService cacheService, IUserRepository userRepository, IPasswordService passwordService, IUnitOfWork unitOfWork)
    {
        _cacheService = cacheService;
        _userRepository = userRepository;
        _passwordService = passwordService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = await _cacheService.GetAsync<long?>($"pwd-reset:{request.Token}", cancellationToken);
        if (userId == null)
            throw new InvalidOperationException("Invalid or expired reset token.");

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        user.PasswordHash = _passwordService.Hash(request.NewPassword);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveAsync($"pwd-reset:{request.Token}", cancellationToken);

        return Unit.Value;
    }
}
