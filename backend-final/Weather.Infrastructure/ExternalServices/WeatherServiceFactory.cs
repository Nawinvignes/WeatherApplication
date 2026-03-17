using Microsoft.Extensions.DependencyInjection;
using Weather.Application.Interfaces;

namespace Weather.Infrastructure.ExternalServices;

public class WeatherServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public WeatherServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IWeatherService Create(string providerName)
    {
        return providerName switch
        {
            "OpenWeather" => _serviceProvider.GetRequiredService<IWeatherService>(),
            _ => throw new ArgumentException("Invalid weather provider")
        };
    }
}