namespace Weather.Domain.Entities;

public class WeatherResponse
{
    public string City { get; set; } = "";
    public string Country { get; set; } = "";
    public string Description { get; set; } = "";
    public double Temperature { get; set; }
    public int Humidity { get; set; }
    public int Pressure { get; set; }
    public double WindSpeed { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public long Sunrise { get; set; }
    public long Sunset { get; set; }

    // 🔥 Nested Builder Class
    public class Builder
    {
        private readonly WeatherResponse _weather = new WeatherResponse();

        public Builder SetCity(string city)
        {
            _weather.City = city;
            return this;
        }

        public Builder SetCountry(string country)
        {
            _weather.Country = country;
            return this;
        }

        public Builder SetDescription(string description)
        {
            _weather.Description = description;
            return this;
        }

        public Builder SetTemperature(double temperature)
        {
            _weather.Temperature = temperature;
            return this;
        }

        public Builder SetHumidity(int humidity)
        {
            _weather.Humidity = humidity;
            return this;
        }

        public Builder SetPressure(int pressure)
        {
            _weather.Pressure = pressure;
            return this;
        }

        public Builder SetWindSpeed(double windSpeed)
        {
            _weather.WindSpeed = windSpeed;
            return this;
        }

        public Builder SetCoordinates(double lat, double lon)
        {
            _weather.Latitude = lat;
            _weather.Longitude = lon;
            return this;
        }

        public Builder SetSunTimes(long sunrise, long sunset)
        {
            _weather.Sunrise = sunrise;
            _weather.Sunset = sunset;
            return this;
        }

        public WeatherResponse Build()
        {
            return _weather;
        }
    }
}