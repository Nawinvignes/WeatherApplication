using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weather.Infrastructure.Mongo;
using System.Security.Claims;

namespace Weather.WeatherService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WeatherLogController : ControllerBase
{
    private readonly WeatherLogRepository _logRepository;

    public WeatherLogController(WeatherLogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    // GET /api/weatherlog — returns only the logged-in user's logs
    [HttpGet]
    public async Task<IActionResult> GetLogs()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token.");

        var logs = await _logRepository.GetLogsByUserAsync(userId);
        return Ok(logs);
    }

    // GET /api/weatherlog/all — admin only, returns all logs
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllLogs()
    {
        var logs = await _logRepository.GetAllLogsAsync();
        return Ok(logs);
    }
}
