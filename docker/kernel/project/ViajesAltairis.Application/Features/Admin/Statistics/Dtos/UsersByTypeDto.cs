namespace ViajesAltairis.Application.Features.Admin.Statistics.Dtos;

public class UsersByTypeDto
{
    public string TypeName { get; init; } = null!;
    public int UserCount { get; init; }
}
