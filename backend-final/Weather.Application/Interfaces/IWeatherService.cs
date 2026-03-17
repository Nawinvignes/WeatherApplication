using Weather.Domain.Entities;

namespace Weather.Application.Interfaces;

public interface IWeatherService
{
    Task<WeatherResponse> GetWeatherAsync(string city);
    Task<WeatherResponse> GetWeatherByCoordinatesAsync(double lat, double lon);
}