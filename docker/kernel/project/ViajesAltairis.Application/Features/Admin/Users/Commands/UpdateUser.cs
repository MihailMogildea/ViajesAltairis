using MediatR;
using ViajesAltairis.Application.Features.Admin.Users.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.Users.Commands;

public record UpdateUserCommand(long Id, long UserTypeId, string Email, string FirstName, string LastName, string? Phone, string? TaxId, string? Address, string? City, string? PostalCode, string? Country, long? LanguageId, long? BusinessPartnerId, long? ProviderId, decimal Discount) : IRequest<UserDto>;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly IRepository<User> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserHandler(IRepository<User> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"User {request.Id} not found.");
        entity.UserTypeId = request.UserTypeId;
        entity.Email = request.Email;
        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Phone = request.Phone;
        entity.TaxId = request.TaxId;
        entity.Address = request.Address;
        entity.City = request.City;
        entity.PostalCode = request.PostalCode;
        entity.Country = request.Country;
        entity.LanguageId = request.LanguageId;
        entity.BusinessPartnerId = request.BusinessPartnerId;
        entity.ProviderId = request.ProviderId;
        entity.Discount = request.Discount;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new UserDto { Id = entity.Id, UserTypeId = entity.UserTypeId, Email = entity.Email, FirstName = entity.FirstName, LastName = entity.LastName, Phone = entity.Phone, TaxId = entity.TaxId, Address = entity.Address, City = entity.City, PostalCode = entity.PostalCode, Country = entity.Country, LanguageId = entity.LanguageId, BusinessPartnerId = entity.BusinessPartnerId, ProviderId = entity.ProviderId, Discount = entity.Discount, Enabled = entity.Enabled, CreatedAt = entity.CreatedAt };
    }
}
