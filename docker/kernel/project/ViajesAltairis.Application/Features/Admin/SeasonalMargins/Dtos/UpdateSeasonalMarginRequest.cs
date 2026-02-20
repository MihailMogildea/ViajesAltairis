namespace ViajesAltairis.Application.Features.Admin.SeasonalMargins.Dtos;

public record UpdateSeasonalMarginRequest(long AdministrativeDivisionId, string StartMonthDay, string EndMonthDay, decimal Margin);
