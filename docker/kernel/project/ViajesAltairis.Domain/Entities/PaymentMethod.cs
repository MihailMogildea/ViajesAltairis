namespace ViajesAltairis.Domain.Entities;

public class PaymentMethod : BaseEntity
{
    public string Name { get; set; } = null!;
    public int MinDaysBeforeCheckin { get; set; }
    public bool Enabled { get; set; }

    public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = [];
}
