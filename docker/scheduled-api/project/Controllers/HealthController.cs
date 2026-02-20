using Microsoft.AspNetCore.Mvc;

namespace ViajesAltairis.ScheduledApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "healthy", service = "scheduled-api" });
}
