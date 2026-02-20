using Microsoft.AspNetCore.Mvc;
using ViajesAltairis.ScheduledApi.Controllers;

namespace ViajesAltairis.Scheduled.Api.Tests.Controllers;

public class HealthControllerTests
{
    [Fact]
    public void Get_ReturnsOk()
    {
        var controller = new HealthController();
        var result = controller.Get();
        result.Should().BeOfType<OkObjectResult>();
    }
}
