using MediatR;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Profile.Commands.UpdateProfile;

public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, UpdateProfileResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileHandler(IUserRepository userRepository, ICurrentUserService currentUser, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateProfileResponse> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_currentUser.UserId!.Value, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        if (request.FirstName != null)
            user.FirstName = request.FirstName;

        if (request.LastName != null)
            user.LastName = request.LastName;

        if (request.Phone != null)
            user.Phone = request.Phone;

        if (request.PreferredLanguageId.HasValue)
            user.LanguageId = request.PreferredLanguageId.Value;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateProfileResponse { UserId = user.Id };
    }
}
