using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Weather.Application.Interfaces;
using Weather.Domain.Entities;

namespace Weather.Infrastructure.ExternalServices;

public class OpenWeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OpenWeatherService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    // =========================================
    // Get Weather By City
    // =========================================
    public async Task<WeatherResponse> GetWeatherAsync(string city)
    {
        var apiKey = _configuration["OpenWeather:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new Exception("OpenWeather API Key is missing.");

        var url =
            $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

        return await FetchWeather(url);
    }

    // =========================================
    // Get Weather By Coordinates
    // =========================================
    public async Task<WeatherResponse> GetWeatherByCoordinatesAsync(double lat, double lon)
    {
        var apiKey = _configuration["OpenWeather:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new Exception("OpenWeather API Key is missing.");

        var url =
            $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={apiKey}&units=metric";

        return await FetchWeather(url);
    }

    // =========================================
    // Common Weather Fetch Logic
    // =========================================
    private async Task<WeatherResponse> FetchWeather(string url)
    {
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"OpenWeather API Error: {error}");
        }

        var json = await response.Content.ReadAsStringAsync();
        using var data = JsonDocument.Parse(json);

        // 🔥 Builder Pattern Used
        return new WeatherResponse.Builder()
            .SetCity(data.RootElement.GetProperty("name").GetString() ?? "")
            .SetCountry(data.RootElement
                .GetProperty("sys")
                .GetProperty("country")
                .GetString() ?? "")
            .SetDescription(data.RootElement
                .GetProperty("weather")[0]
                .GetProperty("description")
                .GetString() ?? "")
            .SetTemperature(data.RootElement
                .GetProperty("main")
                .GetProperty("temp")
                .GetDouble())
            .SetHumidity(data.RootElement
                .GetProperty("main")
                .GetProperty("humidity")
                .GetInt32())
            .SetPressure(data.RootElement
                .GetProperty("main")
                .GetProperty("pressure")
                .GetInt32())
            .SetWindSpeed(data.RootElement
                .GetProperty("wind")
                .GetProperty("speed")
                .GetDouble())
            .SetCoordinates(
                data.RootElement
                    .GetProperty("coord")
                    .GetProperty("lat")
                    .GetDouble(),
                data.RootElement
                    .GetProperty("coord")
                    .GetProperty("lon")
                    .GetDouble())
            .SetSunTimes(
                data.RootElement
                    .GetProperty("sys")
                    .GetProperty("sunrise")
                    .GetInt64(),
                data.RootElement
                    .GetProperty("sys")
                    .GetProperty("sunset")
                    .GetInt64())
            .Build();
    }
}