using MediatR;
using ViajesAltairis.Application.Features.Admin.Users.Dtos;
using ViajesAltairis.Application.Interfaces;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Users.Commands;

public record CreateUserCommand(long UserTypeId, string Email, string Password, string FirstName, string LastName, string? Phone, string? TaxId, string? Address, string? City, string? PostalCode, string? Country, long? LanguageId, long? BusinessPartnerId, long? ProviderId, decimal Discount) : IRequest<UserDto>;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IRepository<User> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;

    public CreateUserHandler(IRepository<User> repository, IUnitOfWork unitOfWork, IPasswordService passwordService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var entity = new User
        {
            UserTypeId = request.UserTypeId,
            Email = request.Email,
            PasswordHash = _passwordService.Hash(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            TaxId = request.TaxId,
            Address = request.Address,
            City = request.City,
            PostalCode = request.PostalCode,
            Country = request.Country,
            LanguageId = request.LanguageId,
            BusinessPartnerId = request.BusinessPartnerId,
            ProviderId = request.ProviderId,
            Discount = request.Discount,
            Enabled = true
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new UserDto { Id = entity.Id, UserTypeId = entity.UserTypeId, Email = entity.Email, FirstName = entity.FirstName, LastName = entity.LastName, Phone = entity.Phone, TaxId = entity.TaxId, Address = entity.Address, City = entity.City, PostalCode = entity.PostalCode, Country = entity.Country, LanguageId = entity.LanguageId, BusinessPartnerId = entity.BusinessPartnerId, ProviderId = entity.ProviderId, Discount = entity.Discount, Enabled = entity.Enabled, CreatedAt = entity.CreatedAt };
    }
}
