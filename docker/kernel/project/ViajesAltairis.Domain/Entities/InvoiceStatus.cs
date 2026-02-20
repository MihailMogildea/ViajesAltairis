namespace ViajesAltairis.Domain.Entities;

public class InvoiceStatus : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<Invoice> Invoices { get; set; } = [];
}
