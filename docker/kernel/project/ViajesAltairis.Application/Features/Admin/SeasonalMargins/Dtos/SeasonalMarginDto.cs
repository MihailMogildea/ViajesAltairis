namespace ViajesAltairis.Application.Features.Admin.SeasonalMargins.Dtos;

public record SeasonalMarginDto(long Id, long AdministrativeDivisionId, string StartMonthDay, string EndMonthDay, decimal Margin, DateTime CreatedAt, DateTime UpdatedAt);
