using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weather.Infrastructure.ExternalServices;
using Weather.Infrastructure.Mongo;
using Weather.Domain.Entities;
using System.Security.Claims;

namespace Weather.WeatherService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WeatherController : ControllerBase
{
    private readonly WeatherServiceFactory _factory;
    private readonly WeatherLogRepository _logRepository;

    public WeatherController(
        WeatherServiceFactory factory,
        WeatherLogRepository logRepository)
    {
        _factory = factory;
        _logRepository = logRepository;
    }

    [HttpGet("{city}")]
    public async Task<IActionResult> GetWeather(string city)
    {
        var service = _factory.Create("OpenWeather");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        try
        {
            var result = await service.GetWeatherAsync(city);

            await _logRepository.InsertLogAsync(new WeatherLog
            {
                UserId = userId,
                City = city,
                RequestedAt = DateTime.UtcNow,
                ErrorMessage = null
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            await _logRepository.InsertLogAsync(new WeatherLog
            {
                UserId = userId,
                City = city,
                RequestedAt = DateTime.UtcNow,
                ErrorMessage = ex.Message
            });

            return StatusCode(500, "An error occurred while fetching weather data.");
        }
    }

    [HttpGet("by-coordinates")]
    public async Task<IActionResult> GetWeatherByCoordinates(
        double lat,
        double lon)
    {
        var service = _factory.Create("OpenWeather");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        try
        {
            var result = await service
                .GetWeatherByCoordinatesAsync(lat, lon);

            await _logRepository.InsertLogAsync(new WeatherLog
            {
                UserId = userId,
                City = $"Lat:{lat}, Lon:{lon}",
                RequestedAt = DateTime.UtcNow,
                ErrorMessage = null
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            await _logRepository.InsertLogAsync(new WeatherLog
            {
                UserId = userId,
                City = $"Lat:{lat}, Lon:{lon}",
                RequestedAt = DateTime.UtcNow,
                ErrorMessage = ex.Message
            });

            return StatusCode(500, "An error occurred while fetching weather data.");
        }
    }
}
