using MediatR;
using ViajesAltairis.Application.Features.Admin.BusinessPartners.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.BusinessPartners.Commands;

public record CreateBusinessPartnerCommand(string CompanyName, string? TaxId, decimal Discount, string Address, string City, string? PostalCode, string Country, string ContactEmail, string? ContactPhone) : IRequest<BusinessPartnerDto>;

public class CreateBusinessPartnerHandler : IRequestHandler<CreateBusinessPartnerCommand, BusinessPartnerDto>
{
    private readonly IRepository<BusinessPartner> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBusinessPartnerHandler(IRepository<BusinessPartner> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BusinessPartnerDto> Handle(CreateBusinessPartnerCommand request, CancellationToken cancellationToken)
    {
        var entity = new BusinessPartner
        {
            CompanyName = request.CompanyName,
            TaxId = request.TaxId,
            Discount = request.Discount,
            Address = request.Address,
            City = request.City,
            PostalCode = request.PostalCode,
            Country = request.Country,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            Enabled = true
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new BusinessPartnerDto(entity.Id, entity.CompanyName, entity.TaxId, entity.Discount, entity.Address, entity.City, entity.PostalCode, entity.Country, entity.ContactEmail, entity.ContactPhone, entity.Enabled, entity.CreatedAt);
    }
}
