namespace ViajesAltairis.Domain.Entities;

public class BusinessPartner : BaseEntity
{
    public string CompanyName { get; set; } = null!;
    public string? TaxId { get; set; }
    public decimal Discount { get; set; }
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string? PostalCode { get; set; }
    public string Country { get; set; } = null!;
    public string ContactEmail { get; set; } = null!;
    public string? ContactPhone { get; set; }
    public bool Enabled { get; set; }

    public ICollection<User> Users { get; set; } = [];
    public ICollection<Invoice> Invoices { get; set; } = [];
}
