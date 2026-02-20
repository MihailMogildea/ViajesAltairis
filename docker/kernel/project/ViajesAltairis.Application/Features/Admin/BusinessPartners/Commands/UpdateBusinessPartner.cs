using MediatR;
using ViajesAltairis.Application.Features.Admin.BusinessPartners.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.BusinessPartners.Commands;

public record UpdateBusinessPartnerCommand(long Id, string CompanyName, string? TaxId, decimal Discount, string Address, string City, string? PostalCode, string Country, string ContactEmail, string? ContactPhone) : IRequest<BusinessPartnerDto>;

public class UpdateBusinessPartnerHandler : IRequestHandler<UpdateBusinessPartnerCommand, BusinessPartnerDto>
{
    private readonly IRepository<BusinessPartner> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBusinessPartnerHandler(IRepository<BusinessPartner> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BusinessPartnerDto> Handle(UpdateBusinessPartnerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"BusinessPartner {request.Id} not found.");
        entity.CompanyName = request.CompanyName;
        entity.TaxId = request.TaxId;
        entity.Discount = request.Discount;
        entity.Address = request.Address;
        entity.City = request.City;
        entity.PostalCode = request.PostalCode;
        entity.Country = request.Country;
        entity.ContactEmail = request.ContactEmail;
        entity.ContactPhone = request.ContactPhone;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new BusinessPartnerDto(entity.Id, entity.CompanyName, entity.TaxId, entity.Discount, entity.Address, entity.City, entity.PostalCode, entity.Country, entity.ContactEmail, entity.ContactPhone, entity.Enabled, entity.CreatedAt);
    }
}
