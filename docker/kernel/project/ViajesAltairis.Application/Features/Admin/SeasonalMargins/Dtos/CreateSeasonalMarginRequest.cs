namespace ViajesAltairis.Application.Features.Admin.SeasonalMargins.Dtos;

public record CreateSeasonalMarginRequest(long AdministrativeDivisionId, string StartMonthDay, string EndMonthDay, decimal Margin);
