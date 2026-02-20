namespace ViajesAltairis.Domain.Entities;

public class SeasonalMargin : AuditableEntity
{
    public long AdministrativeDivisionId { get; set; }
    public string StartMonthDay { get; set; } = null!;
    public string EndMonthDay { get; set; } = null!;
    public decimal Margin { get; set; }

    public AdministrativeDivision AdministrativeDivision { get; set; } = null!;
}
