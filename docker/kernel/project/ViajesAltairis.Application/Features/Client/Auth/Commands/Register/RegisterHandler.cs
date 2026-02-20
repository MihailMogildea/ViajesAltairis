using MediatR;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Client.Auth.Commands.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterHandler(IUserRepository userRepository, IPasswordService passwordService, IJwtTokenService jwtTokenService, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException("A user with this email already exists.");

        var user = new User
        {
            UserTypeId = 5, // Client
            Email = request.Email,
            PasswordHash = _passwordService.Hash(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            LanguageId = request.PreferredLanguageId,
            Enabled = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenService.GenerateToken(user.Id, user.Email, "Client");

        return new RegisterResponse
        {
            UserId = user.Id,
            Token = token
        };
    }
}
