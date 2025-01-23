namespace WeatherApiWrapperService.Contracts;

public interface IWeatherService
{
    Task<string?> GetWeatherAsync(string city, string country);

    Task<string?> GetWeatherByLocationAsync(double latitude, double longitude);

    Task DeleteAllCaches();
    Task DeleteCache(string key);
}
